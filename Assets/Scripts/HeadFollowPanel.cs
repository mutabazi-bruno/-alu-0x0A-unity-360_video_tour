using UnityEngine;

// Keeps a UI panel in front of the user. It stays still while you look around
// and only drifts back into view once you've turned far away from it.
public class HeadFollowPanel : MonoBehaviour
{
    public float distance = 1.2f;
    public float heightBelowEyes = 0.55f;
    public float tiltDegrees = 30f;
    public float recenterAngle = 60f;
    public float turnSpeed = 90f;

    private Transform head;
    private float yaw;
    private bool placed;
    private bool recentering;

    private void LateUpdate()
    {
        if (head == null)
        {
            if (Camera.main == null) return;
            head = Camera.main.transform;
        }

        float headYaw = head.eulerAngles.y;

        if (!placed)
        {
            yaw = headYaw;
            placed = true;
        }

        float offset = Mathf.Abs(Mathf.DeltaAngle(yaw, headYaw));
        if (offset > recenterAngle) recentering = true;

        if (recentering)
        {
            yaw = Mathf.MoveTowardsAngle(yaw, headYaw, turnSpeed * Time.deltaTime);
            if (offset < 2f) recentering = false;
        }

        Quaternion facing = Quaternion.Euler(0f, yaw, 0f);
        transform.position = head.position + facing * Vector3.forward * distance + Vector3.down * heightBelowEyes;
        transform.rotation = facing * Quaternion.Euler(tiltDegrees, 0f, 0f);
    }
}
