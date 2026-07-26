using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetItem : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text amountText;
    
    private int targetType;
    private int targetAmount;
    private int currentAmount;
    private Color originalTextColor;
    
    public void SetIcon(Sprite sprite)
    {
        if (iconImage != null)
        {
            iconImage.sprite = sprite;
        }
    }
    
    void Awake()
    {
        if (iconImage == null)
        {
            Transform iconTrans = transform.Find("Icon");
            if (iconTrans != null)
            {
                iconImage = iconTrans.GetComponent<Image>();
            }
        }
        
        if (amountText == null)
        {
            Transform amountTrans = transform.Find("Amount");
            if (amountTrans != null)
            {
                amountText = amountTrans.GetComponent<Text>();
            }
        }
        
        if (amountText != null)
        {
            originalTextColor = amountText.color;
        }
    }
    
    public void Initialize(int type, int amount)
    {
        targetType = type;
        targetAmount = amount;
        currentAmount = amount;
        UpdateDisplay();
    }
    
    public void SubtractAmount(int count)
    {
        currentAmount = Mathf.Max(0, currentAmount - count);
        UpdateDisplay();
    }
    
    private void UpdateDisplay()
    {
        if (amountText != null)
        {
            bool isCompleted = currentAmount <= 0;
            if (isCompleted)
            {
                amountText.text = "✓";
                amountText.color = Color.green;
            }
            else
            {
                amountText.text = currentAmount.ToString();
                amountText.color = originalTextColor;
            }
        }
        
        if (iconImage != null)
        {
            bool isCompleted = currentAmount <= 0;
            iconImage.color = isCompleted ? new Color(0.5f, 0.5f, 0.5f) : Color.white;
        }
    }
    
    public bool IsCompleted()
    {
        return currentAmount <= 0;
    }
    
    public int GetTargetType()
    {
        return targetType;
    }
}