using UnityEngine;

namespace MMMatch.UI
{
    public abstract class UIPanelBase : MonoBehaviour
    {
        public virtual void OnOpen() { }
        public virtual void OnClose() { }

        public void Close()
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ClosePanel(this);
        }
    }
}