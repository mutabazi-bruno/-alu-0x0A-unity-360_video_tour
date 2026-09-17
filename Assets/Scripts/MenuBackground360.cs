using UnityEngine;
using UnityEngine.Video;


[RequireComponent(typeof(MeshRenderer), typeof(VideoPlayer))]
public class MenuBackground360 : MonoBehaviour
{
    [Header("Here I can Either Use Picture Or Video")]
    public VideoClip video;
    public Texture photo;

    [Range(0f, 1f)] public float videoVolume = 0f;
    [Range(0f, 5f)] public float rotateDegreesPerSecond = 1f;
    public Color emptyColor = new Color(0.05f, 0.07f, 0.12f);

    private Material material;
    private RenderTexture videoTexture;
    private Transform head;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.playOnAwake = false;

        if (video != null)
        {
            videoTexture = new RenderTexture((int)video.width, (int)video.height, 0);
            player.source = VideoSource.VideoClip;
            player.clip = video;
            player.isLooping = true;
            player.renderMode = VideoRenderMode.RenderTexture;
            player.targetTexture = videoTexture;

            if (videoVolume > 0f)
            {
                player.audioOutputMode = VideoAudioOutputMode.Direct;
                player.SetDirectAudioVolume(0, videoVolume);
            }
            else
            {
                player.audioOutputMode = VideoAudioOutputMode.None;
            }

            ApplyTexture(videoTexture, Color.white);
            player.Play();
        }
        else if (photo != null)
        {
            ApplyTexture(photo, Color.white);
        }
        else
        {
            ApplyTexture(null, emptyColor);
        }
    }

    private void ApplyTexture(Texture texture, Color tint)
    {
        if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
        if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", texture);
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", tint);
        if (material.HasProperty("_Color")) material.SetColor("_Color", tint);
    }

    private void Update()
    {
        if (head == null && Camera.main != null) head = Camera.main.transform;
        if (head != null) transform.position = head.position;
        transform.Rotate(0f, rotateDegreesPerSecond * Time.deltaTime, 0f, Space.World);
    }

    private void OnDestroy()
    {
        if (videoTexture != null) videoTexture.Release();
    }
}
