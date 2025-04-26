using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public Vector2 wallJumpForce = new Vector2(10f, 16f);

    [Header("Movement Tuning")]
    public float acceleration = 60f;
    public float deceleration = 30f;
    public float airAcceleration = 30f;
    public float airDeceleration = 5f;

    [Header("Jump Tuning")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 10f;
    public float jumpStaminaConsumption = 5f;
    public float wallJumpStaminaConsumption = 10f;
    public float currentStamina;

    [Header("Grabbing")]
    public float boxMoveSpeed = 3f;
    public bool isGrabbing = false;
    public bool grabAttempt = false;
    public bool nearBox = false;

    [Header("Movie Mode")]
    public bool canMove = true;

    [Header("Ground & Wall Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;

    [HideInInspector]
    public bool isGrounded = false;
    private bool isOnWall = false;
    private int wallDir;

    private Rigidbody rb;
    private float inputX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;
        groundLayer = LayerMask.GetMask("Ground", "Box"); // gronden + boxes
    }

    void Update()
    {
        if (!canMove) return;

        inputX = Input.GetAxisRaw("Horizontal");
        grabAttempt = Input.GetMouseButton(1);

        UpdateGroundAndWallStatus();

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && !isGrabbing) Jump();
            else if (isOnWall && !isGrabbing) WallJump();
        }

        if (!isGrabbing && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        HandleMovement(inputX);

        ApplyGravityTweaks();
    }

    private void UpdateGroundAndWallStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        bool touchingLeft = Physics.CheckSphere(wallCheckLeft.position, wallCheckRadius, wallLayer);
        bool touchingRight = Physics.CheckSphere(wallCheckRight.position, wallCheckRadius, wallLayer);

        isOnWall = !isGrounded && (touchingLeft || touchingRight);
        wallDir = touchingLeft ? -1 : (touchingRight ? 1 : 0);
    }

    private void HandleMovement(float input)
    {
        float targetSpeed = input * (isGrabbing ? boxMoveSpeed : moveSpeed);
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = isGrounded
            ? (Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration)
            : (Mathf.Abs(targetSpeed) > 0.01f ? airAcceleration : airDeceleration);

        float movement = accelRate * Time.fixedDeltaTime;

        if (Mathf.Abs(speedDiff) <= movement)
        {
            rb.linearVelocity = new Vector3(targetSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x + Mathf.Sign(speedDiff) * movement, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }

    private void Jump()
    {
        if (currentStamina < jumpStaminaConsumption) return;
        currentStamina -= jumpStaminaConsumption;

        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false;
    }

    private void WallJump()
    {
        if (currentStamina < wallJumpStaminaConsumption) return;
        currentStamina -= wallJumpStaminaConsumption;

        isOnWall = false;
        Vector3 jumpDirection = new Vector3(-wallDir * wallJumpForce.x, wallJumpForce.y, 0);
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }

    private void ApplyGravityTweaks()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheckLeft.position, wallCheckRadius);
        }
        if (wallCheckRight != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheckRight.position, wallCheckRadius);
        }
    }
}