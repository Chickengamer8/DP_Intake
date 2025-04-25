using UnityEngine;

public class leverInteract : MonoBehaviour
{
    public GameObject promptUI;
    public doorController connectedDoor;
    private bool playerNearby = false;
    private bool isActivated = false;

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            ActivateLever();
        }
    }

    void ActivateLever()
    {
        isActivated = true;
        if (promptUI != null)
            promptUI.SetActive(false);

        // Hier kun je een animatie starten als je wilt
        if (connectedDoor != null)
            connectedDoor.OpenDoor();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            playerNearby = true;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}
