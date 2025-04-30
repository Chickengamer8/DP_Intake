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
    public Vector3 deadzoneSize = new Vector3(4f, 2f, 4f);
    public Vector3 deadzoneOffset = Vector3.zero;
    public float deadzoneSmoothSpeed = 5f;

    [Header("Pupil Wander Area")]
    public Vector3 pupilWanderRadius = new Vector3(0.3f, 0.3f, 0.3f); // nieuw

    [Header("Wander Settings")]
    public float wanderInterval = 2f;
    public float wanderRange = 1f;
    public float moveSpeed = 2f;
    public float wanderPupilSpeed = 3f;
    public float wanderFrequencyX = 1.5f;
    public float wanderFrequencyY = 1.3f;
    public float wanderFrequencyZ = 1.1f;

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
        Vector3 desiredCenter = mainCamera.transform.position + deadzoneOffset;
        deadzoneCenter = Vector3.Lerp(deadzoneCenter, desiredCenter, Time.deltaTime * deadzoneSmoothSpeed);
    }

    void UpdateVision()
    {
        canSeePlayer = false;
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        bool isInHideZone = Physics.OverlapSphere(player.position, 0.1f, hideLayer).Length > 0;

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer))
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
                Vector3 offset = new Vector3(
                    Random.Range(-wanderRange, wanderRange),
                    Random.Range(-wanderRange, wanderRange),
                    Random.Range(-wanderRange, wanderRange)
                );
                wanderTarget = deadzoneCenter + offset;
                wanderTimer = wanderInterval;
            }

            transform.position = Vector3.SmoothDamp(transform.position, wanderTarget, ref currentVelocity, 0.2f);

            float wanderX = Mathf.Sin(Time.time * wanderFrequencyX) * pupilWanderRadius.x;
            float wanderY = Mathf.Cos(Time.time * wanderFrequencyY) * pupilWanderRadius.y;
            float wanderZ = Mathf.Sin(Time.time * wanderFrequencyZ) * pupilWanderRadius.z;
            Vector3 wanderOffset = new Vector3(wanderX, wanderY, wanderZ);
            if (eyePupil != null)
                eyePupil.localPosition = Vector3.Lerp(eyePupil.localPosition, wanderOffset, Time.deltaTime * wanderPupilSpeed);
        }
        else
        {
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
        Vector3 center = camPos + deadzoneOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, deadzoneSize);

        if (eyePupil != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(eyePupil.position, pupilWanderRadius * 2f);
        }
    }
}
