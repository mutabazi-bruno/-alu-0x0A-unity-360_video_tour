using UnityEngine;


public class InfoBoxToggle : MonoBehaviour
{
    public GameObject infoBox;

    public void Toggle()
    {
        if (infoBox != null) infoBox.SetActive(!infoBox.activeSelf);
    }
}
