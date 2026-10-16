using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 12f;


    public float gravity = -20f;

    [Header("References")]
    public Transform cameraTransform;
    public IsometricCamera cameraController; // drag Player Cam here

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;

        // make the cursor invisible
        Cursor.visible = false;
    }

    void Update()
    {
        // Escape frees the cursor
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Clicking in the game locks it again
        if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        // Get WASD input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(horizontal, 0f, vertical);

        // Don't make diagonal movement faster
        if (input.magnitude > 1f)
            input.Normalize();

        // Camera directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ignore camera's vertical angle
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Movement based on the camera
        Vector3 moveDirection =
            cameraForward * vertical +
            cameraRight * horizontal;

        // Rotate player toward movement direction, but only when not locked on
        // (FaceTarget on the camera handles rotation while locked on)
        bool lockedOn = cameraController != null && cameraController.isLocked;

        if (moveDirection.magnitude > 0.1f && !lockedOn)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Move
        controller.Move(
            moveDirection.normalized *
            moveSpeed *
            Time.deltaTime
        );

        // Ground
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }



        // Gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}