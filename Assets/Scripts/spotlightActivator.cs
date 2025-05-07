using UnityEngine;

public class spotlightActivator : MonoBehaviour
{
    [Header("Spotlight Setup")]
    public GameObject spotlightBeam;
    public float beamGrowSpeed = 2f;  // groeisnelheid
    public float targetBeamScaleY = 1f;  // doelhoogte

    [Header("Watcher Setup")]
    public watcherChaseController chaseController;
    public float franticMoveRange = 0.5f;
    public float franticMoveSpeed = 20f;
    public Transform watcherTargetPositionB;  // ← nieuw: doelpositie B

    [Header("Disable On Activate")]
    public GameObject objectToDisable;

    [Header("Audio")]
    public AudioSource spotlightAudio;

    private bool isPlayerInZone = false;
    private bool isActivated = false;
    private Vector3 watcherFranticStartPos;

    private void Start()
    {
        if (spotlightBeam != null)
        {
            spotlightBeam.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isPlayerInZone || isActivated) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateSpotlight();
        }
    }

    private void ActivateSpotlight()
    {
        isActivated = true;

        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
            Debug.Log("[SpotlightActivator] Disabled object: " + objectToDisable.name);
        }

        if (spotlightBeam != null)
        {
            spotlightBeam.SetActive(true);
            StartCoroutine(GrowBeam());
        }

        if (spotlightAudio != null)
            spotlightAudio.Play();

        if (chaseController != null)
            chaseController.DisableDamageZone();

        if (chaseController.watcher != null)
        {
            // Forceer teleport naar locatie B
            if (watcherTargetPositionB != null)
            {
                chaseController.watcher.position = watcherTargetPositionB.position;
                Debug.Log("[SpotlightActivator] Watcher teleported to position B.");
            }

            // Start frantic beweging (optioneel)
            watcherFranticStartPos = chaseController.watcher.position;
            InvokeRepeating(nameof(FranticMovement), 0f, 0.02f);
        }

        Debug.Log("[SpotlightActivator] Spotlight activated, watcher should now shake.");
    }

    private System.Collections.IEnumerator GrowBeam()
    {
        Vector3 currentScale = spotlightBeam.transform.localScale;
        Vector3 targetScale = new Vector3(currentScale.x, targetBeamScaleY, currentScale.z);

        while (spotlightBeam.transform.localScale.y < targetBeamScaleY)
        {
            spotlightBeam.transform.localScale = Vector3.MoveTowards(
                spotlightBeam.transform.localScale,
                targetScale,
                beamGrowSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private void FranticMovement()
    {
        if (chaseController.watcher != null)
        {
            float randomX = Random.Range(-franticMoveRange, franticMoveRange);
            float randomY = Random.Range(-franticMoveRange, franticMoveRange);

            Vector3 franticOffset = new Vector3(randomX, randomY, 0f);
            chaseController.watcher.position = watcherFranticStartPos + franticOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInZone = false;
    }
}
