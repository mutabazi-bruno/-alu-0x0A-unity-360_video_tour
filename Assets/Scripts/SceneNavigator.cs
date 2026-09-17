using UnityEngine;

public class SceneNavigator : MonoBehaviour
{
    public const string MainMenu = "MainMenuScene";
    public const string IntranetTour = "IntranetTourScene";
    public const string CampusTour = "CustomCampusTourScene";

    [Tooltip("Scene name as it appears in the build scene list, e.g. MainMenuScene")]
    public string targetScene = MainMenu;

    public void Go()
    {
        SceneFader.LoadScene(targetScene);
    }
}
