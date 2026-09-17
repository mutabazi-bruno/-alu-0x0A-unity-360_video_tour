using UnityEngine;

// Opens and closes a point-of-interest info panel. Hook a button's OnClick to Toggle().
public class InfoBoxToggle : MonoBehaviour
{
    public GameObject infoBox;

    public void Toggle()
    {
        if (infoBox != null) infoBox.SetActive(!infoBox.activeSelf);
    }
}
