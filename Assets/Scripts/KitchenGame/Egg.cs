using System.Collections;
using UnityEngine;

/// <summary>
/// The Egg class controls the egg's movement, fade-out effect, and its interaction with the pan.
/// </summary>
public class Egg : Accelerometer
{
    protected SpriteRenderer _spR;
    protected Color _originalColor;
    protected FGGameManager _gameManager;
    protected Rigidbody2D _rb;

    protected const float _minSpeed = 15.0f;
    protected const float _maxSpeed = 20.0f;

    protected bool _canMove = false;

    protected float _originalTransparency;
    protected const float _fadeDuration = 3f;
    protected const float _targetTransparency = 0f;
    protected float _transparencyIncrement;
    protected float _timer;

    /// <summary>
    /// Sets whether the egg can move or not.
    /// </summary>
    /// <param name="canMove">True if the egg can move, false otherwise.</param>
    public void SetMove(bool canMove) => _canMove = canMove;

    protected override void Start ()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spR = GetComponent<SpriteRenderer>();
        _originalColor = GetComponent<SpriteRenderer>().color;
        _originalTransparency = _originalColor.a;

        _gameManager = FindObjectOfType<FGGameManager>();

        _speed = GetRandomSpeed();

        _timer = 0f;
        _transparencyIncrement = (_targetTransparency - _originalTransparency) / _fadeDuration;
    }

    protected override void Update ()
    {
        if (_canMove)
            Move(GetAccelerometerValue(), _speed);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pan"))
        {
            if (_canMove)
                Handheld.Vibrate();

            _canMove = false;
            StartCoroutine(FadeObject());
        }
    }

    /// <summary>
    /// Generates a random speed for the egg's movement within the defined range.
    /// </summary>
    /// <returns>A random speed value between minSpeed and maxSpeed.</returns>
    private float GetRandomSpeed() => Random.Range(_minSpeed, _maxSpeed);

    /// <summary>
    /// Coroutine to fade out the egg's transparency and then destroy it.
    /// </summary>
    /// <returns>An IEnumerator for the coroutine.</returns>
    private IEnumerator FadeObject()
    {

        _gameManager.eggs.Remove(gameObject);
        while (_timer < _fadeDuration)
        {
            float newTransparency = _originalTransparency + _transparencyIncrement * _timer;

            Color newColor = _spR.color;
            newColor.a = newTransparency;
            _spR.color = newColor;

            _timer += Time.deltaTime;

            yield return null;
        }
        Destroy(gameObject);
    }
}
