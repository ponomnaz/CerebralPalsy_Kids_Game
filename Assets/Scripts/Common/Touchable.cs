using UnityEngine;

/// <summary>
/// Touchable object that responds to touch interactions, supporting drag-and-drop functionality.
/// </summary>
public class Touchable : MonoBehaviour
{
    protected const ushort _maxOrderInLayer = 32000;    // Maximum sorting layer order value

    // Components
    protected Rigidbody2D _rb;                          // Rigidbody2D component for physics
    protected Collider2D _collider;                     // Collider2D component for detecting touch
    protected SpriteRenderer _spR;                      // SpriteRenderer component for visual representation

    // Touch state tracking
    protected Vector2 _positionBeforeDrag;              // Position of the object before it was dragged
    protected bool _canTouch;                           // Determines if the object can be touched
    [SerializeField] protected bool _isTouched;         // Indicates if the object is currently touched
    protected int _touchId;                             // Unique ID of the touch event

    /// <summary>
    /// Gets the current touched state of the object.
    /// </summary>
    public bool GetIsTouched() => _isTouched;

    /// <summary>
    /// Sets the touched state of the object.
    /// </summary>
    public void SetIsTouched(bool isTouched) => _isTouched = isTouched;

    /// <summary>
    /// Gets the ID of the touch event associated with this object.
    /// </summary>
    public int GetTouchId() => _touchId;

    /// <summary>
    /// Sets the ID of the touch event associated with this object.
    /// </summary>
    public void SetTouchId(int touchId) => _touchId = touchId;

    /// <summary>
    /// Initializes component references and sets initial values.
    /// </summary>
    protected virtual void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spR = GetComponent<SpriteRenderer>();

        _positionBeforeDrag = transform.position;
        _canTouch = true;
        _isTouched = false;
        _touchId = -1;
    }

    /// <summary>
    /// Handles any per-frame logic. Override this in subclasses as needed.
    /// </summary>
    protected virtual void Update()
    {
        // Empty for now, designed for overrides in derived classes
    }

    /// <summary>
    /// Checks if the object can be touched and starts a touch if the conditions are met.
    /// </summary>
    public void OnTriggerTouch()
    {
        if (!_isTouched && _canTouch)
        {
            for (int t = 0; t < Input.touchCount; t++)
            {
                Touch touch = Input.GetTouch(t);
                if (touch.phase == TouchPhase.Began &&
                    _collider.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)) &&
                    IsClosestObjectToTouch(touch))
                {
                    StartTouch(touch);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Handles dragging behavior while the object is being touched.
    /// </summary>
    public void OnTriggerDrag()
    {
        if (_isTouched)
        {
            Touch currentTouch = FindTouch(_touchId);
            switch (currentTouch.phase)
            {
                case TouchPhase.Began:
                    if (!TouchExist(_touchId))
                    {
                        Drop(); // Drop the object if touch is no longer valid
                    }
                    break;
                case TouchPhase.Moved:
                    Drag(currentTouch.position); // Move the object based on touch position
                    break;
                case TouchPhase.Stationary:
                    break; // Do nothing while stationary
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    Drop(); // Drop the object on touch end or cancellation
                    break;
            }
        }
    }

    /// <summary>
    /// Begins the touch interaction and sets initial touch parameters.
    /// </summary>
    /// <param name="touch">The touch that initiated interaction.</param>
    protected virtual void StartTouch(Touch touch)
    {
        _touchId = touch.fingerId;
        _isTouched = true;
    }

    /// <summary>
    /// Moves the object based on the touch position, with collision checks.
    /// </summary>
    /// <param name="newPosition">New screen position from the touch.</param>
    protected virtual void Drag(Vector2 newPosition)
    {
        if (_isTouched)
        {
            Vector2 movePosition = CheckCollisions(newPosition);
            _rb.MovePosition(movePosition);
        }
    }

    /// <summary>
    /// Checks for collisions around the object and adjusts the movement if necessary.
    /// </summary>
    /// <param name="newPosition">The target position of the object.</param>
    /// <returns>The adjusted position based on detected collisions.</returns>
    protected virtual Vector2 CheckCollisions(Vector2 newPosition)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(newPosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, _collider.bounds.extents.x);

        bool tileOnSide = false;
        bool tileOnTopBottom = false;

        // Check for specific tag collisions to restrict movement
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("TBBorder"))
                tileOnTopBottom = true;
            else if (collider.CompareTag("SBorder"))
                tileOnSide = true;
        }

        Vector2 movePosition = Camera.main.ScreenToWorldPoint(newPosition);

        // Restrict movement based on collision flags
        if (tileOnTopBottom)
            movePosition.y = _rb.position.y;
        if (tileOnSide)
            movePosition.x = _rb.position.x;

        return movePosition;
    }

    /// <summary>
    /// Ends the current touch interaction and resets touch parameters.
    /// </summary>
    public virtual void Drop()
    {
        _positionBeforeDrag = transform.position; // Reset to the position before dragging
        _touchId = -1;
        _isTouched = false;
    }

    /// <summary>
    /// Finds a touch with the specified touch ID.
    /// </summary>
    /// <param name="touchId">The ID of the touch to find.</param>
    /// <returns>The touch with the specified ID, or a default touch if not found.</returns>
    private Touch FindTouch(int touchId)
    {
        foreach (var touch in Input.touches)
        {
            if (touchId == touch.fingerId)
                return touch;
        }
        Debug.Log("FindTouch() cannot find the touch with the specified ID.");
        Debug.Break();
        return new Touch(); // Return a default touch if not found
    }

    /// <summary>
    /// Checks if a touch with the specified ID exists.
    /// </summary>
    /// <param name="touchId">The ID of the touch to check.</param>
    /// <returns>True if the touch exists, otherwise false.</returns>
    private bool TouchExist(int touchId)
    {
        foreach (var touch in Input.touches)
        {
            if (touchId == touch.fingerId)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Determines if this object is the closest to the touch point, allowing prioritization of touch targets.
    /// Override to implement custom distance checks if needed.
    /// </summary>
    /// <param name="currentTouch">The touch event to check against.</param>
    /// <returns>True if this is the closest object, otherwise false.</returns>
    protected virtual bool IsClosestObjectToTouch(Touch currentTouch)
    {
        return true; // Default behavior assumes it's the closest
    }
}
