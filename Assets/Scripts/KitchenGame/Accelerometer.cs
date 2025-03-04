using UnityEngine;

/// <summary>
/// This class is responsible for handling accelerometer input and applying shake detection and movement based on accelerometer values.
/// </summary>
public class Accelerometer : MonoBehaviour
{
    // Constants for the accelerometer update interval and low pass filter kernel _width in seconds.
    protected const float _updateInterval = 1.0f / 60.0f;
    protected const float _lowPassKernelWidthInSeconds = 1.0f;

    // Threshold value for shake detection. This defines the sensitivity for detecting significant accelerometer changes.
    protected float _shakeDetectionThreshold = 0.7f;

    // Low-pass filter settings
    protected float _lowPassFilterFactor;
    protected Vector3 _lowPassValue = Vector3.zero;

    // Speed for movement based on accelerometer input.
    protected float _speed;

    protected virtual void Start()
    {
        _lowPassFilterFactor = _updateInterval / _lowPassKernelWidthInSeconds;

        _shakeDetectionThreshold *= _shakeDetectionThreshold;

        _lowPassValue = GetAccelerometerValue();
    }

    protected virtual void Update()
    {

    }

    /// <summary>
    /// Detects shaking based on accelerometer data and the specified threshold.
    /// </summary>
    /// <param name="acceleration">Current accelerometer value.</param>
    /// <param name="shakeThreshold">Threshold to detect a shake.</param>
    protected virtual void Shaking(Vector3 acceleration, float sakeThreshold)
    {

    }

    /// <summary>
    /// Moves the object in the given direction with the specified speed.
    /// </summary>
    /// <param name="direction">The direction vector to move the object.</param>
    /// <param name="speed">The speed of movement.</param>
    protected virtual void Move(Vector2 direction, float speed)
    {
        if (direction.sqrMagnitude > 1)
            direction.Normalize();

        direction *= Time.deltaTime;

        transform.Translate(direction * speed);
    }

    /// <summary>
    /// Updates the rotation of the object based on a value.
    /// </summary>
    /// <param name="value">The value that determines the rotation angle.</param>
    protected virtual void UpdateRotation(float value)
    {
        if (value <= 0)
        {
            value = Mathf.Abs(value);
            float clampedValue = Mathf.Clamp01(value);

            float angle = Mathf.Lerp(0f, 90f, clampedValue);

            Vector3 rotation = new Vector3(0f, 0f, angle);

            Quaternion targetRotation = Quaternion.Euler(rotation);

            transform.rotation = targetRotation;
        }
    }

    /// <summary>
    /// Retrieves the current accelerometer value by averaging multiple acceleration events.
    /// </summary>
    /// <returns>The average accelerometer value over the current frame.</returns>
    protected Vector3 GetAccelerometerValue()
    {
        Vector3 acc = Vector3.zero;
        float period = 0.0f;

        foreach (var evnt in Input.accelerationEvents)
        {
            acc += evnt.acceleration * evnt.deltaTime;
            period += evnt.deltaTime;
        }

        if (period > 0)
            acc *= 1.0f / period;

        return acc;
    }


    /// <summary>
    /// Applies a low-pass filter to the accelerometer data to smooth out noise and fluctuations.
    /// </summary>
    /// <param name="prevValue">The previous accelerometer value (smoothed).</param>
    /// <param name="curValue">The current accelerometer value to filter.</param>
    /// <returns>The filtered accelerometer value.</returns>
    protected Vector3 LowPassFilterAccelerometer(Vector3 prevValue, Vector3 curValue)
    {
        Vector3 newValue = Vector3.Lerp(prevValue, curValue, _lowPassFilterFactor);
        return newValue;
    }
}