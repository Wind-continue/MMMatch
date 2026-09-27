using System.Collections.Generic;
using UnityEngine;

namespace MMMatch.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private Transform uiRoot;
        [SerializeField] private List<GameObject> panelPrefabs = new List<GameObject>();

        private Dictionary<string, GameObject> templates = new Dictionary<string, GameObject>();
        private List<UIPanelBase> panelStack = new List<UIPanelBase>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (uiRoot == null) uiRoot = transform;

            CacheTemplates();
        }

        void Start()
        {
            Open<EnterGameUI>();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void CacheTemplates()
        {
            templates.Clear();

            foreach (GameObject prefab in panelPrefabs)
            {
                if (prefab == null) continue;
                UIPanelBase panel = prefab.GetComponent<UIPanelBase>();
                if (panel != null)
                {
                    string key = panel.GetType().Name;
                    if (!templates.ContainsKey(key))
                        templates[key] = prefab;
                }
                else
                {
                    Debug.LogWarning("UIManager: Prefab '" + prefab.name + "' has no UIPanelBase component, skipping.");
                }
            }

            UIPanelBase[] scenePanels = uiRoot.GetComponentsInChildren<UIPanelBase>(true);
            foreach (var panel in scenePanels)
            {
                string key = panel.GetType().Name;
                if (!templates.ContainsKey(key))
                    templates[key] = panel.gameObject;
                panel.gameObject.SetActive(false);
            }
        }

        public T Open<T>() where T : UIPanelBase
        {
            string key = typeof(T).Name;
            T panel = null;
            GameObject obj = null;

            if (templates.ContainsKey(key) && templates[key] != null)
            {
                obj = Instantiate(templates[key], uiRoot);
                obj.name = key;
                panel = obj.GetComponent<T>();

                if (panel == null)
                {
                    Debug.LogWarning("UIManager: Prefab for '" + key + "' instantiated but GetComponent<" + key + "> returned null. Adding component dynamically.");
                    panel = obj.AddComponent<T>();
                }
            }

            if (panel == null)
            {
                Debug.LogWarning("UIManager: No template found for '" + key + "', creating empty panel.");
                obj = new GameObject(key);
                obj.transform.SetParent(uiRoot, false);
                RectTransform rect = obj.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.localScale = Vector3.one;
                panel = obj.AddComponent<T>();
            }

            panel.transform.SetAsLastSibling();
            panel.gameObject.SetActive(true);
            panel.OnOpen();
            panelStack.Add(panel);

            return panel;
        }

        public void ClosePanel(UIPanelBase panel)
        {
            if (panel == null) return;

            int index = panelStack.IndexOf(panel);
            if (index < 0) return;

            panel.OnClose();
            panelStack.RemoveAt(index);
            Destroy(panel.gameObject);
        }

        public void ClosePanel<T>() where T : UIPanelBase
        {
            for (int i = panelStack.Count - 1; i >= 0; i--)
            {
                if (panelStack[i] is T)
                {
                    ClosePanel(panelStack[i]);
                    return;
                }
            }
        }

        public void CloseAllPanels()
        {
            for (int i = panelStack.Count - 1; i >= 0; i--)
            {
                if (panelStack[i] != null)
                {
                    panelStack[i].OnClose();
                    Destroy(panelStack[i].gameObject);
                }
            }
            panelStack.Clear();
        }

        public T FindPanel<T>() where T : UIPanelBase
        {
            for (int i = panelStack.Count - 1; i >= 0; i--)
            {
                if (panelStack[i] is T t)
                    return t;
            }
            return null;
        }
    }
}