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

    public void StartQuestion()
    {
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (question_image != null)
        {
            question_image.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Question image not assigned in inspector!");
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
