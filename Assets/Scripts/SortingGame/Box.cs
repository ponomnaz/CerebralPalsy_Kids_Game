using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a Box that can be filled with specific items (Fillings) and can spawn coins based on set probabilities.
/// </summary>
public class Box : Touchable
{
    public Sprite ClosedBox;           // Sprite for closed box state
    public Sprite OpenedBox;           // Sprite for opened box state
    public Sprite FullBox;             // Sprite for full box state
    public FillingType Type;           // Type of filling the box accepts

    protected GameObject _coinPrefabe; // Coin prefab (if applicable for dropping)

    protected Color _originalColor;    // Initial color of the box
    protected Color _fullCanColor;     // Color for when box is full
    protected Color _errorColor;       // Color for incorrect filling
    protected Color _correctColor;     // Color for correct filling
    protected Color _coinDroppedColor; // Color for when coin is dropped

    protected float _errorDuration;    // Duration of error color effect
    protected float _correctDuration;  // Duration of correct color effect
    protected float _coinDuration;     // Duration of coin color effect
    protected int _currentFillingNum;  // Current number of fillings in the box
    protected int _targetFillingNum;   // Target number of fillings needed to fill the box

    protected float _coinDropProb;     // Probability of dropping a coin

    protected bool _isFull;            // Flag indicating if the box is full

    // Sets the target number of fillings needed to complete the box
    public void SetTargetFillingNum(int num) => _targetFillingNum = num;

    // Sets the probability of dropping a coin
    public void SetCoinDropProb(float prob) => _coinDropProb = prob;

    protected override void Start()
    {
        base.Start();
        _spR.sprite = ClosedBox;

        _originalColor = _spR.color;
        _fullCanColor = Color.green;
        _errorColor = Color.red;
        _correctColor = Color.green;
        _coinDroppedColor = Color.yellow;

        _errorDuration = 1f;
        _correctDuration = 0.5f;
        _coinDuration = 0.5f;
        _currentFillingNum = 0;

        _isFull = false;
    }

    protected override void StartTouch(Touch touch)
    {
        base.StartTouch(touch);
        if (!_isFull)
            _spR.sprite = OpenedBox;
    }

    // Checks if this box is the closest to the touch
    protected override bool IsClosestObjectToTouch(Touch currentTouch)
    {
        Vector2 touchPosition = currentTouch.position;

        foreach (var box in SGGameManager.instance.Boxes)
        {
            if (box.GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(touchPosition)))
            {
                uint order = (uint)box.GetComponent<SpriteRenderer>().sortingOrder % _maxOrderInLayer;
                if (order > _spR.sortingOrder)
                    return false;
            }
        }

        foreach (var coin in SGGameManager.instance.Coins)
        {
            if (coin.GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(touchPosition)))
                return false;
        }

        return true;
    }

    protected override Vector2 CheckCollisions(Vector2 newPosition)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(newPosition);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, _collider.bounds.extents.x);

        bool tileOnSide = false;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Grass"))
            {
                tileOnSide = true;
                break;
            }
        }

        Vector2 movePosition = Camera.main.ScreenToWorldPoint(newPosition);

        if (tileOnSide)
            movePosition.x = _rb.position.x;

        return movePosition;
    }

    public override void Drop()
    {
        base.Drop();
        if (!_isFull)
            _spR.sprite = ClosedBox;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isTouched && collision.CompareTag("Filling"))
        {
            Filling filling = collision.GetComponent<Filling>();
            if (filling.GetIsTouched())
                FillingInBox(filling);
        }
    }

    // Handles logic for adding fillings to the box
    protected void FillingInBox(Filling filling)
    {
        if (filling.Type == Type)
        {
            if (_isFull)
                filling.Delete(false);
            else
                filling.Delete(true);

            CorrectFilling();
        }
        else
        {
            filling.Drop();
            WrongFilling();
        }
    }

    // Indicates the filling was incorrect, causing vibration and error display
    protected void WrongFilling()
    {
        Handheld.Vibrate();
        SGGameManager.instance.errors++;
        Drop();
        StartCoroutine(ChangeColor(_errorColor, _errorDuration, true));
    }

    // Indicates the filling was correct, updating box state and optionally dropping a coin
    protected void CorrectFilling()
    {
        if (!_isFull)
        {
            _currentFillingNum++;
            BoxIsFull();
            if (DropCoin())
            {
                Handheld.Vibrate();
                Drop();
                StartCoroutine(ChangeColor(_coinDroppedColor, _coinDuration, true));
            }
            else
            {
                StartCoroutine(ChangeColor(_correctColor, _correctDuration, false));
            }
        }
    }

    // Checks if a coin should be dropped based on probability
    protected bool DropCoin()
    {
        if (Random.value <= _coinDropProb)
        {
            SGGameManager.instance.SpawnCoin(_collider.bounds.center);
            return true;
        }
        return false;
    }

    // Updates the box to "full" status once target filling number is reached
    protected void BoxIsFull()
    {
        if (!_isFull && _currentFillingNum >= _targetFillingNum)
        {
            if (FullBox != null)
            {
                _spR.sprite = FullBox;
            }
            else
            {
                _spR.color = _fullCanColor;
                _originalColor = _fullCanColor;
            }
            Handheld.Vibrate();
            _isFull = true;
        }
    }

    // Changes the color temporarily, then reverts back to the original color
    protected IEnumerator ChangeColor(Color color, float duration, bool block)
    {
        if (block)
            _canTouch = false;

        _spR.color = color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _spR.color = Color.Lerp(color, _originalColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _spR.color = _originalColor;
        if (block)
            _canTouch = true;
    }
}
