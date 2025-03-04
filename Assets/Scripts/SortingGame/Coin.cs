using UnityEngine;

/// <summary>
/// Represents a coin in the game that can fade over time and be collected upon touch.
/// </summary>
public class Coin : Touchable
{
    protected float _originalTransparency;     // Initial transparency level of the coin
    protected bool _fade;                      // Determines whether the coin should fade out over time
    protected float _fadeDuration;             // Duration over which the coin fades to target transparency
    protected float _targetTransparency;       // Transparency level the coin fades to (typically zero for full fade-out)
    protected float _timer;                    // Tracks time elapsed during fading

    protected float _transparencyIncrement;    // Rate of transparency change per second for fade effect

    /// <summary>
    /// Sets the duration over which the coin will fade.
    /// </summary>
    /// <param name="duration">Fade duration in seconds.</param>
    public void SetFadeDuration(float duration) => _fadeDuration = duration;

    /// <summary>
    /// Enables or disables fading for the coin.
    /// </summary>
    /// <param name="fade">True to enable fading, false to disable.</param>
    public void SetFade(bool fade) => _fade = fade;

    /// <summary>
    /// Initializes the coin's properties, including transparency and fade increment.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        _originalTransparency = _spR.color.a;                   // Store original transparency
        _targetTransparency = 0f;                               // Fade target is fully transparent
        _timer = 0f;                                            // Reset fade _timer
        _transparencyIncrement = (_targetTransparency - _originalTransparency) / _fadeDuration;  // Calculate fade rate
    }

    /// <summary>
    /// Updates the coin each frame, applying fade effect if enabled.
    /// </summary>
    protected override void Update()
    {
        if (_fade)
            FadeObject();
    }

    /// <summary>
    /// Fades the coin's transparency gradually over the fade duration. Deletes the coin when fully faded.
    /// </summary>
    protected void FadeObject()
    {
        if (_timer < _fadeDuration)
        {
            // Calculate new transparency based on time and increment
            float newTransparency = _originalTransparency + _transparencyIncrement * _timer;

            // Update the coin's transparency
            Color newColor = _spR.color;
            newColor.a = newTransparency;
            _spR.color = newColor;

            _timer += Time.deltaTime;  // Increment _timer
        }
        else
        {
            Delete(false);  // Delete coin if fully faded
        }
    }

    /// <summary>
    /// Handles touch interaction, deleting the coin immediately upon touch (collecting it).
    /// </summary>
    /// <param name="touch">The touch input data.</param>
    protected override void StartTouch(Touch touch) => Delete(true);

    /// <summary>
    /// Deletes the coin and notifies the game manager whether it was collected.
    /// </summary>
    /// <param name="collected">True if the coin was collected by the player; false if it faded out.</param>
    public virtual void Delete(bool collected) => SGGameManager.instance.DeleteCoin(gameObject, collected);
}
