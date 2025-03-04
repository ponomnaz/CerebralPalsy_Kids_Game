using UnityEngine;

/// <summary>
/// Adjusts the camera's orthographic size to fit the _width of a specified SpriteRenderer.
/// This ensures that the entire Rink is visible on the screen.
/// </summary>
public class CameraSize : MonoBehaviour
{
    /// <summary>
    /// The SpriteRenderer representing the Rink whose size determines the camera's orthographic size.
    /// </summary>
    public SpriteRenderer Rink;

    /// <summary>
    /// The Camera component used to render the scene.
    /// </summary>
    public Camera Cam;

    /// <summary>
    /// Initializes the camera size based on the Rink's dimensions.
    /// </summary>
    void Start()
    {
        // Check if the Rink and camera are assigned
        if (Rink != null && Cam != null)
        {
            // Calculate the orthographic size based on the Rink's _width and the screen's aspect ratio
            float orthoSize = Rink.bounds.size.x * Screen.height / Screen.width * 0.5f;
            Cam.orthographicSize = orthoSize;

            // Optional: Log the calculated orthographic size for debugging purposes
            Debug.Log($"Camera orthographic size set to: {Cam.orthographicSize}");
        }
        else
        {
            Debug.LogError("Rink or Camera is not assigned in the CameraSize script.");
        }
    }
}
