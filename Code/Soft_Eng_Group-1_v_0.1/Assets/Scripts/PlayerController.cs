using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene transitions

public class PlayerController2D : MonoBehaviour
{
    private Rigidbody2D rb;

    // Variable to store the number of collected collectibles
    private int collectedCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
    }
}
