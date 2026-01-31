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
    private float yaw;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        lookAction?.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        moveAction?.action.Disable();
        lookAction?.action.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        lookInput = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;

        yaw += lookInput.x * lookSensitivity;
        pitch = Mathf.Clamp(pitch - lookInput.y * lookSensitivity, minPitch, maxPitch);

        if (cameraPivot != null)
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
        // rb.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        if (move.sqrMagnitude > 1f) move.Normalize();

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}
