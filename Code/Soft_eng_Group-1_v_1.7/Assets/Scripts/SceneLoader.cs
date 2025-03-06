using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Set the scene name you want to load in the Inspector
    public string sceneName;

    // This method can be assigned to the button's OnClick event
    public void LoadSceneOnClick()
    {
        SceneManager.LoadScene(sceneName);
    }
}
