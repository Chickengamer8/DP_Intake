using UnityEngine;

public class movingPlatformManager : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Activation")]
    public bool hasSurvivor = false;

    [HideInInspector] public Vector3 currentVelocity; // ✅ Voeg dit toe

    private Vector3 targetPosition;
    private Vector3 lastPosition; // ✅ Voor velocity berekening

    private void Start()
    {
        if (pointA != null)
            targetPosition = pointB.position;

        lastPosition = transform.position; // ✅ Init
    }

    private void Update()
    {
        if (!hasSurvivor || pointA == null || pointB == null)
        {
            currentVelocity = Vector3.zero; // ✅ Zet velocity op nul als stil
            return;
        }

        Vector3 oldPosition = transform.position;

        // Platform bewegen
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // ✅ Velocity berekenen
        currentVelocity = (transform.position - oldPosition) / Time.deltaTime;

        // Wissel target
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            targetPosition = (targetPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }
}
