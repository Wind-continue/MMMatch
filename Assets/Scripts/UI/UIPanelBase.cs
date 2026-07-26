using UnityEngine;

namespace MMMatch.UI
{
    public class UIPanelBase : MonoBehaviour
    {
        protected UIManager uiManager;
        
        protected virtual void Awake()
        {
            uiManager = UIManager.Instance;
        }
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public virtual void ShowWithAnimation()
        {
            Show();
        }
        
        public virtual void HideWithAnimation()
        {
            Hide();
        }
    }
}