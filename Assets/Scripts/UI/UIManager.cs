using System.Collections.Generic;
using UnityEngine;

namespace MMMatch.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [Header("Settings")]
        [SerializeField] private Transform uiRoot;
        
        private Dictionary<string, UIPanelBase> panelCache = new Dictionary<string, UIPanelBase>();
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            // 如果没有设置 UIRoot，自动查找
            if (uiRoot == null)
            {
                uiRoot = GameObject.Find("UIRoot")?.transform;
                if (uiRoot == null)
                {
                    GameObject uiRootObj = new GameObject("UIRoot");
                    uiRoot = uiRootObj.transform;
                    Debug.LogWarning("UIRoot not found, created automatically.");
                }
            }
        }
        
        public T GetPanel<T>(bool createIfNotExist = true) where T : UIPanelBase
        {
            string panelName = typeof(T).Name;
            
            // 如果缓存中已有，直接返回
            if (panelCache.ContainsKey(panelName))
            {
                return panelCache[panelName] as T;
            }
            
            if (!createIfNotExist)
                return null;
            
            // 尝试按名称查找场景中已存在的面板对象
            GameObject existingObj = GameObject.Find(panelName);
            if (existingObj == null)
            {
                // 尝试在 UIRoot 下查找
                existingObj = uiRoot?.Find(panelName)?.gameObject;
            }
            
            if (existingObj != null)
            {
                // 获取或添加组件
                T existingPanel = existingObj.GetComponent<T>();
                if (existingPanel == null)
                {
                    existingPanel = existingObj.AddComponent<T>();
                    Debug.LogWarning("Added " + panelName + " component to existing GameObject.");
                }
                
                panelCache.Add(panelName, existingPanel);
                Debug.Log("Found existing panel: " + panelName);
                return existingPanel;
            }
            
            // 尝试按类型查找
            T typePanel = FindObjectOfType<T>();
            if (typePanel != null)
            {
                panelCache.Add(panelName, typePanel);
                return typePanel;
            }
            
            Debug.LogWarning("Creating new panel: " + panelName);
            
            // 创建新面板
            GameObject panelObj = new GameObject(panelName);
            panelObj.transform.SetParent(uiRoot);
            
            // 设置 RectTransform
            RectTransform rect = panelObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            
            // 添加面板组件
            T panel = panelObj.AddComponent<T>();
            panel.Hide();
            
            panelCache.Add(panelName, panel);
            return panel;
        }
        
        public void ShowPanel<T>() where T : UIPanelBase
        {
            T panel = GetPanel<T>();
            if (panel != null)
            {
                panel.Show();
            }
        }
        
        public void HidePanel<T>() where T : UIPanelBase
        {
            string panelName = typeof(T).Name;
            if (panelCache.ContainsKey(panelName))
            {
                panelCache[panelName].Hide();
            }
        }
        
        public bool IsPanelVisible<T>() where T : UIPanelBase
        {
            string panelName = typeof(T).Name;
            if (panelCache.ContainsKey(panelName))
            {
                return panelCache[panelName].gameObject.activeSelf;
            }
            return false;
        }
        
        public void TogglePanel<T>() where T : UIPanelBase
        {
            T panel = GetPanel<T>();
            if (panel != null)
            {
                if (panel.gameObject.activeSelf)
                {
                    panel.Hide();
                }
                else
                {
                    panel.Show();
                }
            }
        }
        
        public void DestroyPanel<T>() where T : UIPanelBase
        {
            string panelName = typeof(T).Name;
            if (panelCache.ContainsKey(panelName))
            {
                Destroy(panelCache[panelName].gameObject);
                panelCache.Remove(panelName);
            }
        }
        
        public void HideAllPanels()
        {
            foreach (var panel in panelCache.Values)
            {
                panel.Hide();
            }
        }
        
        public Transform GetUIRoot()
        {
            return uiRoot;
        }
    }
}