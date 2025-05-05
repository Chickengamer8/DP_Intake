using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mainSpotlightManager : MonoBehaviour
{
    [Header("State")]
    public int spotlightsActive = 0;

    [Header("Camera")]
    public cameraFollow cameraScript;
    public Transform cameraFocusPoint;
    public float zoomOutLevel = 10f;
    public float zoomDuration = 2f;

    [Header("Tom Movement")]
    public GameObject tomObject;
    public Transform tomPointA;
    public Transform tomPointB;
    public float tomMoveSpeed = 2f;

    [Header("Initial Sequence Changes (Start)")]
    public List<GameObject> objectsToEnableAtStart = new List<GameObject>();
    public List<GameObject> objectsToDisableAtStart = new List<GameObject>();

    [Header("Final Changes (After Tom Reaches B)")]
    public GameObject objectToDisable;
    public GameObject objectToEnable;

    [Header("Watcher Frantic Movement")]
    public Transform watcher;
    public float franticMoveRange = 0.2f;

    private Vector3 watcherFranticStartPos;
    private bool isFrantic = false;
    private bool endSequenceStarted = false;

    private void Start()
    {
        if (watcher != null)
        {
            watcherFranticStartPos = watcher.position;
        }
    }

    private void Update()
    {
        if (!endSequenceStarted && spotlightsActive >= 3)
        {
            endSequenceStarted = true;
            StartCoroutine(EndSequence());
        }

        if (isFrantic)
        {
            FranticMovement();
        }
    }

    private IEnumerator EndSequence()
    {
        // Enable start objects
        foreach (GameObject obj in objectsToEnableAtStart)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDisableAtStart)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // Start frantic watcher movement
        isFrantic = true;

        // Pan and zoom camera
        cameraScript.SetCutsceneTargetWithOffset(cameraFocusPoint, zoomOutLevel, new Vector3(0, 0, -10));
        yield return new WaitForSeconds(zoomDuration);

        // Move Tom from A to B
        if (tomObject != null && tomPointA != null && tomPointB != null)
        {
            tomObject.transform.position = tomPointA.position;

            while (Vector3.Distance(tomObject.transform.position, tomPointB.position) > 0.1f)
            {
                tomObject.transform.position = Vector3.MoveTowards(
                    tomObject.transform.position,
                    tomPointB.position,
                    tomMoveSpeed * Time.deltaTime
                );
                yield return null;
            }
        }

        // Final changes
        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        // Stop frantic movement
        isFrantic = false;

        Debug.Log("End sequence completed.");
    }

    private void FranticMovement()
    {
        if (watcher != null)
        {
            float randomX = Random.Range(-franticMoveRange, franticMoveRange);
            float randomY = Random.Range(-franticMoveRange, franticMoveRange);

            Vector3 franticOffset = new Vector3(randomX, randomY, 0f);
            watcher.position = watcherFranticStartPos + franticOffset;
        }
    }
}
