using UnityEngine;
using System.Collections.Generic;

public class watcherChaseController : MonoBehaviour
{
    [Header("Watcher Setup")]
    public GameObject watcherObject;
    public Transform watcher;
    public Transform pointA;
    public Transform pointB;
    public float baseChaseSpeed = 5f;
    public float maxChaseSpeed = 10f;
    public float distanceThreshold = 10f;

    [Header("Trigger Zone")]
    public Collider triggerZone;

    [Header("Sprite Mask")]
    public GameObject spriteMask;

    [Header("Danger Zone")]
    public Collider damageZone;
    public bool damageZoneActive = true;

    [Header("Camera Zoom")]
    public cameraFollow cameraFollowScript;
    public float targetZoom = 7f;
    public float zoomChangeSpeed = 1f;

    [Header("Reset Objects")]
    public List<Transform> objectsToReset = new List<Transform>();
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();

    private bool chaseStarted = false;
    private bool movingToB = false;
    private bool resetInProgress = false;

    private Vector3 originalWatcherPosition;
    private float originalZoom;

    [Header("Speed Multiplier")]
    public float maxSpeedMultiplier = 2f; // bijvoorbeeld: 2 betekent maximaal 2x base speed


    private void Start()
    {
        if (watcherObject != null)
            watcherObject.SetActive(false);

        if (watcher != null)
            originalWatcherPosition = watcher.localPosition;

        if (cameraFollowScript != null)
            originalZoom = cameraFollowScript.defaultZoom;

        // Save original positions
        foreach (Transform obj in objectsToReset)
        {
            if (obj != null && !originalPositions.ContainsKey(obj))
                originalPositions.Add(obj, obj.position);
        }
    }

    private void Update()
    {
        if (chaseStarted && globalPlayerStats.instance != null && globalPlayerStats.instance.currentHealth <= 0f && !resetInProgress)
        {
            Debug.Log("[WatcherChaseController] Player died during chase, resetting scene after delay.");
            resetInProgress = true;
            ResetScene();
            return;
        }

        if (!chaseStarted || watcher == null) return;

        float distanceToPlayer = Vector3.Distance(watcher.position, globalPlayerStats.instance.transform.position);
        float clampedDistance = Mathf.Clamp01((distanceToPlayer - distanceThreshold) / distanceThreshold);
        float speedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, clampedDistance);
        float dynamicSpeed = baseChaseSpeed * speedMultiplier;

        if (movingToB)
        {
            watcher.position = Vector3.MoveTowards(watcher.position, pointB.position, dynamicSpeed * Time.deltaTime);

            if (Vector3.Distance(watcher.position, pointB.position) < 0.1f)
            {
                movingToB = false;
                Debug.Log("[WatcherChaseController] Watcher reached Point B, entering wander mode.");
            }
        }
    }

    public void DisableDamageZone()
    {
        Debug.Log("[WatcherChaseController] Damage zone disabled by spotlight.");
        damageZoneActive = false;
    }

    public void ResetScene()
    {
        Debug.Log("[WatcherChaseController] Resetting scene objects to original positions.");

        // Reset watcher
        if (watcher != null)
            watcher.localPosition = originalWatcherPosition;

        // Reset camera zoom
        if (cameraFollowScript != null)
            cameraFollowScript.SetZoom(originalZoom);

        // Reset listed objects
        foreach (var kvp in originalPositions)
        {
            if (kvp.Key != null)
                kvp.Key.position = kvp.Value;
        }

        // Reset flags
        chaseStarted = false;
        movingToB = false;
        resetInProgress = false;

        if (watcherObject != null)
            watcherObject.SetActive(false);

        if (spriteMask != null)
            spriteMask.SetActive(false);

        damageZoneActive = true;
    }

    private System.Collections.IEnumerator SmoothZoomOut()
    {
        float t = 0f;
        float startZoom = cameraFollowScript.defaultZoom;

        while (Mathf.Abs(cameraFollowScript.defaultZoom - targetZoom) > 0.05f)
        {
            cameraFollowScript.SetZoom(Mathf.Lerp(startZoom, targetZoom, t));
            t += Time.deltaTime * zoomChangeSpeed;
            yield return null;
        }

        cameraFollowScript.SetZoom(targetZoom);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (chaseStarted) return;
        if (!other.CompareTag("Player")) return;

        chaseStarted = true;
        movingToB = true;

        if (watcherObject != null)
        {
            watcherObject.SetActive(true);
            Debug.Log("[WatcherChaseController] Watcher object enabled.");
        }

        if (spriteMask != null)
            spriteMask.SetActive(true);

        if (cameraFollowScript != null)
            StartCoroutine(SmoothZoomOut());
    }

    private void OnDrawGizmosSelected()
    {
        if (watcher != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(watcher.position, distanceThreshold);
        }
    }
}
