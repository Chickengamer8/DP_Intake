using UnityEngine;

public class moveBigBox : MonoBehaviour
{
    public Transform[] pathPoints;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float speedMultiplier = 1f;
    public Renderer boxRenderer;
    public Color endColor = Color.white;
    public MonoBehaviour movableBoxScript; // Script om te activeren
    public GameObject objectToActivate;     // Nieuw! Object om te activeren

    private int currentPointIndex = 0;
    private bool isMoving = false;
    private bool playerInGrabZone = false;
    private bool isGrabbed = false;
    private bool autoMoveStarted = false;
    private Transform player;

    private Collider boxCollider;
    private Rigidbody rb;

    void Start()
    {
        boxCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (movableBoxScript != null)
        {
            movableBoxScript.enabled = false;
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }

    void Update()
    {
        if (autoMoveStarted)
        {
            MoveAlongPath();
            return;
        }

        if (playerInGrabZone && Input.GetMouseButton(1))
        {
            isGrabbed = true;
            Vector3 directionToPush = (pathPoints[1].position - pathPoints[0].position).normalized;
            transform.position += directionToPush * moveSpeed * Time.deltaTime;

            float passedDot = Vector3.Dot(
                (transform.position - pathPoints[0].position).normalized,
                (pathPoints[1].position - pathPoints[0].position).normalized
            );

            if (passedDot > 0.95f)
            {
                isGrabbed = false;
                autoMoveStarted = true;
                currentPointIndex = 1;
            }
        }
        else
        {
            isGrabbed = false;
        }
    }

    void MoveAlongPath()
    {
        if (currentPointIndex >= pathPoints.Length) return;

        Transform targetPoint = pathPoints[currentPointIndex];

        Vector3 direction = (targetPoint.position - transform.position).normalized;
        transform.position += direction * (moveSpeed * speedMultiplier) * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, targetPoint.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, targetPoint.position);
        if (distance < 0.1f)
        {
            transform.position = targetPoint.position;

            if (currentPointIndex == pathPoints.Length - 1)
            {
                transform.rotation = targetPoint.rotation;

                if (boxCollider != null)
                {
                    boxCollider.isTrigger = true;
                }

                if (boxRenderer != null)
                {
                    boxRenderer.material.color = endColor;
                }

                if (movableBoxScript != null)
                {
                    movableBoxScript.enabled = true;
                }

                if (objectToActivate != null)
                {
                    objectToActivate.SetActive(true);
                }

                this.enabled = false; // Zet moveBigBox script uit
            }
            else
            {
                currentPointIndex++;
            }
        }
    }

    public void SetPlayerInZone(bool state, Transform playerRef)
    {
        playerInGrabZone = state;
        player = playerRef;
    }
}
