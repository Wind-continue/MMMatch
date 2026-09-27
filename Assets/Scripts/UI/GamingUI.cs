using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MMMatch.UI
{
    public class GamingUI : UIPanelBase
    {
        [Header("UI References")]
        [SerializeField] private GameObject Title_Style1;
        [SerializeField] private GameObject Title_Style2;
        [SerializeField] private Text Step_Text;

        [Header("Game Settings")]
        public int totalSteps = 10;

        private int currentSteps;
        private bool isGameOver = false;
        private bool isLevelComplete = false;
        private bool isGameStarted = false;
        private bool pendingWin = false;
        private int pendingWinLevel;
        private int pendingWinScore;
        private int pendingWinCoins;

        public static GamingUI Instance { get; private set; }

        public bool IsGameActive { get { return isGameStarted && !isGameOver && !isLevelComplete; } }

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
                Instance = null;
        }

        void Update()
        {
            if (!isGameStarted) return;

            if (pendingWin)
            {
                ItemController ic = GetComponentInChildren<ItemController>();
                if (ic == null) ic = FindObjectOfType<ItemController>();
                if (ic == null || !ic.IsProcessing)
                {
                    pendingWin = false;
                    ShowWinUI(pendingWinLevel, pendingWinScore, pendingWinCoins);
                }
            }
        }

        public override void OnOpen()
        {
            ResolveReferences();
            StartGame();
        }

        public override void OnClose()
        {
            Time.timeScale = 1f;
            isGameStarted = false;
            isGameOver = false;
            isLevelComplete = false;
            pendingWin = false;
        }

        private void ResolveReferences()
        {
            if (Title_Style1 == null)
            {
                Transform t = transform.Find("Title_Style1");
                if (t != null) Title_Style1 = t.gameObject;
            }

            if (Title_Style2 == null)
            {
                Transform t = transform.Find("Title_Style2");
                if (t != null) Title_Style2 = t.gameObject;
            }

            if (Step_Text == null && Title_Style2 != null)
            {
                Transform st = Title_Style2.transform.Find("StepArea/Step_Text");
                if (st != null) Step_Text = st.GetComponent<Text>();
            }

            if (Step_Text == null)
            {
                Transform st = transform.Find("Title_Style2/StepArea/Step_Text");
                if (st != null) Step_Text = st.GetComponent<Text>();
            }
        }

        private void StartGame()
        {
            int currentLevel = SaveManager.Instance.PlayerData.currentLevel;
            LevelConfig config = LevelConfig.Load(currentLevel);
            totalSteps = config.step;
            currentSteps = totalSteps;

            if (Title_Style1 != null)
                Title_Style1.SetActive(false);
            if (Title_Style2 != null)
                Title_Style2.SetActive(true);

            if (Step_Text != null)
                Step_Text.text = currentSteps.ToString();
            else
                Debug.LogWarning("GamingUI: Step_Text not found!");

            isGameStarted = true;
            isGameOver = false;
            isLevelComplete = false;
            pendingWin = false;
        }

        public void OnSuccessfulMatch()
        {
            if (!IsGameActive) return;

            currentSteps--;

            if (Step_Text != null)
                Step_Text.text = currentSteps.ToString();

            if (currentSteps <= 0)
                TriggerGameOver();
        }

        private void TriggerGameOver()
        {
            if (isGameOver || isLevelComplete) return;
            isGameOver = true;

            int level = SaveManager.Instance.PlayerData.currentLevel;
            int score = SaveManager.Instance.GetLevelScore(level);
            SaveManager.Instance.SaveLevelProgress(level, score, false);

            StartCoroutine(ShowGameOverUICoroutine(level));
        }

        public void OnAllTargetsCompleted()
        {
            if (isGameOver || isLevelComplete) return;
            isLevelComplete = true;

            int level = SaveManager.Instance.PlayerData.currentLevel;
            int remainingSteps = currentSteps;
            int score = remainingSteps * 10;
            int coinsEarned = score;

            SaveManager.Instance.SaveLevelProgress(level, score, true);
            SaveManager.Instance.UnlockNextLevel();
            SaveManager.Instance.AddCoins(coinsEarned);

            pendingWin = true;
            pendingWinLevel = level;
            pendingWinScore = score;
            pendingWinCoins = coinsEarned;
        }

        private void ShowWinUI(int level, int score, int coins)
        {
            if (UIManager.Instance != null)
            {
                LevelWinUI winUI = UIManager.Instance.Open<LevelWinUI>();
                if (winUI != null)
                    winUI.Setup(level, score, coins);
            }
        }

        private IEnumerator ShowGameOverUICoroutine(int level)
        {
            yield return new WaitForSecondsRealtime(0.5f);

            ItemController ic = FindObjectOfType<ItemController>();
            while (ic != null && ic.IsProcessing)
                yield return null;

            if (UIManager.Instance != null)
            {
                GameOverUI gameOverUI = UIManager.Instance.Open<GameOverUI>();
                if (gameOverUI != null)
                    gameOverUI.Setup(level);
            }
        }

        public int GetCurrentSteps()
        {
            return currentSteps;
        }
    }
}