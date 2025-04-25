using UnityEngine;

public class movableBox : MonoBehaviour
{
    private Rigidbody rb;
    private Collider triggerCollider;
    private Collider physicalCollider;

    private Transform player;
    private bool isMovable = false;
    private bool isAttached = false;

    public float moveSpeed = 3f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        triggerCollider = GetComponent<Collider>();
        physicalCollider = GetComponentInChildren<Collider>();

        if (physicalCollider == null)
        {
            Debug.LogError("No physical collider found as child!");
        }

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    void FixedUpdate()
    {
        if (isMovable && isAttached && player != null)
        {
            float input = Input.GetAxisRaw("Horizontal");
            Vector3 newPosition = transform.position + new Vector3(input * moveSpeed * Time.fixedDeltaTime, 0f, 0f);
            rb.MovePosition(newPosition);
        }
    }

    public void SetMovable(bool movable, Transform playerTransform)
    {
        isMovable = movable;
        if (movable && playerTransform != null)
        {
            player = playerTransform;
            isAttached = true;
            if (physicalCollider != null)
                physicalCollider.enabled = false; // tijdelijk uitschakelen zodat speler niet tegen box drukt

            // Verplaats naar player (parenting)
            transform.SetParent(player);
        }
        else
        {
            isAttached = false;
            player = null;
            if (physicalCollider != null)
                physicalCollider.enabled = true;

            transform.SetParent(null);
        }
    }
}
