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

    private Vector3 targetPosition;

    private void Start()
    {
        if (pointA != null)
            targetPosition = pointB.position;
    }

    private void Update()
    {
        if (!hasSurvivor || pointA == null || pointB == null)
            return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Switch target when arriving
            if (targetPosition == pointA.position)
                targetPosition = pointB.position;
            else
                targetPosition = pointA.position;
        }
    }
}
