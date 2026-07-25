using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayBtn : MonoBehaviour
{
    [SerializeField] private Button m_playBtn;
    private void Awake()
    {
        m_playBtn.onClick.AddListener(OnPlayBtnClicked);
    }

    private void OnPlayBtnClicked()
    {
        SceneManager.LoadScene("MainGame");
    }
}
