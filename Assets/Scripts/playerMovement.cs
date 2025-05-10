using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float hideMoveSpeed = 4f;
    public float hideJumpForce = 10f;

    [Header("Observed Movement")]
    public float observedMoveSpeed = 6f;
    public float observedJumpForce = 12f;

    [Header("Default Movement")]
    public float defaultMoveSpeed;
    public float defaultJumpForce;

    [Header("Other Settings")]
    public Vector2 wallJumpForce = new Vector2(4f, 5f);
    public float acceleration = 60f;
    public float deceleration = 30f;
    public float airAcceleration = 30f;
    public float airDeceleration = 5f;
    public float fallMultiplier = 2.5f;

    [Header("Sprint Settings")]
    public float sprintSpeed = 12f;
    public float sprintStaminaConsumption = 15f;
    private bool isSprinting = false;

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

    [Header("Box movement")]
    public LayerMask boxLayer;
    public Transform grabHitbox;

    [HideInInspector] public bool isGrounded = false;

    private Rigidbody rb;
    private bool isOnWall = false;
    private int wallDir;
    private float wallStickCounter;
    private float wallSlideSpeed;
    private int wallJumpLockDirection;
    private float wallJumpLockCounter;
    private float inputX;

    [Header("Animation")]
    public Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (globalPlayerStats.instance != null)
        {
            currentStamina = globalPlayerStats.instance.maxStamina;
            StartCoroutine(SetPlayerToCheckpoint());
        }
        else
        {
            currentStamina = 100f;
        }

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;

        groundLayer = LayerMask.GetMask("Ground", "Box");
    }

    private System.Collections.IEnumerator SetPlayerToCheckpoint()
    {
        yield return null;

        if (globalPlayerStats.instance != null)
        {
            Debug.Log($"[Player] Teleporteert naar checkpoint: {globalPlayerStats.instance.lastCheckpointPosition}");
            rb.MovePosition(globalPlayerStats.instance.lastCheckpointPosition);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if (!canMove || pauseMenuManager.isGamePaused)
        {
            bool blockedKey =
                Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow);

            if (blockedKey)
            {
                return;
            }
        }

        inputX = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("velocityX", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("velocityY", rb.linearVelocity.y);

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

        bool canGrabBox = false;
        if (grabHitbox != null)
        {
            Collider[] grabHits = Physics.OverlapSphere(grabHitbox.position, 0.1f, LayerMask.GetMask("Box", "coverWall", "NonSolid"));
            canGrabBox = grabHits.Length > 0;
            Debug.Log(grabHits);
        }
        grabAttempt = Input.GetMouseButton(1) && !standingOnBox && canGrabBox;

        UpdateGroundAndWallStatus();
        HandleWallJumpLock();
        UpdateStamina();
        ApplyStateEffects();

        if (Input.GetButtonDown("Jump") && canMove)
        {
            if (isGrabbing)
            {
                // Zoek een BoxGrabTrigger en forceer loslaten
                BoxGrabTrigger grabTrigger = GetComponentInChildren<BoxGrabTrigger>();
                if (grabTrigger != null)
                {
                    grabTrigger.ForceDetachBox();
                }
                isGrabbing = false;
                Debug.Log("Jump while grabbing: Forced box release.");
            }

            if (isGrounded && !isGrabbing)
            {
                Jump();
            }
            else if (isOnWall && !isGrabbing)
            {
                inputX = 0f;
                WallJump();
            }

            animator.SetBool("jump", true);
        }
        else if (rb.linearVelocity.y < 0)
        {
            animator.SetBool("jump", false);
        }

        bool canSprint = globalPlayerStats.instance != null && globalPlayerStats.instance.isHiding;

        if (Input.GetKey(KeyCode.LeftShift) && inputX != 0 && !isGrabbing && currentStamina > 0f && isGrounded && canSprint)
        {
            if (!isSprinting && currentStamina > 20f)
            {
                isSprinting = true;
            }
        }
        else
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            currentStamina -= sprintStaminaConsumption * Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isSprinting = false;
            }
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
        animator.SetBool("isGrounded", isGrounded);

        bool touchingLeft = Physics.CheckSphere(wallCheckLeft.position, wallCheckRadius, wallLayer);
        bool touchingRight = Physics.CheckSphere(wallCheckRight.position, wallCheckRadius, wallLayer);

        bool wasOnWall = isOnWall;
        isOnWall = !isGrounded && (touchingLeft || touchingRight);
        wallDir = touchingLeft ? -1 : (touchingRight ? 1 : 0);

        if (!isOnWall)
        {
            wallStickCounter = 0f;
            wallSlideSpeed = wallSlideStartSpeed;
            animator.SetBool("onWall", false);
        }
        else
        {
            if (!wasOnWall) wallStickCounter = 0f;
            animator.SetBool("onWall", true);
        }
    }

    private void HandleMovement(float input)
    {
        if (wallJumpLockCounter > 0 && input == wallJumpLockDirection)
        {
            input = 0f;
        }

        float usedSpeed = isSprinting ? sprintSpeed : moveSpeed;
        float targetSpeed;

        if (isGrabbing)
        {
            targetSpeed = input * boxMoveSpeed;
            animator.SetBool("isPushing", true);

            rb.linearVelocity = new Vector3(targetSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            return;
        }
        else
        {
            targetSpeed = input * usedSpeed;
            animator.SetBool("isPushing", false);
        }

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
        else
        {
            animator.SetBool("wallJumped", false);
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

    private void ApplyStateEffects()
    {
        if (globalPlayerStats.instance != null)
        {
            if (globalPlayerStats.instance.isHiding)
            {
                moveSpeed = hideMoveSpeed;
                jumpForce = hideJumpForce;
            }
            else
            {
                moveSpeed = observedMoveSpeed;
                jumpForce = observedJumpForce;
            }
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
        animator.SetBool("wallJumped", true);
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