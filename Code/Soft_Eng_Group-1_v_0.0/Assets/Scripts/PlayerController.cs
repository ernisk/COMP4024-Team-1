using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişi için gerekli

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Hareket hızı

    private Rigidbody2D rb;
    private Vector2 movementInput;

    // Toplanan collectible sayısını tutan değişken
    private int collectedCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D bileşenini al
    }

    void Update()
    {
        // Klavye girişlerini al (WASD veya yön tuşları)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        // Köşelerde fazla hızlı hareketi önlemek için normalize et
        movementInput = movementInput.normalized;
    }

    void FixedUpdate()
    {
        // Rigidbody2D ile hareket ettir
        rb.linearVelocity = movementInput * moveSpeed;
    }

    // Trigger olan collider'larla çarpışma kontrolü
    void OnTriggerEnter2D(Collider2D other)
    {
        // Eğer çarpışılan obje "Collectible" tag'ine sahipse
        if (other.CompareTag("Collectible"))
        {
            Destroy(other.gameObject);
            collectedCount++;
            Debug.Log("Toplanan Collectible Sayısı: " + collectedCount);
        }

        // Eğer çarpışılan obje "Finish" tag'ine sahipse
        if (other.CompareTag("Finish"))
        {
            Debug.Log("Finish'e ulaşıldı. Ana menüye geçiliyor...");
            SceneManager.LoadScene("Main_Menu");
        }
    }
}
