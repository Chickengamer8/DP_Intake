using UnityEngine;

public class BoxGrabTrigger : MonoBehaviour
{
    private playerMovement playerScript;
    private Rigidbody boxRb;
    private bool playerInCollider = false;
    private FixedJoint grabJoint = null;

    void Start()
    {
        playerScript = GameObject.FindWithTag("Player").GetComponent<playerMovement>();
        boxRb = GetComponent<Rigidbody>();

        // Zorg dat de box in eerste instantie niet zomaar verschuift
        boxRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (!playerInCollider || !playerScript.isGrounded) return;

        if (playerScript.grabAttempt)
        {
            if (!playerScript.isGrabbing)
            {
                playerScript.isGrabbing = true;
                AttachBoxToPlayer();
            }
        }
        else if (playerScript.isGrabbing)
        {
            playerScript.isGrabbing = false;
            DetachBoxFromPlayer();
        }
    }

    private void AttachBoxToPlayer()
    {
        grabJoint = gameObject.AddComponent<FixedJoint>();
        grabJoint.connectedBody = playerScript.GetComponent<Rigidbody>();
        grabJoint.breakForce = 1000f;
        grabJoint.breakTorque = 1000f;

        // Ontvries positie zodat de box kan bewegen
        boxRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void DetachBoxFromPlayer()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
        }

        // Vries de box weer vast zodat hij niet meer beweegt
        boxRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
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
