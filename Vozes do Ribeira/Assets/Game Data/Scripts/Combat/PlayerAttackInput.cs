using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[DisallowMultipleComponent]
public sealed class PlayerAttackInput : MonoBehaviour
{
    [SerializeField] private CameraAimProvider aim;
    [SerializeField] private ProjectileCaster caster;

    // A entrada inicia a animação no PlayerAnimationController.
    // O projétil é liberado posteriormente pelo Animation Event.
    public void OnAttack(InputValue value)
    {
    }

    public void ReleaseMagicProjectile()
    {
        if (Application.isPlaying && isActiveAndEnabled && Time.timeScale > 0f &&
            aim != null && caster != null &&
            aim.TryGetAimPoint(out Vector3 point))
        {
            caster.TryCast(point);
        }
    }
}
