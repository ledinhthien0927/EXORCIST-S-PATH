using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementMobile : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float gravity = -20f;

    [Header("Button State")]
    public bool upPressed;
    public bool downPressed;
    public bool leftPressed;
    public bool rightPressed;

    private CharacterController controller;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float x = 0f;
        float z = 0f;

        if (leftPressed) x -= 1f;
        if (rightPressed) x += 1f;
        if (upPressed) z += 1f;
        if (downPressed) z -= 1f;

        Vector3 move = (transform.right * x + transform.forward * z).normalized;
        Vector3 finalMove = move * moveSpeed;

        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        finalMove.y = velocity.y;

        controller.Move(finalMove * Time.deltaTime);
    }

    public void SetUp(bool value) => upPressed = value;
    public void SetDown(bool value) => downPressed = value;
    public void SetLeft(bool value) => leftPressed = value;
    public void SetRight(bool value) => rightPressed = value;
}