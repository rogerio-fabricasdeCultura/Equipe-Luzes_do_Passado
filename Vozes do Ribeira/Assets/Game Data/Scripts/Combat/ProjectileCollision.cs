using UnityEngine;

internal static class ProjectileCollision
{
    public static bool IsOwner(Collider collider, Transform owner)
    {
        return owner != null && (collider.transform == owner || collider.transform.IsChildOf(owner));
    }
}
