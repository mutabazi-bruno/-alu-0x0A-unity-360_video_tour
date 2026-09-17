using System.Collections;
using TMPro;
using UnityEngine;

// Custom campus tour: moves between 360 photo rooms with a fade.
// Each room only lists the rooms physically connected to it, so the tour
// stays continuous (Main Hall -> Upper Hall -> Chill Center, no skipping).
public class PanoramaNavigator : MonoBehaviour
{
    [System.Serializable]
    public class Room
    {
        public string roomName;

        [Tooltip("Parent holding this room's 360 sphere, hotspots and info point")]
        public GameObject root;

        [Tooltip("Indices of rooms you can walk to directly from this one")]
        public int[] connectedRooms;
    }

    public Room[] rooms;
    public int startingIndex = 0;

    [Tooltip("Headset camera inside the XR Origin")]
    public Transform xrCamera;

    [Tooltip("Optional label that shows the current room name")]
    public TMP_Text roomLabel;

    [SerializeField] private float fadeDuration = 0.4f;

    private int currentIndex;
    private bool isTransitioning;

    private void Start()
    {
        if (xrCamera == null && Camera.main != null) xrCamera = Camera.main.transform;

        currentIndex = Mathf.Clamp(startingIndex, 0, rooms.Length - 1);
        for (int i = 0; i < rooms.Length; i++) rooms[i].root.SetActive(i == currentIndex);

        CenterCurrentRoom();
        UpdateLabel();
    }

    private void Update()
    {
        CenterCurrentRoom();
    }

    private void CenterCurrentRoom()
    {
        if (xrCamera != null) rooms[currentIndex].root.transform.position = xrCamera.position;
    }

    public bool CanTravelTo(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= rooms.Length) return false;
        foreach (int connected in rooms[currentIndex].connectedRooms)
        {
            if (connected == targetIndex) return true;
        }
        return false;
    }

    public void TravelTo(int targetIndex)
    {
        if (isTransitioning || targetIndex == currentIndex) return;

        if (!CanTravelTo(targetIndex))
        {
            Debug.LogWarning($"{rooms[currentIndex].roomName} is not connected to room index {targetIndex}.");
            return;
        }

        StartCoroutine(TravelRoutine(targetIndex));
    }

    private IEnumerator TravelRoutine(int targetIndex)
    {
        isTransitioning = true;
        SceneFader fader = SceneFader.Instance;

        if (fader != null) yield return fader.FadeTo(1f, fadeDuration);

        rooms[currentIndex].root.SetActive(false);
        currentIndex = targetIndex;
        rooms[currentIndex].root.SetActive(true);
        CenterCurrentRoom();
        UpdateLabel();

        if (fader != null) yield return fader.FadeTo(0f, fadeDuration);
        isTransitioning = false;
    }

    private void UpdateLabel()
    {
        if (roomLabel != null) roomLabel.text = rooms[currentIndex].roomName;
    }
}
