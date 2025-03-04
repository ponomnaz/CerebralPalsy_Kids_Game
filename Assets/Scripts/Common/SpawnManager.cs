using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages spawning of game objects within a specified area and ensures they are visible within the camera's view.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera; // The main camera to check object visibility

    /// <summary>
    /// Spawns a game object at a random position within the specified area and adds it to the target list.
    /// </summary>
    /// <param Name="obj">The game object prefab to spawn.</param>
    /// <param Name="_areaCenter">The center point of the spawn area.</param>
    /// <param Name="_areaSize">The size of the spawn area.</param>
    /// <param Name="target">The list to add the spawned instance.</param>
    public void Spawn(GameObject obj, Vector2 _areaCenter, Vector2 _areaSize, List<GameObject> target)
    {
        // Create an instance of the selected object at a random position and add it to the target list
        if (target != null)
            target.Add(Instantiate(obj, GetRandomSpawnPosition(_areaCenter, _areaSize), Quaternion.identity));
        else
            Instantiate(obj, GetRandomSpawnPosition(_areaCenter, _areaSize), Quaternion.identity);
    }

    /// <summary>
    /// Gets a random spawn position within the defined area that is also visible in the camera's view.
    /// </summary>
    /// <param Name="center">The center of the spawn area.</param>
    /// <param Name="size">The size of the spawn area.</param>
    /// <returns>A valid random position within the area that is visible in the camera.</returns>
    private Vector2 GetRandomSpawnPosition(Vector2 center, Vector2 size)
    {
        Vector2 position;

        // Keep generating a random position until one is found that is in view
        do
        {
            position = center + new Vector2(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2));
        }
        while (!IsObjectInView(position));

        return position; // Return the valid position
    }

    /// <summary>
    /// Checks if a given position is within the camera's view.
    /// </summary>
    /// <param Name="position">The position to check.</param>
    /// <returns>True if the position is within the camera's bounds; otherwise, false.</returns>
    private bool IsObjectInView(Vector2 position)
    {
        // Ensure the camera is assigned
        if (_mainCamera != null)
        {
            // Convert world position to viewport position
            Vector3 viewportPos = _mainCamera.WorldToViewportPoint(position);

            // Check if the position is within the camera's visible bounds
            return viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0;
        }
        else
        {
            Debug.LogError("Camera is not assigned.");
            return false; // Camera not assigned, cannot check visibility
        }
    }
}
