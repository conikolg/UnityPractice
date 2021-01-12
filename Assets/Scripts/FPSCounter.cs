using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    // Reference to Text object
    private Text _displayText;

    // Number of times per second to update FPS counter
    private const float RefreshRate = 2f;

    // Timestamp of next update needed
    private float _timer;

    public void Start()
    {
        _displayText = GetComponent<Text>();
    }

    public void Update()
    {
        // Only update if next update time has been reached
        if (Time.unscaledTime > _timer)
        {
            // Standard readout of floating point number
            _displayText.text = string.Format(
                new System.Globalization.CultureInfo("en-US"),
                "{0}", 1f / Time.unscaledDeltaTime);
            
            // Set next update time
            _timer = Time.unscaledTime + 1f / RefreshRate;
        }
    }
}