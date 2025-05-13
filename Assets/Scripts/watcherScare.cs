using UnityEngine;

public class watcherScare : MonoBehaviour
{
    [Header("Transforms")]
    public Transform watcher;
    public Transform pointA;
    public Transform pointB;
    public Transform cameraPointA;
    public Transform player;

    [Header("Camera")]
    public cameraFollow cameraFollowScript;
    public float cameraHoldDuration = 2f;
    public Vector3 cutsceneOffset = new Vector3(0f, 0f, -10f);
    public float cutsceneZoom = 5f;

    [Header("Settings")]
    public float moveSpeed = 5f;

    [Header("Audio")]
    public AudioSource scareAudio;

    private bool hasTriggered = false;
    private bool movingToB = false;

    private playerMovement playerMovementScript;
    private Rigidbody playerRb;

    private void Start()
    {
        if (player != null)
        {
            playerMovementScript = player.GetComponent<playerMovement>();
            playerRb = player.GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (movingToB && watcher != null && pointB != null)
        {
            watcher.position = Vector3.MoveTowards(watcher.position, pointB.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(watcher.position, pointB.position) < 0.1f)
            {
                movingToB = false;
                if (watcher.gameObject.activeSelf)
                {
                    watcher.gameObject.SetActive(false);
                    Debug.Log("[WatcherScare] Watcher reached Point B and was disabled.");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        hasTriggered = true;

        if (watcher != null && pointA != null)
        {
            watcher.gameObject.SetActive(true);
            watcher.position = pointA.position;
            movingToB = true;
            Debug.Log("[WatcherScare] Watcher reset to Point A and started moving to Point B.");
        }

        if (scareAudio != null)
        {
            scareAudio.Play();
            Debug.Log("[WatcherScare] Scare audio played.");
        }

        if (cameraFollowScript != null && cameraPointA != null && player != null)
        {
            StartCoroutine(FocusCameraTemporarily());
        }
    }

    private System.Collections.IEnumerator FocusCameraTemporarily()
    {
        // Disable player movement
        if (playerMovementScript != null) playerMovementScript.canMove = false;
        if (playerRb != null) playerRb.linearVelocity = Vector3.zero;

        // Switch camera to cutscene point
        cameraFollowScript.SetCutsceneTargetWithOffset(cameraPointA, cutsceneZoom, cutsceneOffset);
        Debug.Log("[WatcherScare] Camera focus switched to cutscene target.");

        yield return new WaitForSeconds(cameraHoldDuration);

        // Return camera to player
        cameraFollowScript.ResetToDefault(player);
        Debug.Log("[WatcherScare] Camera returned to player.");

        // Re-enable movement
        if (playerMovementScript != null) playerMovementScript.canMove = true;
    }
}
