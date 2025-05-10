using UnityEngine;

public class platformStick : MonoBehaviour
{
    public playerMovement playerScript;    // Sleep je speler-script in

    private Rigidbody playerRb;
    private bool playerOnPlatform = false;
    private FixedJoint stickJoint;

    private void Start()
    {
        if (playerScript != null)
            playerRb = playerScript.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = false;
            Detach();
        }
    }

    private void Update()
    {
        if (playerOnPlatform && playerRb != null)
        {
            // ✅ Check voor input: WASD + pijltjes + spatie
            bool isMovingInput = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                                 Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
                                 Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) ||
                                 Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) ||
                                 Input.GetKey(KeyCode.Space);

            if (isMovingInput)
            {
                Detach();
            }
            else
            {
                // ✅ Check of de speler volledig stil staat (velocity == 0)
                if (playerRb.linearVelocity.magnitude < 0.01f) // Klein beetje speling
                {
                    TryAttach();
                }
            }
        }
    }

    private void TryAttach()
    {
        if (stickJoint == null)
        {
            stickJoint = playerRb.gameObject.AddComponent<FixedJoint>();
            stickJoint.connectedBody = GetComponent<Rigidbody>();
            Debug.Log("🔗 Player gekoppeld aan platform (velocity == 0)");
        }
    }

    private void Detach()
    {
        if (stickJoint != null)
        {
            Destroy(stickJoint);
            stickJoint = null;
            Debug.Log("🛑 Player losgekoppeld van platform (input)");
        }
    }
}
