using UnityEngine;

public class bossSpotlightTrigger : MonoBehaviour
{
    public mainSpotlightManager spotlightManager;
    private bool hasActivated = false;
    private bool playerInZone = false;
    public GameObject visualConfirmation;

    private void Update()
    {
        if (playerInZone && !hasActivated && Input.GetKeyDown(KeyCode.E))
        {
            spotlightManager.spotlightsActive++;
            hasActivated = true;
            visualConfirmation.SetActive(true);
            Debug.Log("Spotlight activated. Total: " + spotlightManager.spotlightsActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = false;
    }
}
