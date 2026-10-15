using UnityEngine;

[DisallowMultipleComponent]
public sealed class CameraAimProvider : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [Tooltip("Posição normalizada da mira: (0.5, 0.5) é o centro. X cresce para a direita e Y para cima, entre 0 e 1.")]
    [SerializeField] private Vector2 aimViewportPosition = new Vector2(0.5f, 0.5f);
    [SerializeField] private Transform owner;
    [SerializeField, Min(1f)] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitMask = ~0;

    public Vector3 Forward => aimCamera != null ? aimCamera.transform.forward : transform.forward;

    private Vector3 ViewportPosition => new Vector3(
        Mathf.Clamp01(aimViewportPosition.x), Mathf.Clamp01(aimViewportPosition.y), 0f);

    public bool TryGetScreenPosition(out Vector2 position)
    {
        position = default;
        if (aimCamera == null) return false;
        position = aimCamera.ViewportToScreenPoint(ViewportPosition);
        return true;
    }

    private void OnValidate()
    {
        aimViewportPosition.x = Mathf.Clamp01(aimViewportPosition.x);
        aimViewportPosition.y = Mathf.Clamp01(aimViewportPosition.y);
    }

    public bool TryGetAimPoint(out Vector3 point)
    {
        point = default;
        if (aimCamera == null) return false;
        Ray ray = aimCamera.ViewportPointToRay(ViewportPosition);
        point = ray.GetPoint(maxDistance);
        float nearest = maxDistance;
        foreach (RaycastHit hit in Physics.RaycastAll(ray, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (ProjectileCollision.IsOwner(hit.collider, owner) || hit.distance >= nearest) continue;
            nearest = hit.distance;
            point = hit.point;
        }
        return true;
    }
}
