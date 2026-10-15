using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Projectile Settings")]
public sealed class ProjectileSettings : ScriptableObject
{
    [SerializeField] private MagicProjectile prefab;
    [SerializeField, Min(0.01f)] private float speed = 20f;
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.01f)] private float lifetime = 5f;
    [SerializeField, Min(0f)] private float cooldown = 0.35f;
    [SerializeField, Min(0.01f)] private float radius = 0.15f;
    [SerializeField] private LayerMask hitMask = ~0;

    public MagicProjectile Prefab => prefab;
    public float Speed => Mathf.Max(0.01f, speed);
    public float Damage => Mathf.Max(0f, damage);
    public float Lifetime => Mathf.Max(0.01f, lifetime);
    public float Cooldown => Mathf.Max(0f, cooldown);
    public float Radius => Mathf.Max(0.01f, radius);
    public LayerMask HitMask => hitMask;
}
