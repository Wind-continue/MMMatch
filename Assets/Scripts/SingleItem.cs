using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SingleItem : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private int _row;
    private int _col;
    public List<Sprite> _sprites;
    public int type;
    private Image _itemImage;
    private ItemController _controller;
    private Image _borderImage;

    public int Row { get { return _row; } }
    public int Col { get { return _col; } }

    void Awake()
    {
        Transform fruitTrans = transform.Find("Fruit");
        if (fruitTrans != null)
        {
            _itemImage = fruitTrans.GetComponent<Image>();
            if (_itemImage == null)
            {
                _itemImage = fruitTrans.gameObject.AddComponent<Image>();
            }
        }
        else
        {
            Debug.LogError("Child object 'Fruit' not found in " + gameObject.name);
        }
        
        Transform borderTrans = transform.Find("Border");
        if (borderTrans != null)
        {
            _borderImage = borderTrans.GetComponent<Image>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_controller != null)
        {
            _controller.OnDragStarted(this, eventData);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (_controller != null)
        {
            _controller.OnDragging(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_controller != null)
        {
            _controller.OnDragEnded(eventData);
        }
    }

    public void SetController(ItemController controller)
    {
        _controller = controller;
    }

    public void BirthItem(int row, int col)
    {
        SetItemIndex(row, col);
        SetItemSpriteRandom();
        Select(false);
    }

    private void SetItemSpriteRandom()
    {
        if (_itemImage == null || _sprites == null || _sprites.Count == 0)
            return;
            
        int index = Random.Range(0, _sprites.Count);
        type = index;
        _itemImage.sprite = _sprites[index];
    }

    public void SetItemIndex(int row, int col)
    {
        this._row = row;
        this._col = col;
    }

    public void Select(bool isSelected)
    {
        if (_borderImage != null)
        {
            _borderImage.enabled = isSelected;
        }
        else
        {
            transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_controller != null)
        {
            _controller.OnItemClicked(this);
        }
    }
}