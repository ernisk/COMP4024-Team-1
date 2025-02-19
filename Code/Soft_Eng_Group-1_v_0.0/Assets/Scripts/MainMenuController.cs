using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    // Play butonuna basıldığında Level_1 sahnesi yüklenir.
    public void PlayGame()
    {
        SceneManager.LoadScene("Level_1");
    }
    
    // Quit butonuna basıldığında oyun kapanır.
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}