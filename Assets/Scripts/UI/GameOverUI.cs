using UnityEngine;
using UnityEngine.UI;

namespace MMMatch.UI
{
    public class GameOverUI : UIPanelBase
    {
        [Header("Button References")]
        [SerializeField] private Button returnBtn;
        [SerializeField] private Button homeBtn;
        [SerializeField] private Button againBtn;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 自动查找按钮
            if (returnBtn == null)
            {
                returnBtn = transform.Find("ReturnBtn")?.GetComponent<Button>();
            }
            
            if (homeBtn == null)
            {
                homeBtn = transform.Find("HomeBtn")?.GetComponent<Button>();
            }
            
            if (againBtn == null)
            {
                againBtn = transform.Find("AgainBtn")?.GetComponent<Button>();
            }
            
            // 添加按钮事件
            if (returnBtn != null)
            {
                returnBtn.onClick.AddListener(OnReturnClicked);
            }
            
            if (homeBtn != null)
            {
                homeBtn.onClick.AddListener(OnHomeClicked);
            }
            
            if (againBtn != null)
            {
                againBtn.onClick.AddListener(OnAgainClicked);
            }
        }
        
        private void OnReturnClicked()
        {
            Hide();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        private void OnHomeClicked()
        {
            Hide();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        private void OnAgainClicked()
        {
            Hide();
            // 重新开始游戏
            if (GamingController.Instance != null)
            {
                GamingController.Instance.ResetGame();
            }
        }
        
        public override void Show()
        {
            base.Show();
            // 显示时可以暂停游戏
            Time.timeScale = 0;
        }
        
        public override void Hide()
        {
            base.Hide();
            // 隐藏时恢复游戏
            Time.timeScale = 1;
        }
    }
}