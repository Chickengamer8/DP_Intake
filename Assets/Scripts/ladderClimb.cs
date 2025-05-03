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
                playerRb.useGravity = false;  // Alleen zwaartekracht uit
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerController != null)
        {
            isPlayerOnLadder = false;
            playerRb.useGravity = true;
        }
    }

    private void Update()
    {
        if (!isPlayerOnLadder || playerController == null) return;

        float verticalInput = Input.GetAxisRaw("Vertical"); // W/S of ↑/↓

        Vector3 climbVelocity = new Vector3(playerRb.linearVelocity.x, verticalInput * climbSpeed, playerRb.linearVelocity.z);
        playerRb.linearVelocity = climbVelocity;
    }
}
