using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public float scrollSpeed = 1.0f; // Speed of scrolling
    public Transform[] backgroundImages; // Array of background images

    private float lowerYBoundary = -14.0f;
    private float upperYBoundary = 43.0f;

    private void Update()
    {
        // Move each background image downwards
        foreach (Transform bgImage in backgroundImages)
        {
            // Calculate the new position based on time, speed, and the image's current position
            Vector3 newPosition = bgImage.position - scrollSpeed * Time.deltaTime * Vector3.up;

            // If the image is completely off screen, reset its position to the top
            if (newPosition.y <= lowerYBoundary) // Change this value depending on your image height
            {
                newPosition.y += upperYBoundary; // Change this value to twice your image height
            }

            // Apply the new position
            bgImage.position = newPosition;
        }
    }
}