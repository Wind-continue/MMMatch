using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ItemController : MonoBehaviour
{
    public int col;
    public int row;
    public float itemWidth = 160f;
    public float itemHeight = 160f;
    private SingleItem[,] ItemGrid;
    private UnityPool pool;
    private RectTransform rectTrans;
    private SingleItem selectedItem;
    private bool isSwapping = false;
    private bool isProcessing = false;

    // 滑动相关
    private SingleItem dragStartItem;
    private Vector2 dragStartPos;
    private bool isDragging = false;
    private const float swipeThreshold = 50f;
    
    // 游戏控制器引用
    [SerializeField] private GamingController gamingController;

    void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        if (rectTrans != null)
        {
            rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
            rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
            rectTrans.pivot = new Vector2(0.5f, 0.5f);
            rectTrans.sizeDelta = new Vector2(960f, 1280f);
        }
        
        pool = GetComponent<UnityPool>();
        if (pool == null)
        {
            pool = gameObject.AddComponent<UnityPool>();
        }
        
        InitializeGridWithoutMatches();
        
        // 如果没有手动指定 gamingController，尝试自动查找
        if (gamingController == null)
        {
            gamingController = FindObjectOfType<GamingController>();
        }
    }

    void InitializeGridWithoutMatches()
    {
        ItemGrid = new SingleItem[row, col];
        
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                ItemGrid[i, j] = CreateItem(i, j, true);
            }
        }
        
        while (HasInitialMatches())
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    SingleItem item = ItemGrid[i, j];
                    if (item != null)
                    {
                        item.BirthItem(i, j);
                    }
                }
            }
        }
    }

    bool HasInitialMatches()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (CheckHorizontalMatch(i, j).Count >= 3)
                {
                    return true;
                }
                if (CheckVerticalMatch(i, j).Count >= 3)
                {
                    return true;
                }
            }
        }
        return false;
    }

    SingleItem CreateItem(int i, int j, bool setPosition = false)
    {
        SingleItem item = pool.GetPooledItem();
        if (item == null) return null;
        
        RectTransform itemRect = item.GetComponent<RectTransform>();
        if (itemRect != null)
        {
            itemRect.SetParent(transform, false);
            itemRect.localScale = Vector3.one;
            itemRect.anchorMin = new Vector2(0, 1);
            itemRect.anchorMax = new Vector2(0, 1);
            itemRect.pivot = new Vector2(0.5f, 0.5f);
            itemRect.sizeDelta = new Vector2(itemWidth, itemHeight);
            
            if (setPosition)
            {
                float x = j * itemWidth + itemWidth / 2;
                float y = -i * itemHeight - itemHeight / 2;
                itemRect.anchoredPosition = new Vector2(x, y);
            }
        }
        
        item.SetController(this);
        item.BirthItem(i, j);
        ItemGrid[i, j] = item;
        return item;
    }

    public void OnItemClicked(SingleItem item)
    {
        if (isSwapping || isProcessing) return;
        
        if (selectedItem == null)
        {
            selectedItem = item;
            selectedItem.Select(true);
        }
        else if (selectedItem == item)
        {
            selectedItem.Select(false);
            selectedItem = null;
        }
        else
        {
            if (IsAdjacent(selectedItem, item))
            {
                StartCoroutine(SwapItems(selectedItem, item));
            }
            else
            {
                selectedItem.Select(false);
                selectedItem = item;
                selectedItem.Select(true);
            }
        }
    }

    private bool IsAdjacent(SingleItem item1, SingleItem item2)
    {
        int rowDiff = Mathf.Abs(item1.Row - item2.Row);
        int colDiff = Mathf.Abs(item1.Col - item2.Col);
        return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
    }

    private IEnumerator SwapItems(SingleItem item1, SingleItem item2)
    {
        isSwapping = true;
        if (selectedItem != null)
        {
            selectedItem.Select(false);
            selectedItem = null;
        }
        
        int row1 = item1.Row;
        int col1 = item1.Col;
        int row2 = item2.Row;
        int col2 = item2.Col;
        
        ItemGrid[row1, col1] = item2;
        ItemGrid[row2, col2] = item1;
        
        item1.SetItemIndex(row2, col2);
        item2.SetItemIndex(row1, col1);
        
        yield return StartCoroutine(MoveItemsTogether(item1, item2, row2, col2, row1, col1));
        
        yield return new WaitForSeconds(0.1f);
        
        if (!CheckMatches())
        {
            ItemGrid[row1, col1] = item1;
            ItemGrid[row2, col2] = item2;
            
            item1.SetItemIndex(row1, col1);
            item2.SetItemIndex(row2, col2);
            
            yield return StartCoroutine(MoveItemsTogether(item1, item2, row1, col1, row2, col2));
        }
        else
        {
            // 成功匹配，减少步数
            if (gamingController != null)
            {
                gamingController.OnSuccessfulMatch();
            }
        }
        
        isSwapping = false;
    }

    private IEnumerator MoveItemsTogether(SingleItem item1, SingleItem item2, int targetRow1, int targetCol1, int targetRow2, int targetCol2)
    {
        RectTransform rect1 = item1.GetComponent<RectTransform>();
        RectTransform rect2 = item2.GetComponent<RectTransform>();
        
        if (rect1 == null || rect2 == null) yield break;
        
        Vector2 startPos1 = rect1.anchoredPosition;
        Vector2 startPos2 = rect2.anchoredPosition;
        
        Vector2 targetPos1 = new Vector2(targetCol1 * itemWidth + itemWidth / 2, -targetRow1 * itemHeight - itemHeight / 2);
        Vector2 targetPos2 = new Vector2(targetCol2 * itemWidth + itemWidth / 2, -targetRow2 * itemHeight - itemHeight / 2);
        
        float duration = 0.25f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EaseOutCubic(t);
            
            rect1.anchoredPosition = Vector2.Lerp(startPos1, targetPos1, t);
            rect2.anchoredPosition = Vector2.Lerp(startPos2, targetPos2, t);
            
            yield return null;
        }
        
        rect1.anchoredPosition = targetPos1;
        rect2.anchoredPosition = targetPos2;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    void Update()
    {
    }

    bool CheckMatches()
    {
        List<SingleItem> matches = FindAllMatches();
        if (matches.Count > 0)
        {
            StartCoroutine(ProcessMatches(matches));
            return true;
        }
        return false;
    }

    List<SingleItem> FindAllMatches()
    {
        List<SingleItem> matches = new List<SingleItem>();
        
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                SingleItem item = ItemGrid[i, j];
                if (item == null) continue;
                
                List<SingleItem> horizontalMatch = CheckHorizontalMatch(i, j);
                List<SingleItem> verticalMatch = CheckVerticalMatch(i, j);
                
                foreach (SingleItem matchItem in horizontalMatch)
                {
                    if (!matches.Contains(matchItem))
                        matches.Add(matchItem);
                }
                foreach (SingleItem matchItem in verticalMatch)
                {
                    if (!matches.Contains(matchItem))
                        matches.Add(matchItem);
                }
            }
        }
        
        return matches;
    }

    List<SingleItem> CheckHorizontalMatch(int startRow, int startCol)
    {
        List<SingleItem> match = new List<SingleItem>();
        SingleItem startItem = ItemGrid[startRow, startCol];
        if (startItem == null) return match;
        
        match.Add(startItem);
        
        for (int j = startCol + 1; j < col; j++)
        {
            SingleItem item = ItemGrid[startRow, j];
            if (item != null && item.type == startItem.type)
            {
                match.Add(item);
            }
            else
            {
                break;
            }
        }
        
        for (int j = startCol - 1; j >= 0; j--)
        {
            SingleItem item = ItemGrid[startRow, j];
            if (item != null && item.type == startItem.type)
            {
                match.Insert(0, item);
            }
            else
            {
                break;
            }
        }
        
        return match.Count >= 3 ? match : new List<SingleItem>();
    }

    List<SingleItem> CheckVerticalMatch(int startRow, int startCol)
    {
        List<SingleItem> match = new List<SingleItem>();
        SingleItem startItem = ItemGrid[startRow, startCol];
        if (startItem == null) return match;
        
        match.Add(startItem);
        
        for (int i = startRow + 1; i < row; i++)
        {
            SingleItem item = ItemGrid[i, startCol];
            if (item != null && item.type == startItem.type)
            {
                match.Add(item);
            }
            else
            {
                break;
            }
        }
        
        for (int i = startRow - 1; i >= 0; i--)
        {
            SingleItem item = ItemGrid[i, startCol];
            if (item != null && item.type == startItem.type)
            {
                match.Insert(0, item);
            }
            else
            {
                break;
            }
        }
        
        return match.Count >= 3 ? match : new List<SingleItem>();
    }

    IEnumerator ProcessMatches(List<SingleItem> matches)
    {
        isProcessing = true;
        
        yield return StartCoroutine(DestroyMatches(matches));
        
        yield return new WaitForSeconds(0.1f);
        
        yield return StartCoroutine(DropItems());
        
        yield return StartCoroutine(FillEmptySpaces());
        
        yield return new WaitForSeconds(0.1f);
        
        List<SingleItem> newMatches = FindAllMatches();
        if (newMatches.Count > 0)
        {
            yield return StartCoroutine(ProcessMatches(newMatches));
        }
        
        isProcessing = false;
    }

    IEnumerator DestroyMatches(List<SingleItem> matches)
    {
        foreach (SingleItem item in matches)
        {
            ItemGrid[item.Row, item.Col] = null;
            pool.ReleasePooledItem(item);
        }
        
        yield return null;
    }

    IEnumerator DropItems()
    {
        List<SingleItem> itemsToMove = new List<SingleItem>();
        List<Vector2> targetPositions = new List<Vector2>();
        
        for (int j = 0; j < col; j++)
        {
            int dropDistance = 0;
            
            for (int i = row - 1; i >= 0; i--)
            {
                if (ItemGrid[i, j] == null)
                {
                    dropDistance++;
                }
                else if (dropDistance > 0)
                {
                    SingleItem item = ItemGrid[i, j];
                    int targetRow = i + dropDistance;
                    
                    ItemGrid[i, j] = null;
                    ItemGrid[targetRow, j] = item;
                    item.SetItemIndex(targetRow, j);
                    
                    itemsToMove.Add(item);
                    targetPositions.Add(new Vector2(j * itemWidth + itemWidth / 2, -targetRow * itemHeight - itemHeight / 2));
                }
            }
        }
        
        if (itemsToMove.Count > 0)
        {
            for (int i = 0; i < itemsToMove.Count; i++)
            {
                RectTransform rect = itemsToMove[i].GetComponent<RectTransform>();
                if (rect != null)
                {
                    StartCoroutine(MoveItemToPosition(rect, targetPositions[i]));
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator FillEmptySpaces()
    {
        List<SingleItem> itemsToDrop = new List<SingleItem>();
        List<RectTransform> rectsToMove = new List<RectTransform>();
        List<Vector2> targetPositions = new List<Vector2>();
        List<float> delays = new List<float>();
        
        for (int j = 0; j < col; j++)
        {
            for (int i = row - 1; i >= 0; i--)
            {
                if (ItemGrid[i, j] == null)
                {
                    SingleItem newItem = CreateItem(i, j);
                    if (newItem != null)
                    {
                        RectTransform rect = newItem.GetComponent<RectTransform>();
                        if (rect != null)
                        {
                            newItem.gameObject.SetActive(false);
                            float startY = itemHeight;
                            rect.anchoredPosition = new Vector2(j * itemWidth + itemWidth / 2, startY);
                            
                            Vector2 targetPos = new Vector2(j * itemWidth + itemWidth / 2, -i * itemHeight - itemHeight / 2);
                            
                            itemsToDrop.Add(newItem);
                            rectsToMove.Add(rect);
                            targetPositions.Add(targetPos);
                            delays.Add(j * 0.03f);
                        }
                    }
                }
            }
        }
        
        for (int k = 0; k < itemsToDrop.Count; k++)
        {
            StartCoroutine(MoveItemToPositionWithDelay(itemsToDrop[k], rectsToMove[k], targetPositions[k], delays[k]));
        }
        
        if (itemsToDrop.Count > 0)
        {
            yield return new WaitForSeconds(0.3f + col * 0.03f);
        }
    }

    IEnumerator MoveItemToPosition(RectTransform rect, Vector2 targetPos)
    {
        Vector2 startPos = rect.anchoredPosition;
        float duration = 0.25f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EaseOutCubic(t);
            
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            
            yield return null;
        }
        
        rect.anchoredPosition = targetPos;
    }

    IEnumerator MoveItemToPositionWithDelay(SingleItem item, RectTransform rect, Vector2 targetPos, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        item.gameObject.SetActive(true);
        
        Vector2 startPos = rect.anchoredPosition;
        float distance = Vector2.Distance(startPos, targetPos);
        float duration = Mathf.Clamp(distance / (itemHeight * 4), 0.15f, 0.3f);
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EaseOutBounce(t);
            
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            
            yield return null;
        }
        
        rect.anchoredPosition = targetPos;
    }

    private float EaseOutBounce(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;
        
        if (t < 1f / d1)
        {
            return n1 * t * t;
        }
        else if (t < 2f / d1)
        {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        }
        else if (t < 2.5f / d1)
        {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        }
        else
        {
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }

    // 滑动开始
    public void OnDragStarted(SingleItem item, PointerEventData eventData)
    {
        if (isSwapping || isProcessing || item == null) return;
        
        dragStartItem = item;
        dragStartPos = eventData.position;
        isDragging = true;
        
        // 选中起始物品
        if (selectedItem != null && selectedItem != item)
        {
            selectedItem.Select(false);
        }
        selectedItem = item;
        selectedItem.Select(true);
    }
    
    // 滑动中
    public void OnDragging(PointerEventData eventData)
    {
        if (!isDragging || isSwapping || isProcessing) return;
    }
    
    // 滑动结束
    public void OnDragEnded(PointerEventData eventData)
    {
        if (!isDragging || isSwapping || isProcessing || dragStartItem == null) return;
        
        isDragging = false;
        
        Vector2 dragEndPos = eventData.position;
        Vector2 dragDelta = dragEndPos - dragStartPos;
        
        // 检查滑动距离是否超过阈值
        if (dragDelta.magnitude >= swipeThreshold)
        {
            // 判断滑动方向
            int targetRow = dragStartItem.Row;
            int targetCol = dragStartItem.Col;
            
            if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
            {
                // 水平滑动
                if (dragDelta.x > 0)
                {
                    targetCol = dragStartItem.Col + 1; // 向右
                }
                else
                {
                    targetCol = dragStartItem.Col - 1; // 向左
                }
            }
            else
            {
                // 垂直滑动
                if (dragDelta.y > 0)
                {
                    targetRow = dragStartItem.Row - 1; // 向上
                }
                else
                {
                    targetRow = dragStartItem.Row + 1; // 向下
                }
            }
            
            // 检查目标位置是否有效
            if (targetRow >= 0 && targetRow < row && targetCol >= 0 && targetCol < col)
            {
                SingleItem targetItem = ItemGrid[targetRow, targetCol];
                if (targetItem != null)
                {
                    StartCoroutine(SwapItems(dragStartItem, targetItem));
                    if (selectedItem != null)
                    {
                        selectedItem.Select(false);
                        selectedItem = null;
                    }
                    dragStartItem = null;
                    return;
                }
            }
        }
        
        // 如果没有成功交换，保持选中状态不变
        dragStartItem = null;
    }
    
}