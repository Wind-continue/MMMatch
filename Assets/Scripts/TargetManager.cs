using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private Transform targetArea;
    [SerializeField] private TargetItem targetItemPrefab;
    
    private List<TargetData> levelTargets = new List<TargetData>();
    private List<TargetItem> targetItems = new List<TargetItem>();
    
    public static TargetManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeTargets();
    }
    
    private void InitializeTargets()
    {
        // 清除现有目标
        foreach (TargetItem item in targetItems)
        {
            Destroy(item.gameObject);
        }
        targetItems.Clear();
        
        // 创建默认目标（直接在代码中定义）
        levelTargets.Clear();
        CreateDefaultTargets();
        
        // 检查引用
        if (targetArea == null)
        {
            Debug.LogError("TargetArea is null! Please assign in Inspector.");
            return;
        }
        
        if (targetItemPrefab == null)
        {
            Debug.LogError("TargetItemPrefab is null! Please assign in Inspector.");
            return;
        }
        
        Debug.Log("Initializing " + levelTargets.Count + " targets...");
        
        // 创建目标项
        float spacing = 140f;
        float startX = -(levelTargets.Count - 1) * spacing / 2;
        
        for (int i = 0; i < levelTargets.Count; i++)
        {
            TargetData data = levelTargets[i];
            TargetItem targetItem = Instantiate(targetItemPrefab, targetArea);
            targetItem.name = "Target_" + i + "_Type" + data.type;
            
            RectTransform rect = targetItem.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = new Vector2(startX + i * spacing, 0);
                rect.localScale = Vector3.one;
            }
            
            // 设置图标（从Resources加载）
            int iconNumber = data.type + 1;
            string spritePath = "Items/item_0" + iconNumber;
            Sprite sprite = Resources.Load<Sprite>(spritePath);
            if (sprite != null)
            {
                targetItem.SetIcon(sprite);
            }
            else
            {
                Debug.LogWarning("Icon not found at: Resources/" + spritePath + ".png");
            }
            
            targetItem.Initialize(data.type, data.amount);
            targetItems.Add(targetItem);
            
            Debug.Log("Created target: type=" + data.type + ", amount=" + data.amount);
        }
    }
    
    private void CreateDefaultTargets()
    {
        // 创建默认目标数据（数量分别为5, 5, 10）
        levelTargets.Add(new TargetData(0, 5));  // type 0: 樱桃
        levelTargets.Add(new TargetData(1, 5));  // type 1: 橙子
        levelTargets.Add(new TargetData(2, 10)); // type 2: 方块
    }
    
    public void OnItemMatched(int itemType, int count)
    {
        foreach (TargetItem targetItem in targetItems)
        {
            if (targetItem.GetTargetType() == itemType && !targetItem.IsCompleted())
            {
                targetItem.SubtractAmount(count);
                
                // 检查是否所有目标都完成
                if (CheckAllCompleted())
                {
                    OnAllTargetsCompleted();
                }
                
                break;
            }
        }
    }
    
    private bool CheckAllCompleted()
    {
        foreach (TargetItem targetItem in targetItems)
        {
            if (!targetItem.IsCompleted())
            {
                return false;
            }
        }
        return true;
    }
    
    private void OnAllTargetsCompleted()
    {
        Debug.Log("All targets completed!");
        // 可以触发胜利逻辑
        if (GamingController.Instance != null)
        {
            GamingController.Instance.OnAllTargetsCompleted();
        }
    }
    
    [System.Serializable]
    public class TargetData
    {
        public int type;
        public int amount;
        
        public TargetData(int type, int amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}