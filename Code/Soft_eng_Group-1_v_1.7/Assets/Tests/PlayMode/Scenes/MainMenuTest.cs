using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class MainMenuTest
{
    private GameObject _mainMenuObject;
    private MainMenuController _mainMenuController;

    [SetUp]
    public void Setup()
    {
        _mainMenuObject = new GameObject("MainMenu");
        _mainMenuController = _mainMenuObject.AddComponent<MainMenuController>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_mainMenuObject != null)
            Object.Destroy(_mainMenuObject);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != SceneManager.GetActiveScene())
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    [Test]
    public void MainmenuSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    [UnityTest]
    public IEnumerator MainmenuWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayGame_LoadsLevelsScene()
    {
        _mainMenuController.PlayGame();
        yield return null;
        Assert.AreEqual("Levels", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator BackButton_LoadsMainMenuScene()
    {
        _mainMenuController.BackButton();
        yield return null;
        Assert.AreEqual("Main_Menu", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator HelpButton_LoadsHelpMenuScene()
    {
        _mainMenuController.HelpButton();
        yield return null;
        Assert.AreEqual("Help_Menu", SceneManager.GetActiveScene().name);
    }
}
