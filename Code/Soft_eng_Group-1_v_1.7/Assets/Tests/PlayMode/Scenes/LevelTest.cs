using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class LevelTest
{
    private GameObject _levelsObject;
    private Levels _levelsController;

    [SetUp]
    public void Setup()
    {
        _levelsObject = new GameObject("Levels");
        _levelsController = _levelsObject.AddComponent<Levels>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_levelsObject != null)
            Object.Destroy(_levelsObject);
    }

    [UnityTest]
    public IEnumerator LoadLevel1_LoadsCorrectScene()
    {
        _levelsController.LoadLevel1();
        yield return null;
    }

    [UnityTest]
    public IEnumerator LoadLevel2_LoadsCorrectScene()
    {
        _levelsController.LoadLevel2();
        yield return null;
    }

    [UnityTest]
    public IEnumerator LoadLevel3_LoadsCorrectScene()
    {
        _levelsController.LoadLevel3();
        yield return null;
    }
}
