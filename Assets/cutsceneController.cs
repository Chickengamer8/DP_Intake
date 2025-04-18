using UnityEngine;
using System.Collections;

public class cutsceneController : MonoBehaviour
{
    public Transform cameraTarget;           // Waar de camera naartoe moet (bijv. Automa Tom)
    public Transform originalCameraTarget;   // Waar hij weer terug moet
    public float zoomAmount = 3f;            // Orthographic size
    public float zoomDuration = 1.5f;
    public float holdTime = 5f;              // Hoe lang de cutscene duurt
    public playerMovement player;
    public Rigidbody playerRb;
    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barSlideDuration = 0.5f;

    private bool hasTriggered = false;

    private cameraFollow cameraFollowScript;

    void Start()
    {
        if (Camera.main != null)
        {
            cameraFollowScript = Camera.main.GetComponent<cameraFollow>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;

        hasTriggered = true;
        playCutscene();
    }

    private void playCutscene()
    {
        // Disable player control
        player.canMove = false;
        playerRb.linearVelocity = Vector3.zero;

        // Slide-in zwarte balken
        StartCoroutine(SlideBars(true));

        // Start camera zoom & pan
        if (cameraFollowScript != null)
        {
            cameraFollowScript.SetCutsceneTarget(cameraTarget, zoomAmount);
        }
    }

    public void sidebarsControl(bool status)
    {
        if (!status)
        {
            // Slide-out zwarte balken
            StartCoroutine(SlideBars(false));

            if (cameraFollowScript != null && originalCameraTarget != null)
            {
                cameraFollowScript.ResetToDefault(originalCameraTarget);
            }

            player.canMove = true;
        }
    }

    private IEnumerator SlideBars(bool slideIn)
    {
        float elapsed = 0f;

        Vector2 topStart = topBar.anchoredPosition;
        Vector2 bottomStart = bottomBar.anchoredPosition;

        Vector2 topEnd = new Vector2(topStart.x, slideIn ? 0f : 150f);
        Vector2 bottomEnd = new Vector2(bottomStart.x, slideIn ? 0f : -150f);

        while (elapsed < barSlideDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / barSlideDuration);

            topBar.anchoredPosition = Vector2.Lerp(topStart, topEnd, progress);
            bottomBar.anchoredPosition = Vector2.Lerp(bottomStart, bottomEnd, progress);

            yield return null;
        }

        topBar.anchoredPosition = topEnd;
        bottomBar.anchoredPosition = bottomEnd;
    }
}
