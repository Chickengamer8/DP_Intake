using UnityEngine;

public class eyeController : MonoBehaviour
{
    [Header("Player Tracking")]
    public Transform player;
    public Camera mainCamera;
    public Transform eyePupil;
    public float lookRadius = 0.5f;
    public float followSpeed = 5f;

    [Header("Deadzone (Wander Area)")]
    public Vector2 deadzoneSize = new Vector2(4f, 2f);
    public Vector2 deadzoneOffset = Vector2.zero;
    public float deadzoneSmoothSpeed = 5f;

    [Header("Pupil Wander Area")]
    public Vector2 pupilWanderRadius = new Vector2(0.3f, 0.3f);

    [Header("Wander Settings")]
    public float wanderInterval = 2f;
    public float wanderRange = 1f;
    public float moveSpeed = 2f;
    public float wanderPupilSpeed = 3f;
    public float wanderFrequencyX = 1.5f;
    public float wanderFrequencyY = 1.3f;

    [Header("Damage Settings")]
    public float damageInterval = 1f;
    public float damagePerTick = 10f;
    public float damageIncreaseRate = 0.5f;
    public float maxDamageMultiplier = 5f;
    public float recoveryRate = 1f;
    public float holeDamagePerTick = 20f; // ← NIEUW
    public LayerMask hideLayer;
    public LayerMask visionBlockingLayers;

    private Vector2 deadzoneCenter;
    private Vector2 wanderTarget;
    private Vector2 currentVelocity;
    private float wanderTimer;
    private float damageTimer;
    private float damageMultiplier = 1f;
    private bool canSeePlayer = false;

    private playerHealth playerHealth;
    private playerMovement playerMovement;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        deadzoneCenter = transform.position;
        wanderTarget = transform.position;
        wanderTimer = wanderInterval;
        damageTimer = damageInterval;

        if (player != null)
        {
            playerHealth = player.GetComponent<playerHealth>();
            playerMovement = player.GetComponent<playerMovement>();
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null)
            return;

