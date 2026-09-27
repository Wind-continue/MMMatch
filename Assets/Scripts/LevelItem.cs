using UnityEngine;
using UnityEngine.UI;
using MMMatch.UI;

public class LevelItem : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Text levelText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Button itemButton;
    [SerializeField] private GameObject lockIcon;

    private int levelIndex;
    private bool isUnlocked;

    void Awake()
    {
        if (backgroundImage == null)
        {
            Transform bgTrans = transform.Find("Background");
            if (bgTrans != null)
                backgroundImage = bgTrans.GetComponent<Image>();
        }

        if (levelText == null)
        {
            Transform ltTrans = transform.Find("LevelText");
            if (ltTrans != null)
                levelText = ltTrans.GetComponent<Text>();
        }

        if (scoreText == null)
        {
            Transform stTrans = transform.Find("ScoreText");
            if (stTrans != null)
                scoreText = stTrans.GetComponent<Text>();
        }

        if (itemButton == null)
        {
            itemButton = GetComponent<Button>();
            if (itemButton == null)
                itemButton = gameObject.AddComponent<Button>();
        }

        if (lockIcon == null)
        {
            Transform liTrans = transform.Find("LockIcon");
            if (liTrans != null)
                lockIcon = liTrans.gameObject;
        }
    }

    public void Setup(int level, bool unlocked, int score)
    {
        levelIndex = level;
        isUnlocked = unlocked;

        if (levelText != null)
            levelText.text = level.ToString();

        if (scoreText != null)
        {
            if (unlocked && score > 0)
                scoreText.text = score.ToString();
            else
                scoreText.text = "";
        }

        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);

        if (itemButton != null)
        {
            itemButton.interactable = unlocked;
            itemButton.onClick.RemoveAllListeners();
            if (unlocked)
            {
                itemButton.onClick.AddListener(OnLevelClicked);
            }
        }

        ApplyVisualState(unlocked);
    }

    private void ApplyVisualState(bool unlocked)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.6f);
        }

        if (levelText != null)
        {
            levelText.color = unlocked ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }
    }

    private void OnLevelClicked()
    {
        if (!isUnlocked) return;

        SaveManager.Instance.SetCurrentLevel(levelIndex);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClosePanel<LevelChooseUI>();
            UIManager.Instance.Open<GamingUI>();
        }
    }
}