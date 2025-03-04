using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class to manage a progress bar using a Unity UI Slider component.
/// It allows for setting the maximum value, updating the current value, 
/// and adjusting the fill color based on a gradient.
/// </summary>
public class ProgressBar : MonoBehaviour
{
    /// <summary>
    /// The gradient used to change the fill color of the progress bar.
    /// </summary>
    public Gradient Gradient;

    /// <summary>
    /// The Image component that displays the fill of the progress bar.
    /// </summary>
    public Image Fill;

    /// <summary>
    /// The Slider component used for the progress bar.
    /// </summary>
    private Slider _slider;

    /// <summary>
    /// Minimum value of the progress bar.
    /// </summary>
    private const int _minValue = 0;

    /// <summary>
    /// Initializes the Slider component and sets its value to the minimum.
    /// </summary>
    void Awake()
    {
        _slider = GetComponent<Slider>();
        ResetSlider();
    }

    /// <summary>
    /// Resets the progress bar to its minimum value.
    /// </summary>
    public void StartIt()
    {
        ResetSlider();
    }

    /// <summary>
    /// Sets the maximum value of the progress bar and resets its current value.
    /// </summary>
    /// <param Name="val">The maximum value to set.</param>
    public void SetMaxValue(float val)
    {
        _slider.maxValue = val;  // Set the maximum value
        ResetSlider();            // Reset to minimum value
        Fill.color = Gradient.Evaluate(1f); // Set fill color to the end of the gradient
    }

    /// <summary>
    /// Sets the current value of the progress bar and updates the fill color accordingly.
    /// </summary>
    /// <param Name="val">The current value to set.</param>
    public void SetValue(float val)
    {
        _slider.value = val; // Update the current value
        Fill.color = Gradient.Evaluate(_slider.normalizedValue); // Update fill color based on current value
    }

    /// <summary>
    /// Resets the slider to its minimum value and sets the fill color.
    /// </summary>
    private void ResetSlider()
    {
        _slider.value = _minValue; // Reset value
        Fill.color = Gradient.Evaluate(0f); // Set fill color to the start of the gradient
    }
}
