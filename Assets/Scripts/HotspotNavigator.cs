using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class HotspotNavigator : MonoBehaviour
{
    [Tooltip("Room spheres in order: LivingRoom, Cantina, Cube, Mezzanine")]
    public GameObject[] spheres;

    [Tooltip("One VideoPlayer per sphere, same order as above")]
    public VideoPlayer[] videoPlayers;

    [Tooltip("Headset camera inside the XR Origin")]
    public Transform xrCamera;

    public int startingIndex = 0;

    [SerializeField] private float fadeDuration = 0.4f;

    private int currentIndex;
    private bool isTransitioning;

    private void Start()
    {
        if (xrCamera == null && Camera.main != null) xrCamera = Camera.main.transform;

        currentIndex = Mathf.Clamp(startingIndex, 0, spheres.Length - 1);
        AssignVideoTextures();

        for (int i = 0; i < spheres.Length; i++)
        {
            bool active = i == currentIndex;
            spheres[i].SetActive(active);
            if (active) videoPlayers[i].Play();
            else videoPlayers[i].Pause();
        }
    }

    private void AssignVideoTextures()
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            Renderer sphereRenderer = spheres[i].GetComponent<Renderer>();
            VideoPlayer player = videoPlayers[i];
            if (sphereRenderer == null || player == null) continue;

            Material material = new Material(sphereRenderer.sharedMaterial);
            Texture texture = player.targetTexture;
            if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
            if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", texture);
            sphereRenderer.material = material;
        }
    }

    private void Update()
    {
        if (xrCamera != null) spheres[currentIndex].transform.position = xrCamera.position;
    }

    public void TravelTo(int targetIndex)
    {
        if (isTransitioning || targetIndex == currentIndex) return;
        if (targetIndex < 0 || targetIndex >= spheres.Length) return;
        StartCoroutine(TravelRoutine(targetIndex));
    }

    private IEnumerator TravelRoutine(int targetIndex)
    {
        isTransitioning = true;
        SceneFader fader = SceneFader.Instance;

        if (fader != null) yield return fader.FadeTo(1f, fadeDuration);

        videoPlayers[currentIndex].Pause();
        spheres[currentIndex].SetActive(false);

        currentIndex = targetIndex;
        spheres[currentIndex].SetActive(true);
        videoPlayers[currentIndex].Play();
        if (xrCamera != null) spheres[currentIndex].transform.position = xrCamera.position;

        float waited = 0f;
        VideoPlayer player = videoPlayers[currentIndex];
        while (!player.isPlaying && waited < 2f)
        {
            waited += Time.unscaledDeltaTime;
            yield return null;
        }

        if (fader != null) yield return fader.FadeTo(0f, fadeDuration);
        isTransitioning = false;
    }
}
