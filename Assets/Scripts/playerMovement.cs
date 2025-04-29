using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float hideMoveSpeed = 4f; // ✅ Nieuw voor hiding
    public float hideJumpForce = 10f; // ✅ Nieuw voor hiding
    public float defaultMoveSpeed;
    public float defaultJumpForce;

    [Header("Other Settings")]
    public Vector2 wallJumpForce = new Vector2(4f, 5f);
    public float acceleration = 60f;
    public float deceleration = 30f;
    public float airAcceleration = 30f;
    public float airDeceleration = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Wall Slide Settings")]
    public float wallStickTime = 0.25f;
    public float wallSlideStartSpeed = 0f;
    public float wallSlideAcceleration = 5f;
    public float wallSlideMaxSpeed = 10f;

    [Header("Wall Jump Lock Settings")]
    public float wallJumpLockDuration = 0.25f;

    [Header("Stamina Settings")]
    public float staminaRegenRate = 10f;
    public float jumpStaminaConsumption = 5f;
    public float wallJumpStaminaConsumption = 10f;
    public float currentStamina;

    [Header("Grabbing Settings")]
    public float boxMoveSpeed = 3f;
    public bool isGrabbing = false;
    public bool grabAttempt = false;
    public bool nearBox = false;

    [Header("Control")]
    public bool canMove = true;

    [Header("Ground & Wall Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;

    [Header("Debug Settings")]
    public bool showDebug = true;

    [Header("Box movement")]
    public LayerMask boxLayer;

    [HideInInspector] public bool isGrounded = false;

    private Rigidbody rb;
    private bool isOnWall = false;
    private int wallDir;
    private float wallStickCounter;
    private float wallSlideSpeed;
    private int wallJumpLockDirection;
    private float wallJumpLockCounter;
    private float inputX;
    private bool standingOnBox = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (globalPlayerStats.instance != null)
            currentStamina = globalPlayerStats.instance.maxStamina;
        else
            currentStamina = 100f;

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;

        groundLayer = LayerMask.GetMask("Ground", "Box");
    }

    private void Update()
    {
        if (!canMove) return;

        inputX = Input.GetAxisRaw("Horizontal");

        Collider[] groundHits = Physics.OverlapSphere(groundCheck.position, groundCheckRadius);
        bool standingOnBox = false;

        foreach (Collider col in groundHits)
        {
            if (col.transform.parent != null && col.transform.parent.gameObject.layer == LayerMask.NameToLayer("Box"))
            {
                standingOnBox = true;
                break;
            }
        }

        grabAttempt = Input.GetMouseButton(1) && !standingOnBox;


        UpdateGroundAndWallStatus();
        HandleWallJumpLock();
        UpdateStamina();
        ApplyHideEffects();

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && !isGrabbing) Jump();
            else if (isOnWall && !isGrabbing) WallJump();
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        HandleMovement(inputX);
        HandleWallSlide();
        ApplyGravityTweaks();
    }

    private void UpdateGroundAndWallStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        bool touchingLeft = Physics.CheckSphere(wallCheckLeft.position, wallCheckRadius, wallLayer);
        bool touchingRight = Physics.CheckSphere(wallCheckRight.position, wallCheckRadius, wallLayer);

        isOnWall = !isGrounded && (touchingLeft || touchingRight);
        wallDir = touchingLeft ? -1 : (touchingRight ? 1 : 0);

        if (!isOnWall)
        {
            wallStickCounter = 0f;
            wallSlideSpeed = wallSlideStartSpeed;
        }
    }

    private void HandleMovement(float input)
    {
        if (wallJumpLockCounter > 0 && input == wallJumpLockDirection)
        {
            input = 0f;
        }

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

    private void HandleWallSlide()
    {
        if (isOnWall && rb.linearVelocity.y <= 0)
        {
            if (wallStickCounter < wallStickTime)
            {
                wallStickCounter += Time.fixedDeltaTime;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }
            else
            {
                wallSlideSpeed += wallSlideAcceleration * Time.fixedDeltaTime;
                wallSlideSpeed = Mathf.Min(wallSlideSpeed, wallSlideMaxSpeed);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallSlideSpeed, rb.linearVelocity.z);
            }
        }
    }

    private void HandleWallJumpLock()
    {
        if (wallJumpLockCounter > 0)
        {
            wallJumpLockCounter -= Time.deltaTime;
        }
    }

    private void UpdateStamina()
    {
        float maxStamina = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxStamina : 100f;

        if (!isGrabbing && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    private void ApplyHideEffects()
    {
        if (globalPlayerStats.instance != null && globalPlayerStats.instance.isHiding)
        {
            moveSpeed = hideMoveSpeed;
            jumpForce = hideJumpForce;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            jumpForce = defaultJumpForce;
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

        wallJumpLockDirection = wallDir;
        wallJumpLockCounter = wallJumpLockDuration;
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

    private void OnDrawGizmosSelected()
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
