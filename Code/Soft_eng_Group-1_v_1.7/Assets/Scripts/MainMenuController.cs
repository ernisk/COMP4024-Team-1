using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    // When the Play button is pressed, load the "Levels" scene.
    public void PlayGame()
    {
        SceneManager.LoadScene("Levels");
    }

    // When the Back button is pressed, return to main menu
    public void BackButton()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    // When the Help button is pressed, load the "Help_Menu" scene
    public void HelpButton()
    {
        SceneManager.LoadScene("Help_Menu");
    }

    // When the Quit button is pressed, close the game.
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
