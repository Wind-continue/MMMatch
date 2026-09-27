using UnityEngine;
using UnityEngine.UI;

namespace MMMatch.UI
{
    public class GameResultUI : UIPanelBase
    {
        [Header("Common")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text coinsText;

        [Header("Buttons")]
        [SerializeField] private Button nextLevelBtn;
        [SerializeField] private Button tryAgainBtn;
        [SerializeField] private Button levelsBtn;
        [SerializeField] private Button homeBtn;

        private int currentLevel;
        private bool isWin;
        private int totalLevels;

        public override void OnOpen()
        {
            ResolveReferences();
        }

        public override void OnClose()
        {
            Time.timeScale = 1f;
        }

        private void ResolveReferences()
        {
            if (titleText == null)
            {
                Transform t = transform.Find("TitleText");
                if (t != null) titleText = t.GetComponent<Text>();
            }

            if (levelText == null)
            {
                Transform t = transform.Find("LevelText");
                if (t != null) levelText = t.GetComponent<Text>();
            }

            if (scoreText == null)
            {
                Transform t = transform.Find("ScoreText");
                if (t != null) scoreText = t.GetComponent<Text>();
            }

            if (coinsText == null)
            {
                Transform t = transform.Find("CoinsText");
                if (t != null) coinsText = t.GetComponent<Text>();
            }

            if (nextLevelBtn == null)
            {
                Transform t = transform.Find("NextLevelBtn");
                if (t != null) nextLevelBtn = t.GetComponent<Button>();
            }

            if (tryAgainBtn == null)
            {
                Transform t = transform.Find("TryAgainBtn");
                if (t == null) t = transform.Find("AgainBtn");
                if (t != null) tryAgainBtn = t.GetComponent<Button>();
            }

            if (levelsBtn == null)
            {
                Transform t = transform.Find("LevelsBtn");
                if (t == null) t = transform.Find("LevelBtn");
                if (t != null) levelsBtn = t.GetComponent<Button>();
            }

            if (homeBtn == null)
            {
                Transform t = transform.Find("HomeBtn");
                if (t != null) homeBtn = t.GetComponent<Button>();
            }
        }

        public void SetupWin(int level, int score, int coins, int totalLevelCount)
        {
            isWin = true;
            currentLevel = level;
            totalLevels = totalLevelCount;

            ResolveReferences();

            if (titleText != null)
                titleText.text = "Level Clear!";

            if (levelText != null)
                levelText.text = "Level " + level;

            if (scoreText != null)
            {
                scoreText.gameObject.SetActive(true);
                scoreText.text = score.ToString();
            }

            if (coinsText != null)
            {
                coinsText.gameObject.SetActive(true);
                coinsText.text = "+" + coins;
            }

            bool isLastLevel = level >= totalLevelCount;

            if (nextLevelBtn != null)
            {
                nextLevelBtn.gameObject.SetActive(!isLastLevel);
                nextLevelBtn.onClick.RemoveAllListeners();
                if (!isLastLevel)
                    nextLevelBtn.onClick.AddListener(OnNextLevelClicked);
            }

            if (tryAgainBtn != null)
                tryAgainBtn.gameObject.SetActive(false);

            BindCommonButtons();

            Time.timeScale = 0f;
        }

        public void SetupFail(int level)
        {
            isWin = false;
            currentLevel = level;

            ResolveReferences();

            if (titleText != null)
                titleText.text = "Game Over";

            if (levelText != null)
                levelText.text = "Level " + level;

            if (scoreText != null)
                scoreText.gameObject.SetActive(false);

            if (coinsText != null)
                coinsText.gameObject.SetActive(false);

            if (nextLevelBtn != null)
                nextLevelBtn.gameObject.SetActive(false);

            if (tryAgainBtn != null)
            {
                tryAgainBtn.gameObject.SetActive(true);
                tryAgainBtn.onClick.RemoveAllListeners();
                tryAgainBtn.onClick.AddListener(OnTryAgainClicked);
            }

            BindCommonButtons();

            Time.timeScale = 0f;
        }

        private void BindCommonButtons()
        {
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

        private void OnTryAgainClicked()
        {
            Close();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ClosePanel<GamingUI>();
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