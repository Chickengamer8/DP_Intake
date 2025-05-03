using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class hornSummon : MonoBehaviour
{
    [Header("Robot Setup")]
    public Transform robotWaypoint;
    public Transform robotSpawnPoint;
    public Transform robotReturnPoint;
    public GameObject robotPrefab;
    public AudioClip hornSound;
    public float enemyKillDelay = 1.5f;

    [Header("Cinematic UI")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barSlideDuration = 0.5f;
    public Vector2 topBarShowPos;
    public Vector2 topBarHidePos;
    public Vector2 bottomBarShowPos;
    public Vector2 bottomBarHidePos;

    [Header("Dialogue")]
    public List<string> dialogueLines = new List<string>();
    public TMPro.TextMeshProUGUI dialogueText;

    [Header("References")]
    public playerMovement playerScript;

    private bool playerInZone = false;
    private bool hasBeenUsed = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (topBar != null) topBar.anchoredPosition = topBarHidePos;
        if (bottomBar != null) bottomBar.anchoredPosition = bottomBarHidePos;
        if (dialogueText != null) dialogueText.text = "";

        if (playerScript == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerScript = player.GetComponent<playerMovement>();
        }
    }

    void Update()
    {
        if (playerInZone && !hasBeenUsed && Input.GetKeyDown(KeyCode.E))
        {
            hasBeenUsed = true;
            StartCoroutine(StartCinematicSequence());
        }
    }

    private IEnumerator StartCinematicSequence()
    {
        if (playerScript != null)
            playerScript.canMove = false;

        yield return StartCoroutine(SlideBarsIn());

        if (dialogueLines.Count > 0)
            yield return StartCoroutine(ShowDialogueLines());

        // ✅ Zodra de tekst klaar is, eerst UI weg
        yield return StartCoroutine(SlideBarsOut());

        // ✅ Daarna robot starten zonder wachten op de UI
        yield return StartCoroutine(SummonRobot());

        if (playerScript != null)
            playerScript.canMove = true;
    }

    private IEnumerator SlideBarsIn()
    {
        float t = 0f;
        while (t < barSlideDuration)
        {
            float lerp = t / barSlideDuration;
            if (topBar != null) topBar.anchoredPosition = Vector2.Lerp(topBarHidePos, topBarShowPos, lerp);
            if (bottomBar != null) bottomBar.anchoredPosition = Vector2.Lerp(bottomBarHidePos, bottomBarShowPos, lerp);
            t += Time.deltaTime;
            yield return null;
        }

        if (topBar != null) topBar.anchoredPosition = topBarShowPos;
        if (bottomBar != null) bottomBar.anchoredPosition = bottomBarShowPos;
    }

    private IEnumerator SlideBarsOut()
    {
        float t = 0f;
        while (t < barSlideDuration)
        {
            float lerp = t / barSlideDuration;
            if (topBar != null) topBar.anchoredPosition = Vector2.Lerp(topBarShowPos, topBarHidePos, lerp);
            if (bottomBar != null) bottomBar.anchoredPosition = Vector2.Lerp(bottomBarShowPos, bottomBarHidePos, lerp);
            t += Time.deltaTime;
            yield return null;
        }

        if (topBar != null) topBar.anchoredPosition = topBarHidePos;
        if (bottomBar != null) bottomBar.anchoredPosition = bottomBarHidePos;
    }

    private IEnumerator ShowDialogueLines()
    {
        for (int i = 0; i < dialogueLines.Count; i++)
        {
            dialogueText.text = dialogueLines[i];

            bool waitingForInput = true;
            while (waitingForInput)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    waitingForInput = false;
                }
                yield return null;
            }
        }

        dialogueText.text = "";
    }

    private IEnumerator SummonRobot()
    {
        if (hornSound != null)
        {
            audioSource.PlayOneShot(hornSound);
        }

        yield return new WaitForSeconds(1f);

        GameObject robot = Instantiate(robotPrefab, robotSpawnPoint.position, Quaternion.identity);
        robotSummon behavior = robot.GetComponent<robotSummon>();
        if (behavior != null)
        {
            behavior.StartSequence(robotWaypoint, robotReturnPoint, enemyKillDelay);
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
