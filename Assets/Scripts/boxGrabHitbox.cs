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
        if (playerScript.nearBox && Input.GetMouseButton(1) && currentBox != null)
        {
            currentBox.SetMovable(true, playerScript.transform);
        }
        else if (currentBox != null)
        {
            currentBox.SetMovable(false, null);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            currentBox = other.GetComponent<movableBox>();
            playerScript.nearBox = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Box") && currentBox != null && other.GetComponent<movableBox>() == currentBox)
        {
            currentBox.SetMovable(false, null);
            playerScript.nearBox = false;
            currentBox = null;
        }
    }
}
