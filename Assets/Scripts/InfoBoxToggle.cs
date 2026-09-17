using UnityEngine;

/// <summary>
/// Toggles a single info box panel open and closed. Attach this to each
/// info button (e.g. the speech-bubble hotspot), assign the info box's
/// GameObject in the Inspector, then hook this script's Toggle() method
/// to that button's On Click event.
/// </summary>
public class InfoBoxToggle : MonoBehaviour
{
    [Tooltip("The info box panel this button opens and closes")]
    public GameObject infoBox;

    /// <summary>
    /// Shows the info box if it's currently hidden, or hides it if it's
    /// currently shown.
    /// </summary>
    public void Toggle()
    {
        infoBox.SetActive(!infoBox.activeSelf);
    }
}