        UpdateDeadzone();
        UpdateVision();
        UpdateEyeMovement();
        UpdateDamage();
        CheckHoleOverlap();
    }

    void UpdateDeadzone()
    {
        Vector2 desiredCenter = (Vector2)mainCamera.transform.position + deadzoneOffset;
        deadzoneCenter = Vector2.Lerp(deadzoneCenter, desiredCenter, Time.deltaTime * deadzoneSmoothSpeed);
    }

    void UpdateVision()
    {
        canSeePlayer = false;
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        bool isInHideZone = Physics.OverlapSphere(player.position, 0.1f, hideLayer).Length > 0;

        if (Physics.Raycast(transform.position, player.position - transform.position, out RaycastHit hit, distanceToPlayer, visionBlockingLayers))
        {
            if (hit.collider.CompareTag("Player") && !isInHideZone)
            {
                canSeePlayer = true;
            }
        }
        else if (!isInHideZone)
        {
            canSeePlayer = true;
        }

        if (globalPlayerStats.instance != null)
            globalPlayerStats.instance.isHiding = !canSeePlayer;

        if (playerMovement != null)
        {
            if (globalPlayerStats.instance != null && globalPlayerStats.instance.isHiding)
            {
                playerMovement.moveSpeed = playerMovement.hideMoveSpeed;
                playerMovement.jumpForce = playerMovement.hideJumpForce;
            }
            else
            {
                playerMovement.moveSpeed = playerMovement.defaultMoveSpeed;
                playerMovement.jumpForce = playerMovement.defaultJumpForce;
            }
        }
    }

    void UpdateEyeMovement()
    {
        if (!canSeePlayer)
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0f)
            {
                Vector2 offset = new Vector2(
                    Random.Range(-wanderRange, wanderRange),
                    Random.Range(-wanderRange, wanderRange)
                );
                wanderTarget = deadzoneCenter + offset;
                wanderTimer = wanderInterval;
            }

            Vector2 smoothPos = Vector2.SmoothDamp((Vector2)transform.position, wanderTarget, ref currentVelocity, 0.2f);
            transform.position = new Vector3(smoothPos.x, smoothPos.y, transform.position.z);

            float wanderX = Mathf.Sin(Time.time * wanderFrequencyX) * pupilWanderRadius.x;
            float wanderY = Mathf.Cos(Time.time * wanderFrequencyY) * pupilWanderRadius.y;
            Vector2 wanderOffset = new Vector2(wanderX, wanderY);
            if (eyePupil != null)
                eyePupil.localPosition = Vector3.Lerp(eyePupil.localPosition, new Vector3(wanderOffset.x, wanderOffset.y, eyePupil.localPosition.z), Time.deltaTime * wanderPupilSpeed);
        }
        else
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position);
            Vector2 clampedDir = Vector2.ClampMagnitude(dir, lookRadius);
            if (eyePupil != null)
                eyePupil.localPosition = Vector3.Lerp(eyePupil.localPosition, new Vector3(clampedDir.x, clampedDir.y, eyePupil.localPosition.z), Time.deltaTime * followSpeed);
        }
    }

    void UpdateDamage()
    {
        if (canSeePlayer)
        {
            damageMultiplier += damageIncreaseRate * Time.deltaTime;
            damageMultiplier = Mathf.Clamp(damageMultiplier, 1f, maxDamageMultiplier);

            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                playerHealth.TakeDamage(damagePerTick * damageMultiplier);
                damageTimer = damageInterval;
            }
        }
        else
        {
            damageMultiplier -= recoveryRate * Time.deltaTime;
            damageMultiplier = Mathf.Clamp(damageMultiplier, 1f, maxDamageMultiplier);
            damageTimer = damageInterval;
        }
    }

    void CheckHoleOverlap()
    {
        Collider[] overlapping = Physics.OverlapSphere(player.position, 0.1f);
        bool isInCoverWall = false;

        // Check eerst of de speler in een coverWall zit
        foreach (var col in overlapping)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("coverWall"))
            {
                isInCoverWall = true;
                break;
            }
        }

        if (globalPlayerStats.instance != null)
            globalPlayerStats.instance.isInCoverWall = isInCoverWall;

        // Als speler in coverWall zit → GEEN hole damage
        if (isInCoverWall) return;

        // Anders check of speler in hole zit en meer dan 50% overlapt
        foreach (var col in overlapping)
        {
            if (col.CompareTag("hole"))
            {
                Collider playerCol = player.GetComponent<Collider>();
                if (playerCol != null)
                {
                    Bounds holeBounds = col.bounds;
                    Bounds playerBounds = playerCol.bounds;

                    Bounds intersection = playerBounds;
                    intersection.Encapsulate(holeBounds.min);
                    intersection.Encapsulate(holeBounds.max);

                    float intersectionVolume = intersection.size.x * intersection.size.y * intersection.size.z;
                    float playerVolume = playerBounds.size.x * playerBounds.size.y * playerBounds.size.z;

                    float overlapPercent = intersectionVolume / playerVolume;

                    if (overlapPercent > 0.5f)
                    {
                        playerHealth.TakeDamage(holeDamagePerTick * Time.deltaTime);
                        Debug.Log("[eyeController] Player taking hole damage.");
                        break;
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying && mainCamera == null)
            mainCamera = Camera.main;

        Vector2 camPos = Application.isPlaying ? deadzoneCenter : (Vector2)mainCamera.transform.position;
        Vector2 center = camPos + deadzoneOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(center.x, center.y, transform.position.z), new Vector3(deadzoneSize.x, deadzoneSize.y, 0f));

        if (eyePupil != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(eyePupil.position, new Vector3(pupilWanderRadius.x * 2f, pupilWanderRadius.y * 2f, 0f));
        }
    }
}
