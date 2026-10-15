using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class ProjectileCaster : MonoBehaviour
{
    [SerializeField] private ProjectileSettings settings;
    [SerializeField] private Transform owner;
    [SerializeField] private Transform castPoint;
    [Tooltip("Point inside the caster, used to check obstacles before the muzzle.")]
    [SerializeField] private Vector3 originOffset = new Vector3(0f, 1.3f, 0f);
    [SerializeField] private UnityEvent onCast = new UnityEvent();
    private float nextCastTime;

    public bool TryCast(Vector3 target)
    {
        if (!isActiveAndEnabled || Time.timeScale <= 0f || Time.time < nextCastTime ||
            settings == null || settings.Prefab == null || castPoint == null || owner == null)
            return false;

        Vector3 direction = target - castPoint.position;
        if (direction.sqrMagnitude < 0.0001f) return false;
        MagicProjectile projectile = Instantiate(settings.Prefab, owner.TransformPoint(originOffset),
            Quaternion.LookRotation(direction));
        nextCastTime = Time.time + settings.Cooldown;
        projectile.Launch(settings, owner, direction.normalized, castPoint.position);
        onCast.Invoke();
        return true;
    }
}
