using UnityEngine;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour
{
    // Loads the "Level1" scene.
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level 1");
    }

    // Loads the "Level2" scene.
    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level 2");
    }

    // Loads the "Level3" scene.
    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level 3");
    }
}
