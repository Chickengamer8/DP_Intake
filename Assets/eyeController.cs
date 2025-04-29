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

    [Header("Wander Settings")]
    public float wanderInterval = 2f;
    public float wanderRange = 1f;
    public float moveSpeed = 2f;
    public float wanderPupilSpeed = 3f;
    public float wanderFrequencyX = 1.5f;
    public float wanderFrequencyY = 1.3f;
    public float wanderRadiusFactor = 0.5f;

    [Header("Damage Settings")]
    public float damageInterval = 1f;
    public float damagePerTick = 10f;
    public float damageIncreaseRate = 0.5f;
    public float maxDamageMultiplier = 5f;
    public float recoveryRate = 1f;
    public LayerMask hideLayer;

    private Vector3 deadzoneCenter;
    private Vector3 wanderTarget;
    private Vector3 currentVelocity;
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
    }

    void UpdateDeadzone()
    {
        Vector3 desiredCenter = mainCamera.transform.position + (Vector3)deadzoneOffset;
        deadzoneCenter = Vector3.Lerp(deadzoneCenter, desiredCenter, Time.deltaTime * deadzoneSmoothSpeed);
    }

    void UpdateVision()
    {
        // Check lijn naar speler
        canSeePlayer = false;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                canSeePlayer = true;
            }
            else
            {
                // Check of het object op Hide-layer zit
                if (((1 << hit.collider.gameObject.layer) & hideLayer) != 0)
                {
                    canSeePlayer = false;
                }
            }
        }
        else
        {
            canSeePlayer = true;
        }

        // Update globalPlayerStats hiding status
        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.isHiding = canSeePlayer;
        }

        // Optioneel: aanpassen van movement/jump als verstopt
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
                wanderTarget = deadzoneCenter + (Vector3)offset;
                wanderTimer = wanderInterval;
            }

            // Smooth wander movement
            transform.position = Vector3.SmoothDamp(transform.position, wanderTarget, ref currentVelocity, 0.2f);

            // Pupil wander beweging
            float wanderX = Mathf.Sin(Time.time * wanderFrequencyX) * (lookRadius * wanderRadiusFactor);
            float wanderY = Mathf.Cos(Time.time * wanderFrequencyY) * (lookRadius * wanderRadiusFactor);
            Vector3 wanderOffset = new Vector3(wanderX, wanderY, 0f);
            if (eyePupil != null)
                eyePupil.localPosition = Vector3.Lerp(eyePupil.localPosition, wanderOffset, Time.deltaTime * wanderPupilSpeed);
        }
        else
        {
            // Volg speler direct
            Vector3 dir = player.position - transform.position;
            Vector3 clampedDir = Vector3.ClampMagnitude(dir, lookRadius);
            if (eyePupil != null)
                eyePupil.localPosition = Vector3.Lerp(eyePupil.localPosition, clampedDir, Time.deltaTime * followSpeed);
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

    void OnDrawGizmos()
    {
        if (!Application.isPlaying && mainCamera == null)
            mainCamera = Camera.main;

        Vector3 camPos = Application.isPlaying ? deadzoneCenter : mainCamera.transform.position;
        Vector3 center = camPos + (Vector3)deadzoneOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(deadzoneSize.x, deadzoneSize.y, 0f));
    }
}
