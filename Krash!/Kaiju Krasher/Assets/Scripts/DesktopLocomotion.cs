using UnityEngine;
using UnityEngine.XR;

public class DesktopLocomotion : MonoBehaviour
{
    [Header("References")]
    public Transform moveRoot;
    public Camera lookCamera;
    public CharacterController characterController;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float sprintMultiplier = 2f;
    public float mouseSensitivity = 2f;
    public bool invertY = false;

    [Header("Options")]
    public KeyCode toggleKey = KeyCode.F1;
    public bool startEnabled = true;
    public bool onlyWhenNoXR = true;

    private bool isEnabled;
    private float yaw;
    private float pitch;
    private bool cursorLocked;

    void Start()
    {
        if (moveRoot == null)
            moveRoot = transform;

        if (lookCamera == null)
            lookCamera = Camera.main;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        isEnabled = startEnabled;

        if (onlyWhenNoXR && XRSettings.isDeviceActive)
            isEnabled = false;

        UpdateCursorState();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isEnabled = !isEnabled;
            UpdateCursorState();
        }

        if (!isEnabled) return;

        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        if (!cursorLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? 1f : -1f);

        yaw += mouseX;
        pitch = Mathf.Clamp(pitch + mouseY, -85f, 85f);

        moveRoot.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (lookCamera != null)
            lookCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = (moveRoot.forward * v + moveRoot.right * h).normalized;
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

        Vector3 motion = move * speed * Time.deltaTime;

        // Use CharacterController so collisions register
        if (characterController != null)
            characterController.Move(motion);
        else
            moveRoot.position += motion;
    }

    void UpdateCursorState()
    {
        cursorLocked = isEnabled;
        Cursor.lockState = isEnabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isEnabled;
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}