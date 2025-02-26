using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    // Update if key pressed matches answer number
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && answer_number == "1")
        {
            CorrectAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && answer_number == "2")
        {
            CorrectAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && answer_number == "3")
        {
            CorrectAnswer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && answer_number == "4")
        {
            CorrectAnswer();
        }
    }

    // Correct Answer Method, hiding question image and destroy obstacle object
    void CorrectAnswer()
    {
        if (question_image != null)
        {
            question_image.gameObject.SetActive(false);

            // ADD OBSTACLE OBJECT DESTROYED
        }
    }
}
