using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PopupOnTriggerPNGTests
{
    [UnityTest]
    public IEnumerator PopupAppearsWhenPlayerEntersTrigger()
    {
        // Create a canvas for the popup (since the script expects to find one)
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create player object
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<BoxCollider2D>();

        // Create popup trigger object
        GameObject popupTrigger = new GameObject("PopupTrigger");
        BoxCollider2D triggerCollider = popupTrigger.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;

        PopupOnTriggerPNG popupScript = popupTrigger.AddComponent<PopupOnTriggerPNG>();

        // Mock a simple texture (so it doesn't error trying to show one)
        popupScript.popupTexture = new Texture2D(100, 100);
        popupScript.popupScale = Vector2.one;

        // Manually call Start() to initialize the canvas check
        popupScript.SendMessage("Start");

        // Simulate player entering the trigger
        popupScript.SendMessage("OnTriggerEnter2D", player.GetComponent<Collider2D>());

        // Wait one frame for Unity to process the trigger and popup creation
        yield return null;

        // Check if popup was created
        GameObject popupImage = GameObject.Find("PopupImage");
        Assert.IsNotNull(popupImage, "Popup did not appear when player entered trigger!");

        // Cleanup
        Object.Destroy(player);
        Object.Destroy(popupTrigger);
        Object.Destroy(canvasObj);
        if (popupImage) Object.Destroy(popupImage);
    }
}
