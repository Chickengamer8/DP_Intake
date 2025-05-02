using UnityEngine;

public class ladderClimb : MonoBehaviour
{
    [Header("Climb Settings")]
    public float climbSpeed = 3f;

    private bool isPlayerOnLadder = false;
    private playerMovement playerController;
    private Rigidbody playerRb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<playerMovement>();
            playerRb = other.GetComponent<Rigidbody>();

            if (playerController != null)
            {
                isPlayerOnLadder = true;
                playerController.canMove = false; // Blokkeer normaal lopen
                playerRb.useGravity = false;      // Zet zwaartekracht tijdelijk uit
                playerRb.linearVelocity = Vector3.zero; // Stop snelheid
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerController != null)
        {
            isPlayerOnLadder = false;
            playerController.canMove = true;    // Zet normaal lopen terug aan
            playerRb.useGravity = true;         // Zet zwaartekracht terug aan
        }
    }

    private void Update()
    {
        if (!isPlayerOnLadder || playerController == null) return;

        float verticalInput = Input.GetAxisRaw("Vertical"); // W/S of ↑/↓
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D of ←/→

        Vector3 climbDirection = Vector3.zero;

        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            climbDirection = new Vector3(0f, verticalInput, 0f);
        }
        else if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            // Bepaal klimmen op basis van horizontale kant, bijv. voor zijkantladder
            climbDirection = new Vector3(0f, Mathf.Abs(horizontalInput), 0f);
        }

        playerRb.linearVelocity = climbDirection * climbSpeed;
    }
}
