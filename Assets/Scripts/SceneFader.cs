using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [SerializeField] private float defaultDuration = 0.5f;

    private Canvas canvas;
    private Image blackImage;
    private Transform cameraTransform;
    private Coroutine fadeRoutine;
    private bool isLoading;

    public float DefaultDuration => defaultDuration;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateOnStartup()
    {
        if (Instance != null) return;
        new GameObject("SceneFader").AddComponent<SceneFader>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildOverlay();
        SetAlpha(1f);
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void BuildOverlay()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = short.MaxValue;

        var rect = (RectTransform)transform;
        rect.sizeDelta = new Vector2(4000f, 4000f);
        rect.localScale = Vector3.one * 0.001f;

        var imageObject = new GameObject("Black", typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(transform, false);
        var imageRect = (RectTransform)imageObject.transform;
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        blackImage = imageObject.GetComponent<Image>();
        blackImage.color = Color.black;
        blackImage.raycastTarget = false;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cameraTransform = null;
        StartFade(0f, defaultDuration);
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            Camera main = Camera.main;
            if (main == null) return;
            cameraTransform = main.transform;
            canvas.worldCamera = main;
        }

        transform.SetPositionAndRotation(
            cameraTransform.position + cameraTransform.forward * 0.25f,
            cameraTransform.rotation);
    }

    // Fades to black, loads the scene, and the new scene fades back in.
    public static void LoadScene(string sceneName)
    {
        if (Instance == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }
        Instance.BeginLoad(sceneName);
    }

    private void BeginLoad(string sceneName)
    {
        if (isLoading) return;

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not in the build scene list. Add it in File > Build Profiles > Scene List.");
            return;
        }

        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        isLoading = true;
        yield return FadeTo(1f, defaultDuration);

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone) yield return null;

        isLoading = false;
    }

    // Can be yielded from other coroutines, e.g. room-to-room transitions.
    public IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = null;

        float startAlpha = blackImage.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeTo(targetAlpha, duration));
    }

    private void SetAlpha(float alpha)
    {
        Color color = blackImage.color;
        color.a = alpha;
        blackImage.color = color;
        canvas.enabled = alpha > 0.001f;
    }
}
