using System.Collections;
using UnityEngine;

/// <summary>
/// Controls object shaking based on accelerometer input.
/// </summary>
public class ShakeController : Accelerometer
{
    public bool shake;

    public float curScore;  // Tracks the score based on shaking

    private float _timer;

    private Vector2 _originalPosition;
    private Vector2 _curDirection;

    private const float _moveSpeed = 8f;
    private const float _moveDistance = 0.8f;
    private const float _timeForShake = 8f;

    /// <summary>
    /// Initializes the ShakeController, setting initial values.
    /// </summary>
    public void Play()
    {
        base.Start();
        shake = false;
        _timer = 0f;
        curScore = 0f;
        _originalPosition = transform.position;
        _curDirection = Vector2.down;  // Initial shake direction
    }

    protected override void Update()
    {
        if (shake)
            Shaking(GetAccelerometerValue(), _shakeDetectionThreshold);

        _timer += Time.deltaTime;
        if (_timer >= _timeForShake)
        {
            SetDirection(ChangedDirection()); // Change shake direction after a set time
            _timer = 0f;
        }
    }

    /// <summary>
    /// Increases the shake score based on accelerometer movement.
    /// </summary>
    /// <param name="acceleration">Current accelerometer values.</param>
    /// <param name="sakeThreshold">Threshold for detecting a shake.</param>
    protected override void Shaking(Vector3 acceleration,  float sakeThreshold)
    {
        Vector3 deltaAcceleration = acceleration;

        // Increase score if shaking in the current direction
        if (Mathf.Abs(deltaAcceleration.x) >= sakeThreshold && _curDirection == Vector2.right)
            curScore += Time.deltaTime;

        if (Mathf.Abs(deltaAcceleration.y) >= sakeThreshold && _curDirection == Vector2.down)
            curScore += Time.deltaTime;
    }

    /// <summary>
    /// Continuously moves the object back and forth to simulate shaking.
    /// </summary>
    private IEnumerator ShakeObject()
    {
        while (true)
        {
            Vector2 targetPosition = _originalPosition - _curDirection * _moveDistance;

            yield return StartCoroutine(Move(Vector2.Distance(_originalPosition, targetPosition), _originalPosition, targetPosition));

            // Move back to original position
            yield return StartCoroutine(Move(Vector2.Distance(transform.position, _originalPosition), targetPosition, _originalPosition));
            
            targetPosition = _originalPosition + _curDirection * _moveDistance;

            yield return StartCoroutine(Move(Vector2.Distance(_originalPosition, targetPosition), _originalPosition, targetPosition));

            // Move back to original position
            yield return StartCoroutine(Move(Vector2.Distance(transform.position, _originalPosition), targetPosition, _originalPosition));
        }
    }

    /// <summary>
    /// Moves the object smoothly between two points.
    /// </summary>
    /// <param name="distance">Total distance to move.</param>
    /// <param name="start">Starting position.</param>
    /// <param name="target">Target position.</param>
    private IEnumerator Move(float distance, Vector2 start, Vector2 target)
    {
        float duration = distance / _moveSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(start, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = target;
    }

    /// <summary>
    /// Stops shaking and resets the object's position before changing direction.
    /// </summary>
    /// <param name="dir">New shake direction.</param>
    private void SetDirection(Vector2 dir)
    {
        StopShaking();
        _curDirection = dir;
        transform.position = _originalPosition;
        StartShaking();
    }

    /// <summary>
    /// Switches the shake direction between down and right.
    /// </summary>
    private Vector2 ChangedDirection()
    {
        if (_curDirection == Vector2.down)
            return Vector2.right;
        else
            return Vector2.down;
    }

    public void StopShaking() => StopAllCoroutines();

    public void StartShaking() => StartCoroutine(ShakeObject());
}
