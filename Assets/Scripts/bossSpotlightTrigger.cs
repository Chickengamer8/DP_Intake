using UnityEngine;

public class bossSpotlightTrigger : MonoBehaviour
{
    public mainSpotlightManager spotlightManager;
    public Transform playerReference; // Sleep hier de echte Player Transform in
    private bool hasActivated = false;
    private bool playerInCollider = false;
    public GameObject visualConfirmation;

    private void Update()
    {
        if (playerInCollider && !hasActivated && Input.GetKeyDown(KeyCode.E))
        {
            spotlightManager.spotlightsActive++;
            hasActivated = true;

            if (visualConfirmation != null)
                visualConfirmation.SetActive(true);

            Debug.Log("Spotlight activated. Total: " + spotlightManager.spotlightsActive);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject == playerReference.gameObject)
        {
            playerInCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject == playerReference.gameObject)
        {
            playerInCollider = false;
        }
    }
}
