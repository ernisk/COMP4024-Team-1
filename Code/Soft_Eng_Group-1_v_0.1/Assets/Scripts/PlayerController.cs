using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene transitions

public class PlayerController2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movementInput;

    // Variable to store the number of collected collectibles
    private int collectedCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }
    void Update()
    {
        // Get keyboard input (WASD or arrow keys)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Normalize input to prevent excessively fast diagonal movement
        movementInput = movementInput.normalized;
    }   
}
