using UnityEngine;
using UnityEngine.UI;

namespace MMMatch.UI
{
    public class EnterGameUI : UIPanelBase
    {
        [SerializeField] private Button playBtn;

        public override void OnOpen()
        {
            if (playBtn == null)
                playBtn = GetComponentInChildren<Button>();

            if (playBtn != null)
            {
                playBtn.onClick.RemoveAllListeners();
                playBtn.onClick.AddListener(OnPlayClicked);
            }
        }

        public override void OnClose()
        {
        }

        private void OnPlayClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.Open<LevelChooseUI>();
            }
        }
    }
}