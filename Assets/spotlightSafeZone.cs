using UnityEngine;

public class spotlightSafeZone : MonoBehaviour
{
    public watcherChaseController chaseController;
    public damageZoneTrigger damageZoneScript;

    public float boostedBaseSpeed = 15f;  // tijdelijke snelheid
    public bool applyOnce = true;         // optioneel: alleen eerste keer activeren

    public cameraFollow cameraFollowScript;
    public float newZoomLevel = 10f;      // verder uitgezoomde camera

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (applyOnce && hasActivated) return;

        hasActivated = true;

        if (chaseController != null)
        {
            chaseController.baseChaseSpeed = boostedBaseSpeed;
            Debug.Log("[SpotlightSafeZone] Watcher base speed set to boosted value: " + boostedBaseSpeed);
        }

        if (damageZoneScript != null)
        {
            damageZoneScript.canDealDamage = false;
            Debug.Log("[SpotlightSafeZone] DamageZoneTrigger script disabled.");
        }

        if (cameraFollowScript != null)
        {
            cameraFollowScript.SetZoom(newZoomLevel);
            Debug.Log("[SpotlightSafeZone] Camera zoomed out to: " + newZoomLevel);
        }
    }
}
