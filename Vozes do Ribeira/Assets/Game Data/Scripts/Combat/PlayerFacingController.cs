using UnityEngine;

// Run after the camera has resolved its position for this frame.
[DefaultExecutionOrder(9000)]
[DisallowMultipleComponent]
public sealed class PlayerFacingController : MonoBehaviour
{
    [SerializeField] private CameraAimProvider aim;
    [SerializeField] private Transform visual;
    [SerializeField, Min(0f)] private float smoothTime = 0.12f;
    private float rotationVelocity;

    private void LateUpdate()
    {
        if (aim == null || visual == null || Time.timeScale <= 0f) return;
        Vector3 forward = aim.Forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f) return;
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        float rotation = Mathf.SmoothDampAngle(visual.eulerAngles.y, angle, ref rotationVelocity, smoothTime);
        visual.rotation = Quaternion.Euler(0f, rotation, 0f);
    }
}
