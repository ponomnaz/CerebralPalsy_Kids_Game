using UnityEngine;

/// <summary>
/// Represents a jug that rotates based on accelerometer input and tracks the amount of poured water.
/// </summary>
public class Jug : Accelerometer
{
    public int water = 0; // Tracks the amount of water poured

    private Rigidbody2D _rb;
    private bool _pour = false;

    private const float _rotationSpeed = 100f;

    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Resets the jug to its original state, stopping pouring and resetting water count.
    /// </summary>
    public void StartPouring()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        water = 0;
        _pour = false;
    }

    public void ContinuePouring() => _pour = true;

    public void StopPouring() => _pour = false;

    protected override void Update()
    {
        base.Update();
        if (_pour)
            UpdateRotation(GetAccelerometerValue().x);
    }

    protected override void UpdateRotation(float value)
    {
        float rotationAmount = -value * _rotationSpeed * Time.deltaTime;

        _rb.MoveRotation(_rb.rotation + rotationAmount);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
            water++;
    }

}
