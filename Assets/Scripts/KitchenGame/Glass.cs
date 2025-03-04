using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a glass that interacts with water and moves between positions.
/// </summary>
public class Glass : MonoBehaviour
{
    public Vector2 originalPosition;  // Stores the original position of the glass
    public int waterNum = 0;  // Tracks the amount of water inside the glass

    [SerializeField] private Glass _otherGlass;  // Reference to the other glass in the game
    [SerializeField] private PGGameManager _gameManager;  // Reference to the game manager

    private Rigidbody2D _rb;

    private Vector2 _targetPosition;
    private bool _active;

    private const float _targetWaterNum = 80f;  // Required water amount to switch active glasses
    private const float _positionOffset = 10f;
    private const float _moveSpeed = 8f;

    /// <summary>
    /// Initializes the glass by setting its position and activation state.
    /// </summary>
    /// <param name="active">Indicates if the glass should start as active.</param>
    public void Play(bool active)
    {
        _rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
        _targetPosition = new Vector2(originalPosition.x, originalPosition.y - _positionOffset);
        SetActive(active);
    }

    public void Update()
    {
        if (_active && waterNum >= _targetWaterNum)
        {
            SetActive(false);
            _otherGlass.SetActive(true);
            _gameManager.CurGlasses++;
        }
    }

    /// <summary>
    /// Moves the glass smoothly to the target position.
    /// </summary>
    /// <param name="target">The target position to move towards.</param>
    private IEnumerator Move(Vector2 target)
    {
        while (transform.position.y != target.y)
        {
            _rb.MovePosition(Vector2.MoveTowards(_rb.position, target, _moveSpeed * Time.deltaTime));
            yield return null;
        }

        if (target == _targetPosition)
            waterNum = 0;
    }

    /// <summary>
    /// Activates or deactivates the glass and moves it accordingly.
    /// </summary>
    /// <param name="a">True to activate, false to deactivate.</param>
    public void SetActive(bool a)
    {
        _active = a;
        if (_active)
            StartCoroutine(Move(originalPosition));
        else
            StartCoroutine(Move(_targetPosition));
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (_active && other.CompareTag("Water"))
            waterNum++;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
            waterNum--;
    }
}