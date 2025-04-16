using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    private float moveSpeedVar;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float airControlMultiplier = 0.5f;
    public float runSpeed = 10f;
    private float runSpeedVar;
    public float runStaminaCostPerSecond = 20f;
    public float maxHorizontalSpeed = 15f;
    public float maxVerticalSpeed = 25f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f;
    public float staminaDrainRate = 20f;
    private float moveInput;
    public float jumpStaminaConsumption = 5f;
    public float wallJumpStaminaConsumption = 15f;

    [Header("Jumping Settings")]
    public float jumpForce = 12f;
    private float jumpForceVar;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Jump Settings")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallJumpLayer;
    public float wallJumpForce = 12f;
    public float wallJumpPush = 10f;
    public float wallJumpControlDelay = 1f;
    private int blockedDirection = 0;
    private bool wasTouchingWall = false;
    private bool wallReadyToJump = false;
    public float wallJumpReadyDelay = 0.5f;
    private float wallContactTime = 0f;


    [Header("Gravity Settings")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Wall-Sticking")]
    public float wallStickDuration = 1f;
    public float wallSlideSpeed = -2f;
    public float wallSlideAcceleration = 10f;
    private float wallStickTimer = 0f;
    private bool isStickingToWall = false;

    [Header("Exhaust Settings")]
    public float exhaustMoveSpeed;
    public float exhaustRunSpeed;
    public float exhaustJumpForce;
    private bool isExhausted = false;

    private Rigidbody rb;
    private bool jumpPressed;
    private bool isGrounded;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool jumpRequested = false;
    private bool isRunning = false;
    private bool isWallJumping = false;
    private float wallJumpTimer = 0f;

    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentStamina = maxStamina;
        moveSpeedVar = moveSpeed;
        runSpeedVar = runSpeed;
        jumpForceVar = jumpForce;
    }

    void Update()
    {
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
    }

    void FixedUpdate()
    {
        handleMovement();
        handleJump();
        applyExtraGravity();
        handleWallStick();
        handleStamina();
    }

    void handleMovement()
    {
        float baseSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeedVar : moveSpeedVar;
        float desiredVelocity = moveInput * baseSpeed;

        if (isWallJumping && !isGrounded)
        {
            if ((blockedDirection == -1 && moveInput < 0f) || (blockedDirection == 1 && moveInput > 0f))
            {
                desiredVelocity = 0f;
            }
        }

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
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else if (isGrounded && !Input.GetKey(KeyCode.LeftShift))
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        if (currentStamina <= 2f)
        {
            Debug.Log("Speler is uitgeput!");
            isExhausted = true;
            moveSpeedVar = exhaustMoveSpeed;
            runSpeedVar = exhaustRunSpeed;
            jumpForceVar = exhaustJumpForce;
        }
        else if (currentStamina >= 20f && currentStamina <= 25f)
        {
            Debug.Log("Speler is hersteld van uitputting!");
            isExhausted = false;
            moveSpeedVar = moveSpeed;
            runSpeedVar = runSpeed;
            jumpForceVar = jumpForce;
        }
    }

    void handleJump()
    {
        if (!jumpRequested) return;

        if ((isTouchingWallLeft || isTouchingWallRight) && !isExhausted && wallReadyToJump)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);

            Vector3 jumpDirection = Vector3.up;

            if (isTouchingWallLeft)
            {
                jumpDirection += Vector3.right * wallJumpPush;
                blockedDirection = -1;
            }
            else if (isTouchingWallRight)
            {
                jumpDirection += Vector3.left * wallJumpPush;
                blockedDirection = 1;
            }

            rb.linearVelocity = new Vector3(jumpDirection.x, wallJumpForce, 0f);
            isWallJumping = true;
            wallJumpTimer = wallJumpControlDelay;
            currentStamina -= wallJumpStaminaConsumption;
            Debug.Log(currentStamina);
        }
        else if (isGrounded)
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
        else if (!Input.GetButton("Jump") && rb.linearVelocity.y > 0 && !isGrounded)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    void handleWallStick()
    {
        bool touchingWall = isTouchingWallLeft || isTouchingWallRight;
        bool falling = !isGrounded && rb.linearVelocity.y <= 0f;

        if (touchingWall && falling)
        {
            // Zet timer alleen als we net beginnen te plakken
            if (!isStickingToWall)
            {
                isStickingToWall = true;
                wallStickTimer = wallStickDuration;
                wallContactTime = 0f;
                wallReadyToJump = false;
            }

            // Tel hoe lang speler aan muur plakt
            wallContactTime += Time.deltaTime;
            if (wallContactTime >= wallJumpReadyDelay)
            {
                wallReadyToJump = true;
            }

            // Stick timer zorgt ervoor dat speler blijft plakken
            if (wallStickTimer > 0f)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f);
                wallStickTimer -= Time.deltaTime;
            }
            else
            {
                // Begin met glijden na plaktijd
                float newY = rb.linearVelocity.y - wallSlideAcceleration * Time.deltaTime;
                newY = Mathf.Clamp(newY, -maxVerticalSpeed, 0f);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, newY, 0f);
            }
        }
        else
        {
            isStickingToWall = false;
            wallContactTime = 0f;
            wallReadyToJump = false;
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
