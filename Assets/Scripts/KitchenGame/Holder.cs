using UnityEngine;

/// <summary>
/// The Holder class extends Touchable.
/// </summary>
public class Holder : Touchable
{
    private Color _originalColor; // Stores the original color of the sprite

    protected override void Start()
    {
        base.Start();
        _originalColor = _spR.color;
    }

    protected override void Update()
    {
        OnTriggerTouch();
        OnTriggerDrag();
    }

    /// <summary>
    /// Restores the sprite color to its original value.
    /// </summary>
    public void RestoreColor() => _spR.color = _originalColor;

    protected override void StartTouch(Touch touch)
    {
        base.StartTouch(touch);
        Color newColor = _originalColor;
        newColor.a = 0.2f;
        _spR.color = newColor;
    }

    protected override void Drag(Vector2 newPosition)
    {
        return;
    }

    public override void Drop()
    {
        base.Drop();
        _spR.color = _originalColor;
    }
}
