using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class tomParkourManager : MonoBehaviour
{
    [Header("Tom Setup")]
    public GameObject tomPrefab;
    public Transform tomSpawnPoint;
    public List<Transform> waypoints;
    public float waypointWaitTime = 2f;
    public float tomMoveSpeed = 3f;
    public float maxPlayerDistance = 30f;

    [Header("UI Setup - BottomBar")]
    public RectTransform bottomBar;
    public TMP_Text dialogueText;
    public Vector3 bottomBarShowPosition;
    public Vector3 bottomBarHidePosition;
    public float bottomBarSlideSpeed = 5f;

    [Header("UI Setup - Countdown")]
    public RectTransform countdownUI;
    public TMP_Text countdownText;
    public Vector3 countdownShowPosition;
    public Vector3 countdownHidePosition;
    public float countdownSlideSpeed = 5f;

    [Header("Dialogue")]
    public List<string> introDialogue;
    public float dialogueSkipDelay = 0.1f;
    public float countdownDelay = 1f;

    [Header("Player Reference")]
    public playerMovement playerScript;

    [Header("Trigger References")]
    public Collider initializeTrigger;

    private GameObject tomInstance;
    private bool sequenceStarted = false;
    private bool hasSequenceRunBefore = false;
    private bool playerInInitializeTrigger = false;

    private void Update()
    {
        if (playerInInitializeTrigger && !sequenceStarted && Input.GetKeyDown(KeyCode.E))
        {
            sequenceStarted = true;
            StartCoroutine(RunSequence());
        }

        if (sequenceStarted && tomInstance != null)
        {
            float distance = Vector3.Distance(playerScript.transform.position, tomInstance.transform.position);
            if (distance > maxPlayerDistance)
            {
                Debug.Log("[tomParkourManager] Player too far, resetting Tom.");
                StopAllCoroutines();
                tomInstance.transform.position = waypoints[0].position;
                sequenceStarted = false;
            }
        }
    }

    private IEnumerator RunSequence()
    {
        playerScript.canMove = false;
        Rigidbody playerRb = playerScript.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
        }

        if (!hasSequenceRunBefore)
        {
            tomInstance = Instantiate(tomPrefab, tomSpawnPoint.position, tomSpawnPoint.rotation);
            yield return StartCoroutine(MoveToWaypoint(waypoints[0]));

            StartCoroutine(SlideUI(bottomBar, bottomBarShowPosition, bottomBarSlideSpeed));
            yield return StartCoroutine(PlayDialogue(introDialogue));
            StartCoroutine(SlideUI(bottomBar, bottomBarHidePosition, bottomBarSlideSpeed));

            hasSequenceRunBefore = true;
        }

        playerScript.canMove = true;

        for (int i = 1; i < waypoints.Count; i++)
        {
            yield return new WaitForSeconds(waypointWaitTime);

            StartCoroutine(SlideUI(countdownUI, countdownShowPosition, countdownSlideSpeed));
            yield return StartCoroutine(CountdownSequence(i));
        }

        Debug.Log("[tomParkourManager] Parkour sequence completed.");
        StartCoroutine(SlideUI(countdownUI, countdownHidePosition, countdownSlideSpeed));
        sequenceStarted = false;
    }

    private IEnumerator CountdownSequence(int waypointNum)
    {
        string[] countdowns = { "3", "2", "1", "GO" };
        foreach (string count in countdowns)
        {
            countdownText.text = count;
            if (count != "GO") yield return new WaitForSeconds(countdownDelay);
        }

        countdownText.text = "";
        yield return StartCoroutine(MoveToWaypoint(waypoints[waypointNum]));
    }

    private IEnumerator MoveToWaypoint(Transform waypoint)
    {
        while (Vector3.Distance(tomInstance.transform.position, waypoint.position) > 0.1f)
        {
            tomInstance.transform.position = Vector3.MoveTowards(tomInstance.transform.position, waypoint.position, tomMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator PlayDialogue(List<string> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            dialogueText.text = lines[i];

            // Wacht tot speler spatie loslaat als hij hem vasthoudt
            yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Space));

            // Wacht op nieuwe spatie indruk
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }
        dialogueText.text = "";
    }

    private IEnumerator SlideUI(RectTransform uiElement, Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(uiElement.anchoredPosition, targetPosition) > 0.01f)
        {
            uiElement.anchoredPosition = Vector3.Lerp(uiElement.anchoredPosition, targetPosition, Time.deltaTime * speed);
            yield return null;
        }
        uiElement.anchoredPosition = targetPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInInitializeTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInInitializeTrigger = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (tomInstance != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tomInstance.transform.position, maxPlayerDistance);
        }
    }
}
