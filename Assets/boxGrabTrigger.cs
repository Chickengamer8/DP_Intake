using UnityEngine;

public class BoxGrabTrigger : MonoBehaviour
{
    private playerMovement playerScript;
    private Transform originalParent;
    private Rigidbody boxRb;
    private Collider boxCollider;
    private bool playerInCollider = false;
    public GameObject puzzleParent;

    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<playerMovement>();
        originalParent = transform.parent;
    }

    private void Update()
    {
        if (!playerInCollider || !playerScript.isGrounded) return;

        if (playerScript.grabAttempt)
        {
            if (!playerScript.isGrabbing)
            {
                playerScript.isGrabbing = true;

                // Zet de box als child van de speler
                originalParent.SetParent(playerScript.transform);
            }
        }
        else if (playerScript.isGrabbing && !playerScript.grabAttempt)
        {
            playerScript.isGrabbing = false;
            playerInCollider = false;

            // Zet de box terug naar originele parent
            originalParent.SetParent(puzzleParent.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCollider = false;
        }
    }
}
