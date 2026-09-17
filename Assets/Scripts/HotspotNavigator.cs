using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Handles navigation between the four 360 video spheres
/// (0 = LivingRoom, 1 = Cantina, 2 = Cube, 3 = Mezzanine) with a fade to black.
/// </summary>
public class HotspotNavigator : MonoBehaviour
{
    [Tooltip("Order: 0 = LivingRoom, 1 = Cantina, 2 = Cube, 3 = Mezzanine")]
    public GameObject[] spheres;

    [Tooltip("Same order as Spheres above, one VideoPlayer per sphere")]
    public VideoPlayer[] videoPlayers;

    [Tooltip("A black UI Image used for the fade transition")]
    public Image fadeImage;

    [Tooltip("How long each fade (in or out) takes, in seconds")]
    public float fadeDuration = 0.5f;

    [Tooltip("Which sphere is shown when the scene starts")]
    public int startingIndex = 0;

    [Tooltip("The XR camera (Main Camera inside XR Origin)")]
    public Transform xrCamera;

    private int currentIndex;
    private bool isTransitioning;

    private void Start()
    {
        currentIndex = startingIndex;
        GiveEachSphereItsOwnVideo();
        SetupFadeInFrontOfEyes();

        // Only the starting sphere is visible and playing.
        for (int i = 0; i < spheres.Length; i++)
        {
            bool on = i == currentIndex;
            spheres[i].SetActive(on);
            if (on) videoPlayers[i].Play(); else videoPlayers[i].Pause();
        }
    }

    // All spheres were sharing one material (LivingRoomMaterial -> LivingRoomRT),
    // so every room showed the same video. Give each sphere its own material
    // that points at its own VideoPlayer's render texture.
    private void GiveEachSphereItsOwnVideo()
    {
        for (int i = 0; i < spheres.Length; i++)
        {
            Renderer r = spheres[i].GetComponent<Renderer>();
            VideoPlayer vp = videoPlayers[i];
            if (r == null || vp == null) continue;

            Material m = new Material(r.sharedMaterial);
            Texture tex = vp.targetTexture;
            if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", tex);
            if (m.HasProperty("_MainTex")) m.SetTexture("_MainTex", tex);
            r.material = m;
        }
    }

    // Screen Space canvases render as a small square in a headset.
    // Turn the fade canvas into a big world space panel stuck to the camera.
    private void SetupFadeInFrontOfEyes()
    {
        if (fadeImage == null || xrCamera == null) return;

        Canvas canvas = fadeImage.canvas;
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 999;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.SetParent(xrCamera, false);
        canvasRect.localPosition = new Vector3(0f, 0f, 0.3f);
        canvasRect.localRotation = Quaternion.identity;
        canvasRect.localScale = Vector3.one * 0.001f;
        canvasRect.sizeDelta = new Vector2(5000f, 5000f); // 5m x 5m at 0.3m away

        RectTransform imgRect = fadeImage.rectTransform;
        imgRect.anchorMin = Vector2.zero;
        imgRect.anchorMax = Vector2.one;
        imgRect.offsetMin = Vector2.zero;
        imgRect.offsetMax = Vector2.zero;
        imgRect.localScale = Vector3.one;

        // Never block controller rays.
        fadeImage.raycastTarget = false;
        GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
        if (gr != null) gr.enabled = false;

        Color c = Color.black;
        c.a = 0f;
        fadeImage.color = c;
    }

    private void Update()
    {
        // Keep the active sphere centered on the head.
        spheres[currentIndex].transform.position = xrCamera.position;
    }

    /// <summary>Hook a hotspot Button's OnClick to this with the destination index.</summary>
    public void TravelTo(int targetIndex)
    {
        if (targetIndex == currentIndex || isTransitioning) return;
        if (targetIndex < 0 || targetIndex >= spheres.Length) return;
        StartCoroutine(FadeAndTravel(targetIndex));
    }

    private IEnumerator FadeAndTravel(int targetIndex)
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(0f, 1f));

        HideSphere(currentIndex);
        ShowSphere(targetIndex);
        currentIndex = targetIndex;
        spheres[currentIndex].transform.position = xrCamera.position;

        // Wait for the new video's first frame so you don't see a frozen/old frame.
        VideoPlayer vp = videoPlayers[currentIndex];
        float t = 0f;
        while (!vp.isPlaying && t < 2f) { t += Time.deltaTime; yield return null; }

        yield return StartCoroutine(Fade(1f, 0f));

        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fadeImage.color = c;
    }

    private void HideSphere(int index)
    {
        videoPlayers[index].Pause();
        spheres[index].SetActive(false);
    }

    private void ShowSphere(int index)
    {
        spheres[index].SetActive(true);
        videoPlayers[index].Play();
    }
}
