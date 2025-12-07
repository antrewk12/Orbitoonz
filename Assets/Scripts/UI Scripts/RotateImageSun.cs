using UnityEngine;

public class RotateImageSun : MonoBehaviour
{
    public float rotationSpeed = 10f;  // Speed of rotation

    void Update()
    {
        // Rotate around Z axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}