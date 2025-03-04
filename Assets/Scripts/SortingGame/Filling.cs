using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a "filling" object in the game that can be dragged, shrinks over time if not interacted with, 
/// and interacts with specific tiles on the game board.
/// </summary>
public class Filling : Touchable
{
    public FillingType Type;                // Type of filling, set externally for game logic

    protected bool _isOnTile;               // True if the filling is positioned on a valid tile

    protected Color _originalColor;         // Original color of the filling for resetting after visual effects
    protected Vector2 _originalSize;        // Initial size of the filling for resetting after shrinking

    protected bool _shrink;                 // If true, the filling will shrink over time
    protected float _shrinkDuration;        // Duration of the shrink animation
    protected Vector2 _minSize;             // Minimum size for the filling after shrinking
    protected float _timer;                 // Timer to track shrinking progress

    /// <summary>
    /// Initializes the filling, setting its color, size, and shrinking behavior.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        SetOrderInLayer();                  // Set rendering layer order to handle overlap correctly

        _isOnTile = false;
        _originalColor = _spR.color;
        _originalSize = transform.localScale;

        _minSize = new Vector2(0.3f, 0.3f); // Define minimum shrink size
        _timer = 0f;

        // Retrieve shrink settings from the game manager
        _shrink = SGGameManager.instance.shrinkFilling;
        _shrinkDuration = SGGameManager.instance.shrinkFillingDuration;
    }

    /// <summary>
    /// Updates the filling's state, shrinking it if enabled.
    /// </summary>
    protected override void Update()
    {
        if (_shrink)
            Shrink();
    }

    /// <summary>
    /// Sets the sprite's order in layer to ensure correct rendering in front of other objects.
    /// </summary>
    private void SetOrderInLayer()
    {
        _spR.sortingOrder = (int)SGGameManager.instance.fillingLayers % _maxOrderInLayer;
        SGGameManager.instance.fillingLayers++;
    }

    /// <summary>
    /// Checks if this filling object is the closest one to the touch point.
    /// </summary>
    /// <param name="currentTouch">The touch input data.</param>
    /// <returns>True if this object is the closest to the touch point; otherwise, false.</returns>
    protected override bool IsClosestObjectToTouch(Touch currentTouch)
    {
        Vector2 touchPosition = currentTouch.position;

        // Iterate over all fillings to check if another is closer to the touch point
        foreach (var tr in SGGameManager.instance.Fillings)
        {
            if (tr.GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(touchPosition)))
            {
                uint order = (uint)tr.GetComponent<SpriteRenderer>().sortingOrder % _maxOrderInLayer;
                if (order > _spR.sortingOrder)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Resets position if dropped outside a tile and finalizes the touch interaction.
    /// </summary>
    public override void Drop()
    {
        if (_isOnTile)
        {
            transform.position = _positionBeforeDrag;
            _isOnTile = false;
        }
        base.Drop();
    }

    /// <summary>
    /// Deletes the filling, with optional parameter for if it was dropped in a trash can area.
    /// </summary>
    /// <param name="inTrashCan">True if the filling is deleted by dropping it in a trash can.</param>
    public virtual void Delete(bool inTrashCan)
    {
        SGGameManager.instance.DeleteFilling(gameObject, inTrashCan);
    }

    /// <summary>
    /// Handles entering a tile's area, marking the filling as being on a tile.
    /// </summary>
    /// <param name="collision">Collider data of the entering object.</param>
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
            EnterTile();
    }

    /// <summary>
    /// Handles exiting a tile's area, marking the filling as no longer on a tile.
    /// </summary>
    /// <param name="collision">Collider data of the exiting object.</param>
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tile"))
            ExitTile();
    }

    /// <summary>
    /// Marks the filling as being on a tile, enabling valid drop behavior.
    /// </summary>
    public void EnterTile() => _isOnTile = true;

    /// <summary>
    /// Marks the filling as no longer being on a tile.
    /// </summary>
    public void ExitTile() => _isOnTile = false;

    /// <summary>
    /// Shrinks the filling's size over time if not being touched.
    /// </summary>
    protected virtual void Shrink()
    {
        if (!_isTouched)
        {
            if (_timer < _shrinkDuration)
            {
                _timer += Time.deltaTime;
                float t = _timer / _shrinkDuration;
                transform.localScale = Vector2.Lerp(_originalSize, _minSize, t);
            }
            else
            {
                Delete(false); // Delete the filling after shrinking completes
            }
        }
    }

    /// <summary>
    /// Changes the filling's color to green briefly, then fades it back to its original color.
    /// </summary>
    protected IEnumerator Good()
    {
        _spR.color = Color.green;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            _spR.color = Color.Lerp(Color.green, _originalColor, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _spR.color = _originalColor;
    }
}
