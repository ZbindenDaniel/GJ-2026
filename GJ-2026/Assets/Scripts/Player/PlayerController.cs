using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputActionReference moveAction;

    [Header("Look")]
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private Transform cameraPivot;

    private Rigidbody rb;
    private float pitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }

        if (lookAction != null)
        {
            lookAction.action.Enable();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (lookAction != null)
        {
            lookAction.action.Disable();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Vector2 look = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;
        float yaw = look.x * lookSensitivity;
        float pitchDelta = -look.y * lookSensitivity;

        if (Mathf.Abs(yaw) > 0f)
        {
            transform.Rotate(0f, yaw, 0f, Space.World);
        }

        pitch = Mathf.Clamp(pitch + pitchDelta, minPitch, maxPitch);

        if (cameraPivot != null)
        {
            cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        Vector3 move = (transform.forward * moveInput.y + transform.right * moveInput.x);

        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        Vector3 targetPosition = rb.position + move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }
}
