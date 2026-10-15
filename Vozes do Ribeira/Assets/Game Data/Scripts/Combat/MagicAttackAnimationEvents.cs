using UnityEngine;

[DisallowMultipleComponent]
public sealed class MagicAttackAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerAttackInput attackInput;

    private void Awake()
    {
        if (attackInput == null)
        {
            attackInput = GetComponentInParent<PlayerAttackInput>();
        }
    }

    // Chamado pelo Animation Event no frame em que a magia deixa a mão.
    public void ReleaseMagicProjectile()
    {
        if (attackInput != null)
        {
            attackInput.ReleaseMagicProjectile();
        }
    }
}
