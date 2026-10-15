using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public sealed class ThirdPersonPlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float sprintSpeed = 8f;

    [Header("Pulo e gravidade")]
    [SerializeField, Min(0f)] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedVerticalSpeed = -2f;

    [Header("Referências")]
    [Tooltip("Normalmente é a Main Camera, que contém o Cinemachine Brain.")]
    [SerializeField] private Transform cameraTransform;
    [Tooltip("FreeLook usada para controlar a câmera de terceira pessoa.")]
    [SerializeField] private CinemachineFreeLook freeLookCamera;

    [Header("Mouse")]
    [SerializeField] private bool lockCursor = true;
    [Tooltip("Inverte o eixo vertical da câmera Cinemachine: mouse para baixo faz a câmera olhar para baixo.")]
    [SerializeField] private bool invertCameraY = true;

    private CharacterController characterController;
    private InputAction sprintAction;
    private Vector2 moveInput;
    private bool jumpRequested;
    private float verticalSpeed;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput.actions == null)
        {
            Debug.LogError(
                "O Player Input precisa receber o asset PlayerControls em Actions.",
                this);
            enabled = false;
            return;
        }

        sprintAction = playerInput.actions.FindAction("Sprint");

        if (freeLookCamera != null)
        {
            freeLookCamera.m_YAxis.m_InvertInput = invertCameraY;
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

    }

    private void OnEnable()
    {
        ApplyCursorState();
    }

    private void OnDisable()
    {
        moveInput = Vector2.zero;
        jumpRequested = false;
    }

    private void Update()
    {
        Vector3 moveDirection = GetCameraRelativeDirection(moveInput);

        UpdateVerticalSpeed();

        bool sprintHeld = sprintAction != null && sprintAction.IsPressed();
        float currentMoveSpeed = sprintHeld ? sprintSpeed : moveSpeed;
        Vector3 velocity = moveDirection * currentMoveSpeed;
        velocity.y = verticalSpeed;
        characterController.Move(velocity * Time.deltaTime);
    }

    // Chamado automaticamente pelo Player Input usando o modo Send Messages.
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Chamado automaticamente pelo Player Input usando o modo Send Messages.
    public void OnSprint(InputValue value)
    {
        // O estado é consultado diretamente pela ação no Update.
    }

    // Chamado automaticamente pelo Player Input usando o modo Send Messages.
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpRequested = true;
        }
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        if (cameraTransform == null)
        {
            return new Vector3(input.x, 0f, input.y);
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = cameraForward * input.y + cameraRight * input.x;
        return Vector3.ClampMagnitude(direction, 1f);
    }

    private void UpdateVerticalSpeed()
    {
        if (characterController.isGrounded)
        {
            if (verticalSpeed < 0f)
            {
                verticalSpeed = groundedVerticalSpeed;
            }

            if (jumpRequested)
            {
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        jumpRequested = false;
        verticalSpeed += gravity * Time.deltaTime;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ApplyCursorState();
        }
    }

    private void ApplyCursorState()
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }

    private void OnValidate()
    {
        gravity = Mathf.Min(gravity, -0.01f);
        groundedVerticalSpeed = Mathf.Min(groundedVerticalSpeed, -0.01f);
    }
}
