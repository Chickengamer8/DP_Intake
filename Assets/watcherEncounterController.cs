using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class watcherEncounterController : MonoBehaviour
{
    [Header("Watcher Setup")]
    public Transform watcher;
    public Transform watcherPupil;
    public Transform watcherRisePoint;
    public float riseSpeed = 1f;

    [Header("Player Setup")]
    public Transform player;
    public Rigidbody playerRb;
    public playerMovement playerMovementScript;
    public float playerLiftSpeed = 1f;
    public float playerHealthReductionRate = 10f;

    [Header("Automa Tom Setup")]
    public GameObject automaTomPrefab;
    public Transform tomSpawnPoint;
    public Transform tomTargetPoint;
    public float tomMoveSpeed = 3f;

    [Header("Cutscene Blocker")]
    public GameObject cutsceneBlocker;

    [Header("Bottom Bar UI")]
    public RectTransform bottomBar;
    public Vector2 hiddenPosition;
    public Vector2 shownPosition;
    public float slideDuration = 0.3f;
    public TMP_Text dialogueText;
    public List<string> cutsceneLines;
    public float timePerLine = 2f;

    private bool cutsceneStarted = false;
    private bool rising = false;
    private bool liftingPlayer = false;
    private bool tomSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (cutsceneStarted) return;

        if (other.CompareTag("Player"))
        {
            cutsceneStarted = true;
            StartCoroutine(StartCutscene());
        }
    }

    private IEnumerator StartCutscene()
    {
        StartWatcherRise();

        yield return StartCoroutine(SlideBottomBar(shownPosition, slideDuration));
        yield return StartCoroutine(ShowCutsceneLines());

        yield return StartCoroutine(SlideBottomBar(hiddenPosition, slideDuration));
    }

    void Update()
    {
        if (rising)
        {
            watcher.position += Vector3.up * riseSpeed * Time.deltaTime;

            if (watcher.position.y >= watcherRisePoint.position.y && !liftingPlayer)
            {
                liftingPlayer = true;
            }
        }

        if (liftingPlayer)
        {
            playerRb.useGravity = false;
            playerRb.linearVelocity = Vector3.zero;
            player.position += Vector3.up * playerLiftSpeed * Time.deltaTime;

            if (globalPlayerStats.instance != null)
            {
                globalPlayerStats.instance.currentHealth -= playerHealthReductionRate * Time.deltaTime;
                globalPlayerStats.instance.currentHealth = Mathf.Max(globalPlayerStats.instance.currentHealth, 5f);
            }

            if (!tomSpawned && watcher.position.y >= tomSpawnPoint.position.y)
            {
                StartCoroutine(SpawnAutomaTom());
            }
        }
    }

    public void StartWatcherRise()
    {
        rising = true;

        if (playerMovementScript != null)
            playerMovementScript.canMove = false;

        playerRb.linearVelocity = Vector3.zero;

        if (cutsceneBlocker != null)
            cutsceneBlocker.SetActive(true);
    }

    private IEnumerator SpawnAutomaTom()
    {
        tomSpawned = true;

        GameObject tom = Instantiate(automaTomPrefab, tomSpawnPoint.position, Quaternion.identity);

        while (Vector3.Distance(tom.transform.position, tomTargetPoint.position) > 0.1f)
        {
            tom.transform.position = Vector3.MoveTowards(tom.transform.position, tomTargetPoint.position, tomMoveSpeed * Time.deltaTime);
            yield return null;
        }

        // Stop rising/lifting
        rising = false;
        liftingPlayer = false;

        playerRb.useGravity = true;
        playerRb.linearVelocity = Vector3.down * 5f;

        if (playerMovementScript != null)
            playerMovementScript.canMove = true;

        if (cutsceneBlocker != null)
            cutsceneBlocker.SetActive(false);
    }

    private IEnumerator SlideBottomBar(Vector2 targetPos, float duration)
    {
        Vector2 startPos = bottomBar.anchoredPosition;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            bottomBar.anchoredPosition = Vector2.Lerp(startPos, targetPos, t / duration);
            yield return null;
        }

        bottomBar.anchoredPosition = targetPos;
    }

    private IEnumerator ShowCutsceneLines()
    {
        foreach (string line in cutsceneLines)
        {
            dialogueText.text = line;
            yield return new WaitForSeconds(timePerLine);
        }

        dialogueText.text = "";
    }
}
