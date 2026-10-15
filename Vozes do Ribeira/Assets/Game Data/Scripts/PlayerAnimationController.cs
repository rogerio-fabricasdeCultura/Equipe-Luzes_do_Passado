using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public sealed class PlayerAnimationController : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Animator do modelo. Se não for informado, será procurado nos filhos deste objeto.")]
    [SerializeField] private Animator animator;

    [Header("Transições")]
    [SerializeField, Min(0f)] private float dampingTime = 0.1f;
    [Tooltip("Deixe desmarcado quando o CharacterController for responsável pelo deslocamento.")]
    [SerializeField] private bool useRootMotion;

    [Header("Parâmetros do Animator")]
    [SerializeField] private string horizontalParameter = "Horizontal";
    [SerializeField] private string verticalParameter = "Vertical";
    [SerializeField] private string moveParameter = "Move";
    [SerializeField] private string sprintParameter = "Sprint";
    [SerializeField] private string groundedParameter = "Grounded";
    [SerializeField] private string jumpParameter = "Jump";
    [SerializeField] private string attackParameter = "Attack";

    private CharacterController characterController;
    private InputAction sprintAction;
    private Vector2 moveInput;

    private int horizontalHash;
    private int verticalHash;
    private int moveHash;
    private int sprintHash;
    private int groundedHash;
    private int jumpHash;
    private int attackHash;

    private bool hasHorizontal;
    private bool hasVertical;
    private bool hasMove;
    private bool hasSprint;
    private bool hasGrounded;
    private bool hasJump;
    private bool hasAttack;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput.actions != null)
        {
            sprintAction = playerInput.actions.FindAction("Sprint");
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError(
                "PlayerAnimationController não encontrou um Animator neste objeto ou em seus filhos.",
                this);
            enabled = false;
            return;
        }

        animator.applyRootMotion = useRootMotion;
        CacheAnimatorParameters();
    }

    private void Update()
    {
        Vector2 normalizedInput = Vector2.ClampMagnitude(moveInput, 1f);
        float deltaTime = Time.deltaTime;

        if (hasHorizontal)
        {
            animator.SetFloat(horizontalHash, normalizedInput.x, dampingTime, deltaTime);
        }

        if (hasVertical)
        {
            animator.SetFloat(verticalHash, normalizedInput.y, dampingTime, deltaTime);
        }

        if (hasMove)
        {
            animator.SetFloat(moveHash, normalizedInput.magnitude, dampingTime, deltaTime);
        }

        if (hasSprint)
        {
            bool sprintHeld = sprintAction != null && sprintAction.IsPressed();
            animator.SetBool(sprintHash, sprintHeld && normalizedInput.sqrMagnitude > 0.001f);
        }

        if (hasGrounded)
        {
            animator.SetBool(groundedHash, characterController.isGrounded);
        }
    }

    // Chamado pelo Player Input quando o comportamento está em Send Messages.
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Chamado pelo Player Input quando o comportamento está em Send Messages.
    public void OnSprint(InputValue value)
    {
        // O estado é consultado diretamente pela ação no Update.
    }

    // Dispara a animação somente quando o mesmo salto pode começar no chão.
    public void OnJump(InputValue value)
    {
        if (value.isPressed && characterController.isGrounded && hasJump)
        {
            animator.SetTrigger(jumpHash);
        }
    }

    // Aciona a layer de ataque sem interromper a Blend Tree de locomoção.
    public void OnAttack(InputValue value)
    {
        if (value.isPressed && hasAttack)
        {
            animator.SetTrigger(attackHash);
        }
    }

    private void OnDisable()
    {
        moveInput = Vector2.zero;
    }

    private void CacheAnimatorParameters()
    {
        horizontalHash = Animator.StringToHash(horizontalParameter);
        verticalHash = Animator.StringToHash(verticalParameter);
        moveHash = Animator.StringToHash(moveParameter);
        sprintHash = Animator.StringToHash(sprintParameter);
        groundedHash = Animator.StringToHash(groundedParameter);
        jumpHash = Animator.StringToHash(jumpParameter);
        attackHash = Animator.StringToHash(attackParameter);

        hasHorizontal = HasParameter(horizontalParameter, AnimatorControllerParameterType.Float);
        hasVertical = HasParameter(verticalParameter, AnimatorControllerParameterType.Float);
        hasMove = HasParameter(moveParameter, AnimatorControllerParameterType.Float);
        hasSprint = HasParameter(sprintParameter, AnimatorControllerParameterType.Bool);
        hasGrounded = HasParameter(groundedParameter, AnimatorControllerParameterType.Bool);
        hasJump = HasParameter(jumpParameter, AnimatorControllerParameterType.Trigger);
        hasAttack = HasParameter(attackParameter, AnimatorControllerParameterType.Trigger);

        if (!hasHorizontal || !hasVertical)
        {
            Debug.LogWarning(
                "A Blend Tree precisa dos parâmetros Float Horizontal e Vertical para receber o movimento direcional.",
                this);
        }
    }

    private bool HasParameter(string parameterName, AnimatorControllerParameterType expectedType)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == expectedType)
            {
                return true;
            }
        }

        return false;
    }

    private void OnValidate()
    {
        dampingTime = Mathf.Max(0f, dampingTime);
    }
}
