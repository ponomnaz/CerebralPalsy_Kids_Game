using UnityEngine;

/// <summary>
/// Represents a liquid object that gets destroyed upon colliding with a wall.
/// </summary>
public class Liquid : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
            Destroy(gameObject);
    }
}
