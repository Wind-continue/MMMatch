using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MMMatch.UI
{
    public class LevelChooseUI : UIPanelBase
    {
        [Header("UI References")]
        [SerializeField] private Transform container;
        [SerializeField] private LevelItem levelItemPrefab;
        [SerializeField] private Button backBtn;
        [SerializeField] private Text pageText;
        [SerializeField] private Button lastPageBtn;
        [SerializeField] private Button nextPageBtn;

        [Header("Settings")]
        [SerializeField] private int totalLevels = 30;
        [SerializeField] private int levelsPerPage = 15;

        private List<LevelItem> levelItems = new List<LevelItem>();
        private int currentPage = 1;
        private int totalPages = 1;
        private bool isInitialized = false;

        public override void OnOpen()
        {
            Initialize();
            GenerateLevelItems();
        }

        public override void OnClose()
        {
            foreach (LevelItem item in levelItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            levelItems.Clear();
            isInitialized = false;
        }

        private void Initialize()
        {
            if (isInitialized) return;

            EnsureSelfBuilt();
            FindContainerIfNeeded();
            FindPrefabIfNeeded();
            totalPages = Mathf.CeilToInt((float)totalLevels / levelsPerPage);
            if (totalPages < 1) totalPages = 1;
            currentPage = 1;

            if (backBtn != null)
                backBtn.onClick.AddListener(OnBackClicked);
            if (lastPageBtn != null)
                lastPageBtn.onClick.AddListener(OnLastPageClicked);
            if (nextPageBtn != null)
                nextPageBtn.onClick.AddListener(OnNextPageClicked);

            isInitialized = true;
        }

        private void EnsureSelfBuilt()
        {
            if (container != null && levelItemPrefab != null && backBtn != null)
                return;

            if (transform.childCount > 0)
                return;

            BuildUI();
        }

        private void BuildUI()
        {
            RectTransform rootRect = EnsureRectTransform(gameObject);
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            GameObject bgObj = CreateChild("Background", transform);
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            GameObject titleObj = CreateChild("Title", transform);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "Select Level";
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.sizeDelta = new Vector2(400f, 60f);
            titleRect.anchoredPosition = new Vector2(0f, -20f);

            GameObject scrollObj = CreateChild("ScrollView", transform);
            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0f, 0.2f);
            scrollRect.anchorMax = new Vector2(1f, 0.85f);
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;

            GameObject viewportObj = CreateChild("Viewport", scrollObj.transform);
            RectTransform vpRect = viewportObj.GetComponent<RectTransform>();
            vpRect.anchorMin = Vector2.zero;
            vpRect.anchorMax = Vector2.one;
            vpRect.offsetMin = Vector2.zero;
            vpRect.offsetMax = Vector2.zero;
            Image vpMask = viewportObj.AddComponent<Image>();
            vpMask.color = Color.clear;
            viewportObj.AddComponent<Mask>().showMaskGraphic = false;

            GameObject contentObj = CreateChild("Content", viewportObj.transform);
            RectTransform contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(0f, 800f);
            contentRect.anchoredPosition = Vector2.zero;

            GridLayoutGroup grid = contentObj.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(140f, 140f);
            grid.spacing = new Vector2(20f, 20f);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;

            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = vpRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;

            container = contentObj.transform;

            levelItemPrefab = BuildLevelItemPrefab(contentObj.transform);

            GameObject pageBarObj = CreateChild("PageBar", transform);
            RectTransform pageBarRect = pageBarObj.GetComponent<RectTransform>();
            pageBarRect.anchorMin = new Vector2(0.5f, 0.05f);
            pageBarRect.anchorMax = new Vector2(0.5f, 0.05f);
            pageBarRect.pivot = new Vector2(0.5f, 0.5f);
            pageBarRect.sizeDelta = new Vector2(400f, 50f);
            pageBarRect.anchoredPosition = Vector2.zero;

            lastPageBtn = BuildButton("LastPageBtn", pageBarObj.transform, "<", new Vector2(-150f, 0f));
            pageText = BuildText("PageText", pageBarObj.transform, "1/2", new Vector2(0f, 0f));
            nextPageBtn = BuildButton("NextPageBtn", pageBarObj.transform, ">", new Vector2(150f, 0f));

            backBtn = BuildButton("BackBtn", transform, "Back", new Vector2(0f, 0f));
            RectTransform backRect = backBtn.gameObject.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.5f, 0f);
            backRect.anchorMax = new Vector2(0.5f, 0f);
            backRect.pivot = new Vector2(0.5f, 0.5f);
            backRect.sizeDelta = new Vector2(200f, 50f);
            backRect.anchoredPosition = new Vector2(0f, 30f);
        }

        private LevelItem BuildLevelItemPrefab(Transform parent)
        {
            GameObject itemObj = new GameObject("LevelItemTemplate");
            itemObj.transform.SetParent(parent, false);
            RectTransform itemRect = itemObj.AddComponent<RectTransform>();
            itemRect.localScale = Vector3.one;

            Image itemBg = itemObj.AddComponent<Image>();
            itemBg.color = new Color(0.25f, 0.25f, 0.35f, 1f);

            LevelItem levelItem = itemObj.AddComponent<LevelItem>();

            GameObject bgChild = CreateChild("Background", itemObj.transform);
            Image bgImg = bgChild.AddComponent<Image>();
            bgImg.color = new Color(0.25f, 0.25f, 0.35f, 1f);
            RectTransform bgRect = bgChild.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            GameObject levelTextObj = CreateChild("LevelText", itemObj.transform);
            Text lt = levelTextObj.AddComponent<Text>();
            lt.text = "1";
            lt.fontSize = 28;
            lt.color = Color.white;
            lt.alignment = TextAnchor.MiddleCenter;
            lt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            RectTransform ltRect = levelTextObj.GetComponent<RectTransform>();
            ltRect.anchorMin = new Vector2(0f, 0.5f);
            ltRect.anchorMax = new Vector2(1f, 0.8f);
            ltRect.offsetMin = Vector2.zero;
            ltRect.offsetMax = Vector2.zero;

            GameObject scoreTextObj = CreateChild("ScoreText", itemObj.transform);
            Text st = scoreTextObj.AddComponent<Text>();
            st.text = "";
            st.fontSize = 18;
            st.color = new Color(1f, 0.85f, 0f);
            st.alignment = TextAnchor.MiddleCenter;
            st.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            RectTransform stRect = scoreTextObj.GetComponent<RectTransform>();
            stRect.anchorMin = new Vector2(0f, 0.15f);
            stRect.anchorMax = new Vector2(1f, 0.45f);
            stRect.offsetMin = Vector2.zero;
            stRect.offsetMax = Vector2.zero;

            GameObject lockObj = CreateChild("LockIcon", itemObj.transform);
            Image lockImg = lockObj.AddComponent<Image>();
            lockImg.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
            RectTransform lockRect = lockObj.GetComponent<RectTransform>();
            lockRect.anchorMin = new Vector2(0.3f, 0.3f);
            lockRect.anchorMax = new Vector2(0.7f, 0.7f);
            lockRect.offsetMin = Vector2.zero;
            lockRect.offsetMax = Vector2.zero;
            lockObj.SetActive(false);

            itemObj.SetActive(false);

            return levelItem;
        }

        private GameObject CreateChild(string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();
            return obj;
        }

        private RectTransform EnsureRectTransform(GameObject obj)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect == null)
                rect = obj.AddComponent<RectTransform>();
            return rect;
        }

        private Button BuildButton(string name, Transform parent, string text, Vector2 anchoredPos)
        {
            GameObject btnObj = CreateChild(name, parent);
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.3f, 0.3f, 0.45f, 1f);
            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = btnImg;

            GameObject txtObj = CreateChild("Text", btnObj.transform);
            Text t = txtObj.AddComponent<Text>();
            t.text = text;
            t.fontSize = 24;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            RectTransform tRect = txtObj.GetComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.offsetMin = Vector2.zero;
            tRect.offsetMax = Vector2.zero;

            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(80f, 45f);
            btnRect.anchoredPosition = anchoredPos;

            return btn;
        }

        private Text BuildText(string name, Transform parent, string text, Vector2 anchoredPos)
        {
            GameObject txtObj = CreateChild(name, parent);
            Text t = txtObj.AddComponent<Text>();
            t.text = text;
            t.fontSize = 22;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            RectTransform tRect = txtObj.GetComponent<RectTransform>();
            tRect.sizeDelta = new Vector2(120f, 40f);
            tRect.anchoredPosition = anchoredPos;
            return t;
        }

        private void FindContainerIfNeeded()
        {
            if (container != null) return;

            Transform found = transform.Find("ScrollView/Viewport/Content");
            if (found != null) { container = found; return; }

            found = transform.Find("Content");
            if (found != null) { container = found; return; }

            found = transform.Find("Container");
            if (found != null) { container = found; return; }
        }

        private void FindPrefabIfNeeded()
        {
            if (levelItemPrefab != null) return;

            levelItemPrefab = GetComponentInChildren<LevelItem>(true);
        }

        private void GenerateLevelItems()
        {
            if (container == null || levelItemPrefab == null) return;

            foreach (LevelItem item in levelItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            levelItems.Clear();

            int maxUnlocked = SaveManager.Instance.PlayerData.maxUnlockedLevel;
            int startLevel = (currentPage - 1) * levelsPerPage + 1;
            int endLevel = Mathf.Min(currentPage * levelsPerPage, totalLevels);

            for (int level = startLevel; level <= endLevel; level++)
            {
                LevelItem item = Instantiate(levelItemPrefab, container);
                item.name = "LevelItem_" + level;
                item.gameObject.SetActive(true);

                RectTransform rect = item.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.SetParent(container, false);
                    rect.localScale = Vector3.one;
                }

                bool unlocked = level <= maxUnlocked;
                int score = SaveManager.Instance.GetLevelScore(level);
                item.Setup(level, unlocked, score);

                levelItems.Add(item);
            }

            UpdatePageUI();
        }

        private void UpdatePageUI()
        {
            if (pageText != null)
                pageText.text = currentPage + "/" + totalPages;

            if (lastPageBtn != null)
                lastPageBtn.interactable = currentPage > 1;

            if (nextPageBtn != null)
                nextPageBtn.interactable = currentPage < totalPages;
        }

        private void OnLastPageClicked()
        {
            if (currentPage <= 1) return;
            currentPage--;
            GenerateLevelItems();
        }

        private void OnNextPageClicked()
        {
            if (currentPage >= totalPages) return;
            currentPage++;
            GenerateLevelItems();
        }

        private void OnBackClicked()
        {
            Close();
        }
    }
}