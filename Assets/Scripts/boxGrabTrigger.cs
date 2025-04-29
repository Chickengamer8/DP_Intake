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

    [Header("References")]
    public playerMovement playerScript;

    private Rigidbody boxRb;
    private bool playerInCollider = false;
    private FixedJoint grabJoint = null;
    private bool waitingForFreeze = false;
    private float freezeTimer = 0f;
    private bool isGrabbed = false;
    private float lastSupportRatio = 0f;
    private bool lastHasSupport = false;

    private void Start()
    {
        if (playerScript == null)
        {
            Debug.LogError("BoxGrabTrigger: Geen playerMovement script toegewezen!");
            enabled = false;
            return;
        }

        if (boxObject != null)
        {
            boxRb = boxObject.GetComponent<Rigidbody>();
            if (boxRb == null)
            {
                Debug.LogError("BoxGrabTrigger: Het toegewezen Box-object heeft GEEN Rigidbody!");
                enabled = false;
                return;
            }

            boxRb.mass = boxMass;
            FreezeBox();
        }
        else
        {
            Debug.LogError("BoxGrabTrigger: Geen Box-object toegewezen! Sleep de juiste Box in de Inspector.");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        HandleGrabInput();
        HandleFreezeTimer();

        if (playerInCollider && playerScript.grabAttempt && !isGrabbed)
        {
            TeleportPlayerAroundBox();
        }

        if (isGrabbed && !Input.GetMouseButton(1))
        {
            isGrabbed = false;
            DetachBoxFromPlayer();
        }
    }

    private void HandleGrabInput()
    {
        if (!playerInCollider || !playerScript.isGrounded) return;

        if (playerScript.grabAttempt)
        {
            if (!isGrabbed)
            {
                isGrabbed = true;
                AttachBoxToPlayer();
            }
        }
    }

    private void AttachBoxToPlayer()
    {
        UnfreezeBox();
        grabJoint = boxObject.AddComponent<FixedJoint>();
        grabJoint.connectedBody = playerScript.GetComponent<Rigidbody>();
        grabJoint.breakForce = 1000f;
        grabJoint.breakTorque = 1000f;
    }

    private void DetachBoxFromPlayer()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
        }

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
        boxRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    private void UnfreezeBox()
    {
        boxRb.constraints = RigidbodyConstraints.None;
        boxRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    private bool CheckSupportUnderBox()
    {
        Vector3 centerCheckPos = boxObject.transform.position;
        bool centerSupported = Physics.Raycast(centerCheckPos, Vector3.down, groundCheckLength, groundLayer);

#if UNITY_EDITOR
        Debug.DrawLine(centerCheckPos, centerCheckPos + Vector3.down * groundCheckLength, centerSupported ? Color.green : Color.red);
#endif

        lastSupportRatio = centerSupported ? 1f : 0f;
        lastHasSupport = centerSupported;

        return centerSupported;
    }

    private void TeleportPlayerAroundBox()
    {
        if (boxObject == null || playerScript == null) return;

        Vector3 playerPos = playerScript.transform.position;
        Vector3 boxPos = boxObject.transform.position;
        float offset = 1f;

        if (playerPos.x < boxPos.x)
            playerScript.transform.position = new Vector3(boxPos.x - offset, playerPos.y, playerPos.z);
        else
            playerScript.transform.position = new Vector3(boxPos.x + offset, playerPos.y, playerPos.z);
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxObject.transform.position, boxObject.transform.localScale);
    }
}
