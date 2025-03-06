using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController2DTests
{
    private GameObject playerObject;
    private PlayerController2D playerController;
    private Rigidbody2D rb;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("Player");
        playerController = playerObject.AddComponent<PlayerController2D>();
        rb = playerObject.AddComponent<Rigidbody2D>();

        // Inject Rigidbody into the PlayerController2D
        playerController.GetType().GetField("rb", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(playerController, rb);

        playerController.moveSpeed = 5f;
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(playerObject);
    }

    [Test]
    public void PlayerStartsWithZeroCollectedCount()
    {
        // We can't directly access private variables, so this is more of a placeholder test if you expose "collectedCount" in the future
        Assert.NotNull(playerController);
    }

    [Test]
    public void PlayerHasRigidbody()
    {
        Assert.NotNull(playerObject.GetComponent<Rigidbody2D>());
    }


    [UnityTest]
    public IEnumerator PlayerReachesFinish_LevelSceneLoads()
    {
        // Setup scene - load a fake level (mock if needed, or use real test scene)
        SceneManager.LoadScene("Level 1"); // You need to create this test scene or mock this if not available

        yield return new WaitForSeconds(0.1f); // Wait to ensure scene loads

        GameObject finishObject = new GameObject("Finish");
        finishObject.tag = "Finish";

        // Simulate trigger enter
        playerController.GetType().GetMethod("OnTriggerEnter2D", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(playerController, new object[] { finishObject.AddComponent<BoxCollider2D>() });

        yield return new WaitForSeconds(0.1f); // Allow time for scene load (if real scene transition)

        // Verify next scene (depending on level you load)
        Assert.AreEqual("Finish_Level", SceneManager.GetActiveScene().name); // This will only work if you create mock/test scenes.
    }

    [UnityTest]
    public IEnumerator PlayerReachesFinish_Level3_LoadsFinishGameScene()
    {
        SceneManager.LoadScene("Level 3"); // Must exist in the build settings or be mocked

        yield return new WaitForSeconds(0.1f);

        GameObject finishObject = new GameObject("Finish");
        finishObject.tag = "Finish";

        // Simulate trigger enter
        playerController.GetType().GetMethod("OnTriggerEnter2D", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(playerController, new object[] { finishObject.AddComponent<BoxCollider2D>() });

        yield return new WaitForSeconds(0.1f); // Wait for scene to potentially change

        Assert.AreEqual("Finish_Game", SceneManager.GetActiveScene().name); // This only works with actual scenes set up
    }
}
