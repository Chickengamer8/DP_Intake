using UnityEngine;

public class BoxGrabTrigger : MonoBehaviour
{
    private playerMovement playerScript;
    private Transform originalParent;
    private Rigidbody boxRb;
    private Collider boxCollider;
    private bool playerInCollider = false;

    private void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<playerMovement>();
        originalParent = transform.parent;
        boxRb = originalParent.GetComponent<Rigidbody>();
        boxCollider = originalParent.GetComponent<Collider>();
    }

    private void Update()
    {
        if (playerInCollider && playerScript.grabAttempt)
        {
            if (!playerScript.isGrabbing)
            {
                Debug.Log("Grab attempt!");
                playerScript.isGrabbing = true;

                // Zet de box als child van de speler
                originalParent.SetParent(playerScript.transform);
            }
        }
        else if (playerScript.isGrabbing && !playerScript.grabAttempt)
        {
            Debug.Log("Release");
            playerScript.isGrabbing = false;

            // Zet de box terug naar originele parent
            originalParent.SetParent(null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerScript.grabAttempt)
        {
            playerInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerScript.grabAttempt)
        {
            playerInCollider = false;
        }
    }
}
