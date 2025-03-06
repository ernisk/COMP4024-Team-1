using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class KruskalMazeGeneratorTests
{
    private GameObject mazeObject;
    private KruskalMazeGenerator mazeGenerator;

    [SetUp]
    public void Setup()
    {
        mazeObject = new GameObject("MazeGenerator");
        mazeGenerator = mazeObject.AddComponent<KruskalMazeGenerator>();
        mazeGenerator.width = 5;
        mazeGenerator.height = 5;
    }

    [TearDown]
    public void Teardown()
    {
        if (mazeObject != null)
            Object.DestroyImmediate(mazeObject);
    }

    [Test]
    public void MazeGeneratorInstance_IsNotNull()
    {
        Assert.IsNotNull(mazeGenerator, "Maze Generator instance should not be null");
    }

    [Test]
    public void MazeDimensions_AreSetCorrectly()
    {
        Assert.AreEqual(5, mazeGenerator.width, "Width should be set correctly");
        Assert.AreEqual(5, mazeGenerator.height, "Height should be set correctly");
    }
}
