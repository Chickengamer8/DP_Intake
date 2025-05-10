using UnityEngine;

public class bossSpotlightTrigger : MonoBehaviour
{
    [Header("Spotlight Setup")]
    public mainSpotlightManager spotlightManager;
    public Transform playerReference; // Sleep hier de echte Player Transform in
    private bool hasActivated = false;
    private bool playerInCollider = false;
    public GameObject visualConfirmation;
    public GameObject batteryUI;

    [Header("Watcher + EyeController Settings")]
    public Transform watcher; // Sleep hier je watcher in
    public float wanderIntervalIncrease = 0.5f;
    public float wanderRangeIncrease = 0.5f;
    public float moveSpeedIncrease = 0.5f;
    public float wanderPupilSpeedIncrease = 0.5f;
    public Vector3 watcherScaleIncrease = Vector3.zero;

    private void Update()
    {
        if (playerInCollider && !hasActivated && Input.GetKeyDown(KeyCode.E))
        {
            spotlightManager.spotlightsActive++;
            hasActivated = true;
            batteryUI.SetActive(false);

            if (visualConfirmation != null)
                visualConfirmation.SetActive(true);

            Debug.Log("Spotlight activated. Total: " + spotlightManager.spotlightsActive);

            // ✅ Boost EyeController van de watcher
            if (watcher != null)
            {
                eyeController eyeScript = watcher.GetComponent<eyeController>();
                if (eyeScript != null)
                {
                    eyeScript.wanderInterval += wanderIntervalIncrease;
                    eyeScript.wanderRange += wanderRangeIncrease;
                    eyeScript.moveSpeed += moveSpeedIncrease;
                    eyeScript.wanderPupilSpeed += wanderPupilSpeedIncrease;

                    // Vergroot de watcher zelf
                    Vector3 newScale = watcher.localScale + watcherScaleIncrease;
                    watcher.localScale = newScale;

                    Debug.Log("[bossSpotlightTrigger] EyeController boosted + watcher scaled.");
                }
                else
                {
                    Debug.LogWarning("[bossSpotlightTrigger] Geen eyeController gevonden op watcher!");
                }
            }
            else
            {
                Debug.LogWarning("[bossSpotlightTrigger] Watcher Transform is niet toegewezen!");
            }
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
