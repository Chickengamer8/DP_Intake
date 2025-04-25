using UnityEngine;

public class eyeWander : MonoBehaviour
{
    [Header("Player & Vision")]
    public Transform player;
    public LayerMask hideLayer;

    [Header("Camera & Deadzone")]
    public Camera mainCamera;
    public Vector2 deadzoneSize = new Vector2(4f, 2f);
    public Vector2 deadzoneOffset = Vector2.zero;
    public float deadzoneSmoothSpeed = 5f;

    [Header("Wander Settings")]
    public float wanderInterval = 2f;
    public float wanderRange = 1f;
    public float moveSpeed = 2f;

    private Vector3 currentTarget;
    private float wanderTimer = 0f;
    private Vector3 deadzoneCenter;
    private Vector3 currentVelocity;

    private bool canSeePlayer = true;

    [Header("Pupil Movement")]
    public Transform eyePupil;
    public float lookRadius = 0.5f;
    public float followSpeed = 5f;
    public float wanderSpeed = 3f;
    public float wanderFrequencyX = 1.5f;
    public float wanderFrequencyY = 1.3f;
    public float wanderRadiusFactor = 0.5f;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        wanderTimer = wanderInterval;
        currentTarget = transform.position;
        deadzoneCenter = transform.position;
    }

    void Update()
    {
        Vector3 desiredCenter = mainCamera.transform.position + (Vector3)deadzoneOffset;
        deadzoneCenter = Vector3.Lerp(deadzoneCenter, desiredCenter, Time.deltaTime * deadzoneSmoothSpeed);

        // Zichtlijn check
        canSeePlayer = !Physics.Linecast(transform.position, player.position, hideLayer);

        // Waar moet het oog heen kijken?
        if (canSeePlayer)
        {
            currentTarget = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
        else
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0f)
            {
                Vector2 offset = new Vector2(
                    Random.Range(-wanderRange, wanderRange),
                    Random.Range(-wanderRange, wanderRange)
                );
                currentTarget = deadzoneCenter + (Vector3)offset;
                wanderTimer = wanderInterval;
            }
        }

        // Clamp binnen deadzone
        Vector3 min = deadzoneCenter - (Vector3)(deadzoneSize / 2f);
        Vector3 max = deadzoneCenter + (Vector3)(deadzoneSize / 2f);
        currentTarget = new Vector3(
            Mathf.Clamp(currentTarget.x, min.x, max.x),
            Mathf.Clamp(currentTarget.y, min.y, max.y),
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, currentTarget, ref currentVelocity, 0.2f);

        TrackPlayerWithEye();
    }

    void TrackPlayerWithEye()
    {
        if (eyePupil == null || player == null) return;

        if (!canSeePlayer)
        {
            float wanderX = Mathf.Sin(Time.time * wanderFrequencyX) * (lookRadius * wanderRadiusFactor);
            float wanderY = Mathf.Cos(Time.time * wanderFrequencyY) * (lookRadius * wanderRadiusFactor);
            Vector3 wanderOffset = new Vector3(wanderX, wanderY, 0f);

            eyePupil.localPosition = Vector3.Lerp(eyePupil.localPosition, wanderOffset, Time.deltaTime * wanderSpeed);
        }
        else
        {
            Vector3 direction = player.position - transform.position;
            Vector3 clampedDir = Vector3.ClampMagnitude(direction, lookRadius);
            Vector3 targetPos = clampedDir;

            eyePupil.localPosition = Vector3.Lerp(eyePupil.localPosition, targetPos, Time.deltaTime * followSpeed);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;

        Vector3 camPos = Application.isPlaying ? deadzoneCenter : mainCamera.transform.position;
        Vector3 center = new Vector3(camPos.x + deadzoneOffset.x, camPos.y + deadzoneOffset.y, transform.position.z);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(deadzoneSize.x, deadzoneSize.y, 0f));
    }
}
