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
        
        // 显示 Title_Style1，隐藏 Title_Style2
        if (Title_Style1 != null)
            Title_Style1.SetActive(true);
        if (Title_Style2 != null)
            Title_Style2.SetActive(false);
    }
    
    public void OnSuccessfulMatch()
    {
        // 首次成功匹配时切换到 Style2
        if (!hasStarted)
        {
            SwitchToStyle2();
            hasStarted = true;
        }
        
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
    
    private void SwitchToStyle2()
    {
        if (Title_Style1 != null)
            Title_Style1.SetActive(false);
        if (Title_Style2 != null)
            Title_Style2.SetActive(true);
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
        hasStarted = false;
        UpdateStepText();
        
        if (Title_Style1 != null)
            Title_Style1.SetActive(true);
        if (Title_Style2 != null)
            Title_Style2.SetActive(false);
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