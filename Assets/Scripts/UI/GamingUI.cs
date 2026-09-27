using UnityEngine;

namespace MMMatch.UI
{
    public class GamingUI : UIPanelBase
    {
        public override void OnOpen()
        {
            GamingController gc = GetComponentInChildren<GamingController>();
            if (gc == null)
                gc = FindObjectOfType<GamingController>();

            if (gc != null)
                gc.StartGame();
        }

        public override void OnClose()
        {
            Time.timeScale = 1f;
        }
    }
}