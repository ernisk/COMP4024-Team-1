using UnityEditor;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : UnityEngine.MonoBehaviour
{
    // Play button running level
    public void PlayGame()
    {
        SceneManager.LoadScene("Level_1");
    }
    
    // Quit button closing application
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}