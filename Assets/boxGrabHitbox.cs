using UnityEngine;

public class boxGrabHitbox : MonoBehaviour
{
    private playerMovement playerScript;
    private movableBox currentBox;

    void Start()
    {
        playerScript = GetComponentInParent<playerMovement>();
    }

    void Update()
    {
        // Alleen actie ondernemen als we bij een box zijn
        if (currentBox != null)
        {
            // Als RMB wordt ingehouden, activeer grab
            if (Input.GetMouseButton(1))
            {
                if (!playerScript.isGrabbing)
                {
                    playerScript.isGrabbing = true;
                    currentBox.SetMovable(true, playerScript.transform);
                }
            }
            else
            {
                if (playerScript.isGrabbing)
                {
                    playerScript.isGrabbing = false;
                    currentBox.SetMovable(false, null);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            // Alleen een nieuwe box activeren als er nog geen actief is
            if (currentBox == null)
            {
                currentBox = other.GetComponent<movableBox>();
                playerScript.nearBox = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            if (currentBox == other.GetComponent<movableBox>())
            {
                if (playerScript.isGrabbing)
                {
                    currentBox.SetMovable(false, null);
                    playerScript.isGrabbing = false;
                }

                currentBox = null;
                playerScript.nearBox = false;
            }
        }
    }
}
