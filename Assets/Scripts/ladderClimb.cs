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

            if (playerController != null && playerRb != null)
            {
                isPlayerOnLadder = true;
                playerRb.useGravity = false;

                // ✅ Reset velocity zodra je de ladder opgaat
                playerRb.linearVelocity = Vector3.zero;

                Debug.Log("[Ladder] Speler is op ladder, velocity gereset & gravity uit.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerController != null)
        {
            isPlayerOnLadder = false;
            playerRb.useGravity = true;

            Debug.Log("[Ladder] Speler heeft ladder verlaten, gravity weer aan.");
        }
    }

    private void Update()
    {
        if (!isPlayerOnLadder || playerController == null || playerRb == null) return;

        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 climbVelocity = new Vector3(playerRb.linearVelocity.x, verticalInput * climbSpeed, playerRb.linearVelocity.z);
        playerRb.linearVelocity = climbVelocity;
    }
}