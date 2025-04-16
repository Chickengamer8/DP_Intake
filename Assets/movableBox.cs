using UnityEngine;

public class movableBox : MonoBehaviour
{
    private bool isGrabbed = false;
    private Transform player;
    private Vector3 grabOffset;

    public float moveSpeed = 3f;

    private Rigidbody rb;
    private Collider boxCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        if (isGrabbed && player != null)
        {
            float input = Input.GetAxisRaw("Horizontal");
            Vector3 targetPos = player.position + grabOffset;

            // Alleen bewegen als input gegeven wordt
            if (Mathf.Abs(input) > 0.1f)
            {
                Vector3 moveDir = new Vector3(input, 0, 0).normalized;
                rb.MovePosition(transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void SetMovable(bool grab, Transform playerTransform)
    {
        isGrabbed = grab;
        player = playerTransform;

        if (grab)
        {
            // Zet collider tijdelijk op trigger om glijden te voorkomen
            boxCollider.isTrigger = true;

            // Offset zodat hij links/rechts van speler zit
            float xOffset = playerTransform.position.x > transform.position.x ? -1f : 1f;
            grabOffset = new Vector3(xOffset, 0, 0);
        }
        else
        {
            boxCollider.isTrigger = false;
        }
    }
}
