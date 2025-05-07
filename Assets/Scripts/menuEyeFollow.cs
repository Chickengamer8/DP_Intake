using UnityEngine;

public class menuEyeFollow : MonoBehaviour
{
    [Header("Tracking Settings")]
    public RectTransform parentRect;   // het oog zelf
    public float maxDistance = 50f;

    [Header("Wander Settings")]
    public float wanderRadius = 30f;
    public float wanderSpeed = 3f;
    public float pauseTime = 1f;

    private Vector3 startLocalPos;
    private Vector3 targetLocalPos;
    private float wanderTimer = 0f;

    void Start()
    {
        startLocalPos = transform.localPosition;
        PickNewWanderTarget();
    }

    void Update()
    {
        if (Cursor.visible)
        {
            TrackCursor();
        }
        else
        {
            Wander();
        }
    }

    void TrackCursor()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, Input.mousePosition, null, out localPoint);

        Vector3 dir = new Vector3(localPoint.x, localPoint.y, 0f);
        Vector3 clamped = Vector3.ClampMagnitude(dir, maxDistance);

        Vector3 targetPos = clamped;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * wanderSpeed);
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * wanderSpeed);

        if (wanderTimer >= pauseTime)
        {
            PickNewWanderTarget();
            wanderTimer = 0f;
        }
    }

    void PickNewWanderTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        targetLocalPos = startLocalPos + new Vector3(randomPoint.x, randomPoint.y, 0f);
    }

    void OnDrawGizmosSelected()
    {
        if (parentRect != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(parentRect.position, maxDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(parentRect.position, wanderRadius);
        }
    }
}
