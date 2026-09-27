using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MMMatch.UI;

public class TargetManager : MonoBehaviour
{
    [SerializeField] private Transform targetArea;
    [SerializeField] private TargetItem targetItemPrefab;
    
    private List<TargetData> levelTargets = new List<TargetData>();
    private List<TargetItem> targetItems = new List<TargetItem>();
    
    public static TargetManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
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
        
        // 创建默认目标
        levelTargets.Clear();
        LoadTargetsFromConfig();
        
        // 自动查找 TargetArea
        FindTargetArea();
        
        // 检查引用
        if (targetArea == null)
        {
            Debug.LogError("TargetArea is null! Please assign in Inspector.");
            return;
        }
        
        Debug.Log("TargetArea found: " + targetArea.name);
        
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
    
    private void FindTargetArea()
    {
        if (targetArea != null)
            return;
        
        Debug.LogWarning("TargetArea not assigned in Inspector. Trying to find automatically...");
        
        // 尝试多种路径查找
        string[] possiblePaths = new string[]
        {
            "TargetArea",
            "/UIRoot/GamingUI/TargetArea",
            "/UIRoot/Title_Style2/TargetArea",
            "/Canvas/GamingUI/TargetArea",
            "GamingUI/TargetArea",
            "Title_Style2/TargetArea"
        };
        
        foreach (string path in possiblePaths)
        {
            Transform area = GameObject.Find(path)?.transform;
            if (area != null)
            {
                targetArea = area;
                Debug.Log("Found TargetArea at: " + path);
                return;
            }
        }
        
        Debug.LogError("TargetArea not found! Please create a GameObject named 'TargetArea' under GamingUI or assign it in Inspector.");
    }
    
    private void LoadTargetsFromConfig()
    {
        int currentLevel = SaveManager.Instance.PlayerData.currentLevel;
        LevelConfig config = LevelConfig.Load(currentLevel);
        if (config.target != null && config.target.Count > 0)
        {
            foreach (LevelConfig.TargetConfig tc in config.target)
            {
                levelTargets.Add(new TargetData(tc.type, tc.amount));
            }
        }
        else
        {
            CreateDefaultTargets();
        }
    }

    private void CreateDefaultTargets()
    {
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
        if (GamingUI.Instance != null)
        {
            GamingUI.Instance.OnAllTargetsCompleted();
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