using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class AutoLoadMainMenu : MonoBehaviour
{
    private const string MAIN_MENU_SCENE = "MainMenu";
    
    // 在游戏启动时自动调用，无需挂载到对象上
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        Debug.Log("AutoLoadMainMenu: Initializing...");
        
        // 获取当前场景名称
        string currentScene = SceneManager.GetActiveScene().name;
        
        // 如果不在主菜单，直接加载主菜单
        if (currentScene != MAIN_MENU_SCENE)
        {
            Debug.LogWarning("AutoLoadMainMenu: Current scene is " + currentScene + ", loading " + MAIN_MENU_SCENE);
            SceneManager.LoadScene(MAIN_MENU_SCENE);
        }
        else
        {
            Debug.Log("AutoLoadMainMenu: Already in main menu.");
        }
    }
}