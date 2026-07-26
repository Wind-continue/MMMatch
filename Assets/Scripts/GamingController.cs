using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamingController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject Title_Style1;
    [SerializeField] private GameObject Title_Style2;
    [SerializeField] private Text Step_Text;
    
    [Header("Game Settings")]
    public int totalSteps = 30;
    
    private int currentSteps;
    private bool hasStarted = false;
    
    public static GamingController Instance { get; private set; }
    
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
        // 初始化游戏状态
        currentSteps = totalSteps;
        UpdateStepText();
        
        // 全程显示 Title_Style2
        if (Title_Style1 != null)
            Title_Style1.SetActive(false);
        if (Title_Style2 != null)
            Title_Style2.SetActive(true);
    }
    
    public void OnSuccessfulMatch()
    {
        // 减少步数
        currentSteps--;
        UpdateStepText();
        
        // 检查是否步数用尽
        if (currentSteps <= 0)
        {
            TriggerGameOver();
        }
    }
    
    private void UpdateStepText()
    {
        if (Step_Text != null)
        {
            Step_Text.text = currentSteps.ToString();
        }
    }
    
    private void TriggerGameOver()
    {
        Debug.Log("Game Over! Steps: " + currentSteps);
        // 游戏结束弹窗接口（暂未实现）
        // GameOverPanel?.SetActive(true);
    }
    
    public void ResetGame()
    {
        currentSteps = totalSteps;
        UpdateStepText();
        
        // 保持显示 Title_Style2
        if (Title_Style1 != null)
            Title_Style1.SetActive(false);
        if (Title_Style2 != null)
            Title_Style2.SetActive(true);
    }
    
    public int GetCurrentSteps()
    {
        return currentSteps;
    }
    
    public void OnAllTargetsCompleted()
    {
        Debug.Log("Level Complete!");
        // 关卡完成逻辑（暂未实现）
        // WinPanel?.SetActive(true);
    }
}