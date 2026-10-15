using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class MagicProjectile : MonoBehaviour
{
    [SerializeField] private UnityEvent onImpact = new UnityEvent();
    private Transform owner;
    private Vector3 direction;
    private float speed;
    private float damage;
    private float radius;
    private float remainingLife;
    private LayerMask hitMask;
    private bool flying;

    public void Launch(ProjectileSettings settings, Transform source, Vector3 heading, Vector3 muzzlePosition)
    {
        owner = source;
        direction = heading.normalized;
        speed = settings.Speed;
        damage = settings.Damage;
        radius = settings.Radius;
        remainingLife = settings.Lifetime;
        hitMask = settings.HitMask;
        flying = true;
        // Check initial overlap and the whole path from the body to the muzzle.
        MoveTo(muzzlePosition);
    }

    private void FixedUpdate()
    {
        if (!flying) return;
        float step = Mathf.Min(Time.fixedDeltaTime, remainingLife);
        MoveTo(transform.position + direction * speed * step);
        remainingLife -= step;
        if (flying && remainingLife <= 0f)
        {
            flying = false;
            Destroy(gameObject);
        }
    }

    private bool CanHit(Collider other)
    {
        return !ProjectileCollision.IsOwner(other, owner) &&
            other.transform != transform && !other.transform.IsChildOf(transform);
    }

    private void MoveTo(Vector3 destination)
    {
        Vector3 start = transform.position;
        foreach (Collider other in Physics.OverlapSphere(start, radius, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (!CanHit(other)) continue;
            Impact(other);
            return;
        }

        Vector3 delta = destination - start;
        float distance = delta.magnitude;
        if (distance <= 0.00001f) return;
        RaycastHit nearest = default;
        bool found = false;
        foreach (RaycastHit hit in Physics.SphereCastAll(start, radius, delta / distance,
            distance, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (!CanHit(hit.collider) || (found && hit.distance >= nearest.distance)) continue;
            nearest = hit;
            found = true;
        }

        if (found)
        {
            transform.position = start + delta / distance * nearest.distance;
            Impact(nearest.collider);
        }
        else transform.position = destination;
    }

    private void Impact(Collider other)
    {
        if (!flying) return;
        flying = false;
        IDamageable target = other.GetComponentInParent<IDamageable>();
        target?.TakeDamage(damage, owner != null ? owner.gameObject : null);
        onImpact.Invoke();
        Destroy(gameObject);
    }
}
