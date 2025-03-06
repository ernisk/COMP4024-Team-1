using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupOnTriggerPNG : MonoBehaviour
{
    [Header("Popup Settings")]
    [Tooltip("Assign the PNG texture to be used for the popup")]
    public Texture2D popupTexture;
    
    [Tooltip("Scale factor for the popup size. 1 means native size.")]
    public Vector2 popupScale = Vector2.one;
    
    [Tooltip("Key to dismiss the popup and destroy this object if correct answer is given")]
    public KeyCode dismissKey = KeyCode.Space;
    
    [Tooltip("Time in seconds after which the popup auto-dismisses if no correct answer is given")]
    public float autoDismissTime = 7f;

    [Tooltip("Sorting Order for the popup texture, default is 3")]
    public int popupSortingOrder = 3;

    [Header("Dismiss Audio")]
    [Tooltip("Audio clip to play when the dismiss key is pressed")]
    public AudioClip dismissAudioClip;

    // Reference to the instantiated popup UI object
    private GameObject popupInstance;
    // Flag to track whether the popup is active
    private bool popupActive = false;
    // Reference to the UI canvas
    private Canvas canvas;
    // Reference to the auto-dismiss coroutine
    private Coroutine autoDismissCoroutine;

    private void Start()
    {
        // Try to find an existing Canvas; if none exists, create one.
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
    }

    // Trigger detection for 2D colliders
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!popupActive && other.CompareTag("Player"))
        {
            // Create a new GameObject for the popup UI
            popupInstance = new GameObject("PopupImage");
            popupInstance.transform.SetParent(canvas.transform, false);
            
            // Add a Canvas component to control the layer (sorting order)
            Canvas popupCanvas = popupInstance.AddComponent<Canvas>();
            popupCanvas.overrideSorting = true;
            popupCanvas.sortingOrder = popupSortingOrder;

            // Add an Image component to display the PNG texture
            Image image = popupInstance.AddComponent<Image>();

            if (popupTexture != null)
            {
                // Convert the Texture2D to a Sprite and assign it to the Image component
                Sprite sprite = Sprite.Create(popupTexture, new Rect(0, 0, popupTexture.width, popupTexture.height), new Vector2(0.5f, 0.5f));
                image.sprite = sprite;

                // Adjust the popup size using the scale factor
                RectTransform rectTransform = popupInstance.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(popupTexture.width, popupTexture.height) * popupScale;
                // Center the popup on the screen
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                Debug.LogWarning("Popup texture is not assigned!");
            }

            popupActive = true;
            // Start the auto-dismiss timer
            autoDismissCoroutine = StartCoroutine(AutoDismissPopup());
        }
    }

    private void Update()
    {
        if (popupActive)
        {
            // Check if the correct dismiss key is pressed.
            bool correctKeyPressed = Input.GetKeyDown(dismissKey);

            // If the dismiss key is a numpad key, also check its alpha equivalent.
            if (!correctKeyPressed && IsKeypadKey(dismissKey))
            {
                KeyCode alphaKey = GetAlphaEquivalent(dismissKey);
                correctKeyPressed = Input.GetKeyDown(alphaKey);
            }

            if (correctKeyPressed)
            {
                // Play the dismiss audio clip with volume doubled if assigned.
                if (dismissAudioClip != null)
                {
                    AudioSource.PlayClipAtPoint(dismissAudioClip, transform.position, 4f);
                }

                if (autoDismissCoroutine != null)
                {
                    StopCoroutine(autoDismissCoroutine);
                }
                
                if (popupInstance != null)
                {
                    Destroy(popupInstance);
                }
                popupActive = false;
                // Correct answer received: destroy this object.
                Destroy(gameObject);
            }
            // If any key is pressed, but not the correct dismiss key, close the popup.
            else if (Input.anyKeyDown)
            {
                if (autoDismissCoroutine != null)
                {
                    StopCoroutine(autoDismissCoroutine);
                }
                
                if (popupInstance != null)
                {
                    Destroy(popupInstance);
                }
                popupActive = false;
            }
        }
    }

    // Coroutine to auto-dismiss the popup after a certain time if no correct answer is given.
    IEnumerator AutoDismissPopup()
    {
        yield return new WaitForSeconds(autoDismissTime);
        if (popupActive)
        {
            if (popupInstance != null)
            {
                Destroy(popupInstance);
            }
            popupActive = false;
            // Ensure the collider remains a trigger so that the popup system can be triggered again.
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D col in colliders)
            {
                col.isTrigger = true;
            }
        }
    }

    // Helper method to check if a key is a numpad key
    private bool IsKeypadKey(KeyCode key)
    {
        return key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9;
    }

    // Helper method to get the corresponding alpha key for a numpad key
    private KeyCode GetAlphaEquivalent(KeyCode keypadKey)
    {
        switch (keypadKey)
        {
            case KeyCode.Keypad0: return KeyCode.Alpha0;
            case KeyCode.Keypad1: return KeyCode.Alpha1;
            case KeyCode.Keypad2: return KeyCode.Alpha2;
            case KeyCode.Keypad3: return KeyCode.Alpha3;
            case KeyCode.Keypad4: return KeyCode.Alpha4;
            case KeyCode.Keypad5: return KeyCode.Alpha5;
            case KeyCode.Keypad6: return KeyCode.Alpha6;
            case KeyCode.Keypad7: return KeyCode.Alpha7;
            case KeyCode.Keypad8: return KeyCode.Alpha8;
            case KeyCode.Keypad9: return KeyCode.Alpha9;
            default: return keypadKey;
        }
    }
}
