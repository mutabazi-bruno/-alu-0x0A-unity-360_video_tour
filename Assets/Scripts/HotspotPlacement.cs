using UnityEngine;

[ExecuteAlways]
public class HotspotPlacement : MonoBehaviour
{
    [Range(-180f, 180f)] public float yaw;
    [Range(-60f, 60f)] public float pitch;
    [Min(0.5f)] public float distance = 3f;

    private void OnEnable() => Apply();
    private void OnValidate() => Apply();

    public void Apply()
    {
        Vector3 direction = Quaternion.Euler(-pitch, yaw, 0f) * Vector3.forward;
        transform.localPosition = direction * distance;
        transform.localRotation = Quaternion.LookRotation(direction);
    }
}
