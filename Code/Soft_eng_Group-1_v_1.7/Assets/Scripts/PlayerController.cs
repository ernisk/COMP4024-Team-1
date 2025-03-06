using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro support

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Movement speed

    [Header("UI Settings")]
    // Public TMP text for displaying the collected count.
    public TMP_Text collectedCountTMP;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    // Variable to store the number of collected collectibles.
    private int collectedCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        UpdateCollectedCountUI();
    }

    void Update()
    {
        // Get keyboard input (WASD or arrow keys)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Normalize input to prevent excessively fast diagonal movement
        movementInput = movementInput.normalized;
    }

    void FixedUpdate()
    {
        // Move using Rigidbody2D
        rb.linearVelocity = movementInput * moveSpeed;
    }

    // Collision detection for trigger colliders
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided with a collectible object
        if (other.CompareTag("Collectible"))
        {
            collectedCount++;
            UpdateCollectedCountUI();
            Destroy(other.gameObject);
        }
        // If the collided object has the tag "Finish"
        else if (other.CompareTag("Finish"))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName == "Level 3")
            {
                Debug.Log("Reached Finish in Level 3. Loaded Finish Screen. Loading Main_Menu...");
                SceneManager.LoadScene("Finish_Game");
            }
            else
            {
                Debug.Log("Reached Finish. Loaded Finish Screen. Loading Levels...");
                SceneManager.LoadScene("Finish_Level");
            }
        }
    }

    // Updates the TMP text with the current collected count
    void UpdateCollectedCountUI()
    {
        if (collectedCountTMP != null)
        {
            collectedCountTMP.text = "" + collectedCount;
        }
    }
}
