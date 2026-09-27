using UnityEngine;
using UnityEngine.UI;
using MMMatch.UI;

namespace MMMatch.UI
{
    public class LevelWinUI : UIPanelBase
    {
        [Header("UI References")]
        [SerializeField] private Text levelText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text coinsText;
        [SerializeField] private Button nextLevelBtn;
        [SerializeField] private Button levelsBtn;
        [SerializeField] private Button homeBtn;

        private int currentLevel;

        public override void OnOpen()
        {
            ResolveReferences();
            Time.timeScale = 0f;
        }

        public override void OnClose()
        {
            Time.timeScale = 1f;
        }

        private void ResolveReferences()
        {
            if (levelText == null)
            {
                Transform lt = transform.Find("LevelText");
                if (lt != null) levelText = lt.GetComponent<Text>();
            }

            if (scoreText == null)
            {
                Transform st = transform.Find("ScoreText");
                if (st != null) scoreText = st.GetComponent<Text>();
            }

            if (coinsText == null)
            {
                Transform ct = transform.Find("CoinsText");
                if (ct != null) coinsText = ct.GetComponent<Text>();
            }

            if (nextLevelBtn == null)
            {
                Transform nb = transform.Find("NextLevelBtn");
                if (nb != null) nextLevelBtn = nb.GetComponent<Button>();
            }

            if (levelsBtn == null)
            {
                Transform lb = transform.Find("LevelsBtn");
                if (lb != null) levelsBtn = lb.GetComponent<Button>();
            }

            if (homeBtn == null)
            {
                Transform hb = transform.Find("HomeBtn");
                if (hb != null) homeBtn = hb.GetComponent<Button>();
            }

            if (nextLevelBtn != null)
            {
                nextLevelBtn.onClick.RemoveAllListeners();
                nextLevelBtn.onClick.AddListener(OnNextLevelClicked);
            }

            if (levelsBtn != null)
            {
                levelsBtn.onClick.RemoveAllListeners();
                levelsBtn.onClick.AddListener(OnLevelsClicked);
            }

            if (homeBtn != null)
            {
                homeBtn.onClick.RemoveAllListeners();
                homeBtn.onClick.AddListener(OnHomeClicked);
            }
        }

        public void Setup(int level, int score, int coins)
        {
            currentLevel = level;

            if (levelText != null)
                levelText.text = "Level " + level;

            if (scoreText != null)
                scoreText.text = score.ToString();

            if (coinsText != null)
                coinsText.text = "+" + coins;
        }

        private void OnNextLevelClicked()
        {
            Close();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ClosePanel<GamingUI>();

                int nextLevel = currentLevel + 1;
                SaveManager.Instance.SetCurrentLevel(nextLevel);

                UIManager.Instance.Open<GamingUI>();
            }
        }

        private void OnLevelsClicked()
        {
            Close();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ClosePanel<GamingUI>();
                UIManager.Instance.Open<LevelChooseUI>();
            }
        }

        private void OnHomeClicked()
        {
            Close();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ClosePanel<GamingUI>();
            }
        }
    }
}