using UnityEngine;

public class movableBox : MonoBehaviour
{
    public Rigidbody boxRb;
    public Transform player;
    public float moveSpeed = 5f;
    public string playerTag = "Player";

    private bool playerInZone = false;
    private playerMovement playerMovementScript;

    void Start()
    {
        if (player != null)
            playerMovementScript = player.GetComponent<playerMovement>();
    }

    void Update()
    {
        if (playerInZone && Input.GetMouseButton(1))
        {
            playerMovementScript.isGrabbingBox = true;

            float input = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(input) > 0.1f)
            {
                Vector3 direction = new Vector3(input, 0f, 0f);
                boxRb.linearVelocity = new Vector3(direction.x * moveSpeed, boxRb.linearVelocity.y, 0f);
            }
            else
            {
                boxRb.linearVelocity = new Vector3(0f, boxRb.linearVelocity.y, 0f); // Stop als geen input
            }
        }
        else
        {
            if (playerMovementScript != null)
                playerMovementScript.isGrabbingBox = false;

            boxRb.linearVelocity = new Vector3(0f, boxRb.linearVelocity.y, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            player = other.transform;
            playerMovementScript = player.GetComponent<playerMovement>();
            playerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = false;
            if (playerMovementScript != null)
                playerMovementScript.isGrabbingBox = false;
        }
    }
}
