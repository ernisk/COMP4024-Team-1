using UnityEngine;

public class Ereaser : MonoBehaviour
{
    [Header("Discrete Movement Settings")]
    // Her tuşa basımında hareket edilecek mesafe (Ereaser'ın genişliği kadar)
    private float stepSize = 1f;

    private Rigidbody2D rb;

    private void Awake()
    {
        // Rigidbody2D bileşenini alıyoruz.
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on Ereaser!");
        }

        // SpriteRenderer üzerinden stepSize belirliyoruz (kare ise genişlik = yükseklik)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            stepSize = sr.bounds.size.x;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found on Ereaser, default stepSize (1f) will be used.");
        }
    }

    private void Update()
    {
        // Her tuşa basıldığında sadece bir seferlik hareket edilsin.
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2.right);
        }
    }

    // Verilen yönde, stepSize kadar hareket ettirir.
    private void Move(Vector2 direction)
    {
        Vector2 targetPosition = rb.position + direction * stepSize;
        rb.MovePosition(targetPosition);
    }

    // Çarpışma (trigger) anında Wall objelerini yok eder.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Duvar objeleri "Wall" ile isimlendirildiği varsayılıyor.
        if (collision.gameObject.name.StartsWith("Wall"))
        {
            Destroy(collision.gameObject);
        }
    }

    // Eğer obje ile temas devam ederse, yine yok etme işlemi yapılır.
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name.StartsWith("Wall"))
        {
            Destroy(collision.gameObject);
        }
    }
}
