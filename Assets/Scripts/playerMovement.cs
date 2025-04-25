using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    private float moveSpeedVar;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float runSpeed = 10f;
    private float runSpeedVar;
    public float maxHorizontalSpeed = 15f;
    public float maxVerticalSpeed = 25f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f;
    public float staminaDrainRate = 20f;
    public float jumpStaminaConsumption = 5f;
    public float wallJumpStaminaConsumption = 15f;

    [Header("Jumping Settings")]
    public float jumpForce = 12f;
    private float jumpForceVar;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float maxFallSpeed = 10f;

    [Header("Wall Jump Settings")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallJumpLayer;
    public float wallJumpForce = 12f;
    public float wallJumpPush = 10f;
    public float wallJumpControlDelay = 1f;
    private int blockedDirection = 0;
    private float wallJumpHorizontalForce = 7f;
    private float wallJumpControlLock = 0.2f;
    private float wallJumpControlTimer = 0f;
    private float wallJumpDirection = 0f;

    [Header("Gravity Settings")]
    public float fallMultiplier = 4f;

    [Header("Wall-Sticking")]
    public float wallStickDuration = 1f;
    public float wallSlideAcceleration = 10f;
    private float wallStickTimer = 0f;
    private bool isStickingToWall = false;

    [Header("Exhaust Settings")]
    public float exhaustMoveSpeed = 3f;
    public float exhaustRunSpeed = 4f;
    public float exhaustJumpForce = 6f;
    private bool isExhausted = false;

    [Header("Box Grabbing")]
    public float boxMoveSpeed = 3f;
    [HideInInspector] public bool isGrabbing = false;
    [HideInInspector] public bool grabAttempt = false;
    [HideInInspector] public bool nearBox = false;
    private Transform grabbedBox;
    private Collider grabbedBoxCollider;

    [Header("Movie Mode")]
    public bool canMove = true;

    private Rigidbody rb;
    private float moveInput;
    private bool jumpPressed;
    public bool isGrounded;
    public bool isTouchingWallLeft;
    public bool isTouchingWallRight;
    private bool jumpRequested = false;
    private bool isRunning = false;
    private bool isWallJumping = false;
    private float wallJumpTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;
        moveSpeedVar = moveSpeed;
        runSpeedVar = runSpeed;
        jumpForceVar = jumpForce;
        groundLayer = LayerMask.GetMask("Ground", "Box");
    }

    void Update()
    {
        if (!canMove) return;

        if (!isWallJumping)
            moveInput = Input.GetAxisRaw("Horizontal");

        jumpPressed = Input.GetButtonDown("Jump");

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && blockedDirection != 0)
        {
            blockedDirection = 0;
            isWallJumping = false;
        }

        isTouchingWallLeft = Physics.CheckSphere(wallCheckLeft.position, wallCheckRadius, wallJumpLayer);
        isTouchingWallRight = Physics.CheckSphere(wallCheckRight.position, wallCheckRadius, wallJumpLayer);

        if (jumpPressed && (isGrounded || isTouchingWallLeft || isTouchingWallRight))
        {
            jumpRequested = true;
        }

        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0f)
                isWallJumping = false;
        }

        grabAttempt = Input.GetMouseButton(1);
    }

    void FixedUpdate()
    {
        if (!canMove) return;

        if (!isGrabbing)
            handleMovement();
        else
            handleBoxMovement();

        handleJump();
        applyExtraGravity();
        handleWallStick();
        handleStamina();
    }

    void handleMovement()
    {
        float baseSpeed = (Input.GetKey(KeyCode.LeftShift) ? runSpeedVar : moveSpeedVar);

        if (wallJumpControlTimer > 0f)
        {
            wallJumpControlTimer -= Time.fixedDeltaTime;
            moveInput = wallJumpDirection;
        }

        float desiredVelocity = moveInput * baseSpeed;
        float speedDiff = desiredVelocity - rb.linearVelocity.x;
        float accelRate = Mathf.Abs(desiredVelocity) > 0.01f ? acceleration : deceleration;
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;

        Vector3 origin = transform.position;
        float direction = Mathf.Sign(moveInput);
        float checkDistance = 0.6f;
        Vector3 checkDir = Vector3.right * direction;

        if (moveInput != 0f && Physics.Raycast(origin, checkDir, checkDistance, groundLayer))
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x + movement, rb.linearVelocity.y, 0f);
        }
    }

    void handleBoxMovement()
    {
        float desiredVelocity = moveInput * boxMoveSpeed;
        float speedDiff = desiredVelocity - rb.linearVelocity.x;
        float accelRate = Mathf.Abs(desiredVelocity) > 0.01f ? acceleration : deceleration;
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x + movement, rb.linearVelocity.y, 0f);
    }

    void handleStamina()
    {
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(moveInput) > 0.1f && isGrounded;
        bool canSprint = currentStamina > 1f;
        isRunning = wantsToRun && canSprint;

        if (isRunning)
            currentStamina -= staminaDrainRate * Time.deltaTime;
        else if (isGrounded && !Input.GetKey(KeyCode.LeftShift))
            currentStamina += staminaRegenRate * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (currentStamina <= 2f)
        {
            isExhausted = true;
            moveSpeedVar = exhaustMoveSpeed;
            runSpeedVar = exhaustRunSpeed;
            jumpForceVar = exhaustJumpForce;
        }
        else if (currentStamina >= 20f && isExhausted)
        {
            isExhausted = false;
            moveSpeedVar = moveSpeed;
            runSpeedVar = runSpeed;
            jumpForceVar = jumpForce;
        }
    }

    void handleJump()
    {
        if (!jumpRequested) return;

        if ((isTouchingWallLeft || isTouchingWallRight) && !isExhausted && !isGrabbing)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            Vector3 jumpDirection = Vector3.up;

            if (isTouchingWallLeft)
            {
                jumpDirection += Vector3.right;
                blockedDirection = -1;
                wallJumpDirection = 1f;
            }
            else if (isTouchingWallRight)
            {
                jumpDirection += Vector3.left;
                blockedDirection = 1;
                wallJumpDirection = -1f;
            }

            rb.linearVelocity = new Vector3(jumpDirection.x * wallJumpHorizontalForce, wallJumpForce, 0f);
            isWallJumping = true;
            wallJumpTimer = wallJumpControlDelay;
            wallJumpControlTimer = wallJumpControlLock;
            currentStamina -= wallJumpStaminaConsumption;
        }
        else if (isGrounded && !isGrabbing)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForceVar, 0f);
            blockedDirection = 0;
            currentStamina -= jumpStaminaConsumption;
        }

        jumpRequested = false;
    }

    void applyExtraGravity()
    {
        if (isStickingToWall) return;

        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    void handleWallStick()
    {
        bool touchingWall = isTouchingWallLeft || isTouchingWallRight;
        bool falling = !isGrounded && rb.linearVelocity.y <= 0f;

        if (touchingWall && falling)
        {
            if (!isStickingToWall)
            {
                isStickingToWall = true;
                wallStickTimer = wallStickDuration;
            }

            if (wallStickTimer > 0f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
                wallStickTimer -= Time.deltaTime;
            }
            else
            {
                float newY = rb.linearVelocity.y - wallSlideAcceleration * Time.deltaTime;
                newY = Mathf.Clamp(newY, -maxVerticalSpeed, 0f);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, newY, 0f);
            }
        }
        else
        {
            isStickingToWall = false;
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
