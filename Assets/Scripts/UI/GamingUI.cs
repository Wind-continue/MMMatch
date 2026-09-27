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

        private const int TotalLevelCount = 30;
        private const float ResultShowDelay = 0.8f;
        private const float ProcessingTimeout = 5f;

        private int currentSteps;
        private bool isGameOver = false;
        private bool isLevelComplete = false;
        private bool isGameStarted = false;
        private bool isShowingResult = false;

        public static GamingUI Instance { get; private set; }

        public bool IsGameActive { get { return isGameStarted && !isGameOver && !isLevelComplete && !isShowingResult; } }

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
            isShowingResult = false;
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
            isShowingResult = false;
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
            if (isGameOver || isLevelComplete || isShowingResult) return;
            isGameOver = true;
            isShowingResult = true;

            int level = SaveManager.Instance.PlayerData.currentLevel;
            int score = SaveManager.Instance.GetLevelScore(level);
            SaveManager.Instance.SaveLevelProgress(level, score, false);

            StartCoroutine(ShowResultCoroutine(level, false, 0, 0));
        }

        public void OnAllTargetsCompleted()
        {
            if (isGameOver || isLevelComplete || isShowingResult) return;
            isLevelComplete = true;
            isShowingResult = true;

            int level = SaveManager.Instance.PlayerData.currentLevel;
            int remainingSteps = currentSteps;
            int score = remainingSteps * 10;
            int coinsEarned = score;

            SaveManager.Instance.SaveLevelProgress(level, score, true);
            SaveManager.Instance.UnlockNextLevel();
            SaveManager.Instance.AddCoins(coinsEarned);

            StartCoroutine(ShowResultCoroutine(level, true, score, coinsEarned));
        }

        private IEnumerator ShowResultCoroutine(int level, bool win, int score, int coins)
        {
            yield return new WaitForSecondsRealtime(ResultShowDelay);

            float elapsed = 0f;
            while (elapsed < ProcessingTimeout)
            {
                ItemController ic = FindObjectOfType<ItemController>();
                if (ic == null || !ic.IsProcessing)
                    break;

                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }

            if (elapsed >= ProcessingTimeout)
                Debug.LogWarning("GamingUI: Timed out waiting for ItemController.IsProcessing, showing result anyway.");

            if (UIManager.Instance != null)
            {
                GameResultUI resultUI = UIManager.Instance.Open<GameResultUI>();
                if (resultUI != null)
                {
                    if (win)
                        resultUI.SetupWin(level, score, coins, TotalLevelCount);
                    else
                        resultUI.SetupFail(level);
                }
                else
                {
                    Debug.LogError("GamingUI: Failed to open GameResultUI!");
                }
            }
            else
            {
                Debug.LogError("GamingUI: UIManager.Instance is null!");
            }
        }

        public int GetCurrentSteps()
        {
            return currentSteps;
        }
    }
}