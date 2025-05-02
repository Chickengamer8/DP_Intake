using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class leverController : MonoBehaviour
{
    [Header("Prompt UI")]
    public RectTransform promptTransform;
    public TMPro.TextMeshProUGUI promptText;
    public string promptMessage = "Press [E] to pull lever";
    public Vector2 hiddenPosition;
    public Vector2 shownPosition;
    public float slideDuration = 0.3f;

    [Header("Lever Setup")]
    public cameraFollow cameraFollowScript;
    public Transform playerFollowTarget;
    public Transform doorFocusPoint;
    public Transform doorHinge;
    public float panSpeed = 2f;
    public float doorOpenSpeed = 1f;
    public Vector3 cutsceneOffset = new Vector3(0f, 0f, -10f);
    public float cutsceneZoom = 5f;

    [Header("Audio")]
    public AudioSource doorSound;

    [Header("Sprites")]
    public SpriteRenderer leverSpriteRenderer;
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    private bool isPlayerInZone = false;
    private bool hasActivated = false;

    private void Update()
    {
        if (!isPlayerInZone || hasActivated) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            hasActivated = true;
            if (doorSound != null)
                doorSound.Play();

            StartCoroutine(HandleLeverActivation());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || hasActivated) return;

        isPlayerInZone = true;
        promptText.text = promptMessage;
        StartCoroutine(SlidePrompt(promptTransform, hiddenPosition, shownPosition, slideDuration));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInZone = false;

        if (!hasActivated)
            StartCoroutine(SlidePrompt(promptTransform, shownPosition, hiddenPosition, slideDuration));
    }

    private IEnumerator HandleLeverActivation()
    {
        StartCoroutine(SlidePrompt(promptTransform, shownPosition, hiddenPosition, slideDuration));

        if (leverSpriteRenderer != null && activeSprite != null)
            leverSpriteRenderer.sprite = activeSprite;

        // Laat camera de deur volgen
        if (cameraFollowScript != null)
            cameraFollowScript.SetCutsceneTargetWithOffset(doorFocusPoint, cutsceneZoom, cutsceneOffset);

        // Wacht totdat deur volledig opent
        yield return StartCoroutine(OpenDoor());

        // Blijf 3 seconden bij de deur hangen
        yield return new WaitForSeconds(3f);

        // Zet camera terug naar speler
        if (cameraFollowScript != null)
            cameraFollowScript.ResetToDefault(playerFollowTarget);
    }

    private IEnumerator OpenDoor()
    {
        Quaternion startRot = doorHinge.localRotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, 90f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * doorOpenSpeed;
            doorHinge.localRotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        doorHinge.localRotation = endRot; // zorg dat hij exact eindigt
    }

    private IEnumerator SlidePrompt(RectTransform target, Vector2 from, Vector2 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.anchoredPosition = Vector2.Lerp(from, to, t / duration);
            yield return null;
        }
        target.anchoredPosition = to;
    }
}
