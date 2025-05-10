using UnityEngine;

public class BoxGrabTrigger : MonoBehaviour
{
    [Header("Box Settings")]
    public GameObject boxObject;
    public float boxMass = 10f;
    public LayerMask groundLayer;
    public float groundCheckLength = 0.6f;
    public float groundSupportThreshold = 0.5f;
    public float freezeDelayAfterFall = 3f;
    public bool dragWithLeft = false;

    [Header("References")]
    public playerMovement playerScript;

    private Rigidbody boxRb;
    private bool playerInCollider = false;
    private ConfigurableJoint grabJoint = null;
    private bool waitingForFreeze = false;
    private float freezeTimer = 0f;
    private bool isGrabbed = false;

    private void Start()
    {
        if (playerScript == null)
        {
            Debug.LogError("BoxGrabTrigger: playerScript is not assigned.");
            enabled = false;
            return;
        }

        if (boxObject != null)
        {
            boxRb = boxObject.GetComponent<Rigidbody>();
            if (boxRb == null)
            {
                Debug.LogError("BoxGrabTrigger: No Rigidbody found on boxObject.");
                enabled = false;
                return;
            }
            boxRb.mass = boxMass;
            FreezeBox();
        }
        else
        {
            Debug.LogError("BoxGrabTrigger: boxObject is not assigned.");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        HandleGrabInput();
        HandleFreezeTimer();

        int mouseButton = dragWithLeft ? 0 : 1;

        // ✅ Loslaten als je de muisknop loslaat
        if (isGrabbed && !Input.GetMouseButton(mouseButton))
        {
            isGrabbed = false;
            DetachBoxFromPlayer();
        }

        // ✅ EXTRA: Check of de box niet meer ondersteund is terwijl je hem vasthebt
        if (isGrabbed && !CheckSupportUnderBox())
        {
            Debug.Log("BoxGrabTrigger: Box lost support while grabbing, force detach!");
            isGrabbed = false;
            DetachBoxFromPlayer();
        }
    }

    private void HandleGrabInput()
    {
        if (!playerInCollider || !playerScript.isGrounded)
            return;

        int mouseButton = dragWithLeft ? 0 : 1;

        if (Input.GetMouseButton(mouseButton))
        {
            // ✅ CHECK: Is deze box binnen de grabHitbox overlap?
            if (playerScript.grabHitbox == null)
            {
                Debug.LogWarning("BoxGrabTrigger: grabHitbox reference missing on playerScript.");
                return;
            }

            Collider[] hitColliders = Physics.OverlapSphere(
                playerScript.grabHitbox.position,
                0.5f,
                LayerMask.GetMask("Box")
            );

            bool isInGrabHitbox = false;
            foreach (Collider col in hitColliders)
            {
                if (col.gameObject == boxObject)
                {
                    isInGrabHitbox = true;
                    break;
                }
            }

            if (!isGrabbed && isInGrabHitbox)
            {
                isGrabbed = true;
                AttachBoxToPlayer();
            }
        }
    }

    private void AttachBoxToPlayer()
    {
        if (playerScript.isGrabbing)
        {
            Debug.Log("BoxGrabTrigger: Player is already grabbing something, skipping.");
            isGrabbed = false;
            return;
        }

        UnfreezeBox();

        grabJoint = boxObject.AddComponent<ConfigurableJoint>();
        grabJoint.connectedBody = playerScript.GetComponent<Rigidbody>();
        grabJoint.xMotion = ConfigurableJointMotion.Locked;
        grabJoint.yMotion = ConfigurableJointMotion.Free;
        grabJoint.zMotion = ConfigurableJointMotion.Locked;
        grabJoint.angularXMotion = ConfigurableJointMotion.Locked;
        grabJoint.angularYMotion = ConfigurableJointMotion.Locked;
        grabJoint.angularZMotion = ConfigurableJointMotion.Locked;

        playerScript.isGrabbing = true;
    }

    private void DetachBoxFromPlayer()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
        }

        playerScript.isGrabbing = false;

        bool hasSupport = CheckSupportUnderBox();
        if (hasSupport)
        {
            boxRb.linearVelocity = Vector3.zero;
            boxRb.angularVelocity = Vector3.zero;
            FreezeBox();
            waitingForFreeze = false;
        }
        else
        {
            UnfreezeBox();
            waitingForFreeze = true;
            freezeTimer = freezeDelayAfterFall;
        }
    }

    private void HandleFreezeTimer()
    {
        if (waitingForFreeze)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                bool hasSupport = CheckSupportUnderBox();
                if (hasSupport)
                {
                    boxRb.linearVelocity = Vector3.zero;
                    boxRb.angularVelocity = Vector3.zero;
                    FreezeBox();
                    waitingForFreeze = false;
                }
                else
                {
                    freezeTimer = freezeDelayAfterFall;
                }
            }
        }
    }

    private void FreezeBox()
    {
        boxRb.constraints = RigidbodyConstraints.FreezePositionX
                          | RigidbodyConstraints.FreezePositionZ
                          | RigidbodyConstraints.FreezeRotationX
                          | RigidbodyConstraints.FreezeRotationY;
    }

    private void UnfreezeBox()
    {
        boxRb.constraints = RigidbodyConstraints.FreezePositionZ
                          | RigidbodyConstraints.FreezeRotationX
                          | RigidbodyConstraints.FreezeRotationY;
    }

    private bool CheckSupportUnderBox()
    {
        Vector3 centerCheckPos = boxObject.transform.position;
        bool centerSupported = Physics.Raycast(
            centerCheckPos,
            Vector3.down,
            groundCheckLength,
            groundLayer
        );

#if UNITY_EDITOR
        Debug.DrawLine(
            centerCheckPos,
            centerCheckPos + Vector3.down * groundCheckLength,
            centerSupported ? Color.green : Color.red
        );
#endif
        return centerSupported;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInCollider = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInCollider = false;
    }

    private void OnDrawGizmos()
    {
        if (boxObject == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            boxObject.transform.position,
            boxObject.transform.position + Vector3.down * groundCheckLength
        );
        Gizmos.DrawSphere(
            boxObject.transform.position + Vector3.down * groundCheckLength,
            0.05f
        );
    }

    public void ForceDetachBox()
    {
        if (grabJoint != null)
        {
            Debug.Log("BoxGrabTrigger: Forced detach of box.");
            Destroy(grabJoint);
            grabJoint = null;
        }

        if (playerScript != null)
        {
            playerScript.isGrabbing = false;
        }

        isGrabbed = false;
        waitingForFreeze = false;

        Debug.Log("BoxGrabTrigger: ForceDetachBox completed.");
    }
}
