using UnityEngine;

public class RotateObject2D : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;  
    private bool rotateClockwise = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // SPACE tuşuna basınca yön değiştirir.
        {
            rotateClockwise = !rotateClockwise;
        }

        float direction = rotateClockwise ? -1f : 1f;
        transform.Rotate(0, 0, direction * rotationSpeed * Time.deltaTime);
    }
}
