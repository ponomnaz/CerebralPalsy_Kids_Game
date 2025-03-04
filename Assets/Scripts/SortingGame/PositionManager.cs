using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the positions of spawn areas and tilemaps based on the user's hand preference.
/// </summary>
public class PositionManager : MonoBehaviour
{
    /// <summary>
    /// The SpriteRenderer for the area where boxes will spawn.
    /// </summary>
    public SpriteRenderer BoxesSpawnArea;

    /// <summary>
    /// The SpriteRenderer for the area where fillings will spawn.
    /// </summary>
    public SpriteRenderer FillingsSpawnArea;

    /// <summary>
    /// The radius within which coins can spawn.
    /// </summary>
    public Vector2 CoinSpawnRadius = new(7f, 7f);

    /// <summary>
    /// The Tilemap for grass on the left side.
    /// </summary>
    [SerializeField] private Tilemap _grassLeft;

    /// <summary>
    /// The Tilemap for tiles on the left side.
    /// </summary>
    [SerializeField] private Tilemap _tilesLeft;

    /// <summary>
    /// The Tilemap for grass on the right side.
    /// </summary>
    [SerializeField] private Tilemap _grassRight;

    /// <summary>
    /// The Tilemap for tiles on the right side.
    /// </summary>
    [SerializeField] private Tilemap _tilesRight;

    /// <summary>
    /// Adjusts the positions of spawn areas and activates the appropriate tilemaps based on the user's hand preference.
    /// </summary>
    /// <param Name="hand">The user's hand preference (left or right).</param>
    public void AdjustPositions(User.Hand hand)
    {
        // Check if the user's hand preference is Right
        if (hand == User.Hand.Right)
        {
            // Mirror the spawn areas for the right hand
            MirrorSpawnAreas();
            // Activate the right tilemaps
            ActivateTilemaps(_grassLeft, _tilesRight);
        }
        else
        {
            // Activate the left tilemaps
            ActivateTilemaps(_grassRight, _tilesLeft);
        }
    }

    /// <summary>
    /// Mirrors the spawn areas to the opposite side.
    /// </summary>
    private void MirrorSpawnAreas()
    {
        // Mirror the position of the BoxesSpawnArea
        BoxesSpawnArea.transform.position = new Vector2(-BoxesSpawnArea.transform.position.x, BoxesSpawnArea.transform.position.y);
        // Mirror the position of the FillingsSpawnArea
        FillingsSpawnArea.transform.position = new Vector2(-FillingsSpawnArea.transform.position.x, FillingsSpawnArea.transform.position.y);
    }

    /// <summary>
    /// Activates the specified tilemaps.
    /// </summary>
    /// <param Name="grass">The grass Tilemap to activate.</param>
    /// <param Name="tiles">The tiles Tilemap to activate.</param>
    private void ActivateTilemaps(Tilemap grass, Tilemap tiles)
    {
        grass.gameObject.SetActive(true);
        tiles.gameObject.SetActive(true);
    }
}