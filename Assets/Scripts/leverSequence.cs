using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class leverSequence : MonoBehaviour
{
    [Header("Waypoints for Camera")]
    public List<Transform> cameraWaypoints;
    public Vector3 cutsceneOffset = new Vector3(0f, 0f, -10f);
    public float cutsceneZoom = 7f;
    public float cameraZoomDuration = 1f;

    [Header("Lever Sprites")]
    public List<SpriteRenderer> leverSprites;
    public Sprite activatedSprite;
    public int playerLeverIndex = 1; // Index van de hendel waar de speler zelf aan trekt

    [Header("Camera Control")]
    public cameraFollow cameraScript;
    public Transform defaultCameraTarget;

    [Header("Rotating Objects")]
    public List<GameObject> rotatingObjects;
    public List<float> rotationAmounts;
    public float rotationSpeed = 90f;
    public float rotationDelay = 1f;

    [Header("Enemy Objects")]
    public List<GameObject> enemyObjects; // Direct de GameObjects, niet meer animators
    public float enemyDisableDelay = 1f;

    [Header("Player Reference")]
    public playerMovement playerScript;
    public Rigidbody playerRb;

    [Header("Puzzle Check")]
    public bool puzzleSolved = false;

    private bool playerInZone = false;
    private bool sequenceStarted = false;

    private void Update()
    {
        if (playerInZone && !sequenceStarted && puzzleSolved)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                sequenceStarted = true;
                StartCoroutine(SequenceRoutine());
            }
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

    private IEnumerator SequenceRoutine()
    {
        // Disable player movement and stop velocity
        playerScript.canMove = false;
        playerRb.linearVelocity = Vector3.zero;

        // Change player's own lever sprite first
        if (playerLeverIndex >= 0 && playerLeverIndex < leverSprites.Count)
        {
            leverSprites[playerLeverIndex].sprite = activatedSprite;
            yield return new WaitForSeconds(0.2f);
        }

        // Step 1: Camera to first waypoint + change first lever sprite
        if (cameraWaypoints.Count > 0 && leverSprites.Count > 0)
        {
            cameraScript.SetCutsceneTargetWithOffset(cameraWaypoints[0], cameraScript.defaultZoom, cutsceneOffset);
            yield return new WaitForSeconds(cameraZoomDuration);
            if (leverSprites[0] != null)
                leverSprites[0].sprite = activatedSprite;
        }

        // Step 2: Camera to second waypoint + change second lever sprite
        if (cameraWaypoints.Count > 1 && leverSprites.Count > 1)
        {
            cameraScript.SetCutsceneTargetWithOffset(cameraWaypoints[1], cameraScript.defaultZoom, cutsceneOffset);
            yield return new WaitForSeconds(cameraZoomDuration);
            if (leverSprites[1] != null)
                leverSprites[1].sprite = activatedSprite;
        }

        // Step 3: Final waypoint + immediately start smooth zoom out
        if (cameraWaypoints.Count > 2)
        {
            cameraScript.SetCutsceneTargetWithOffset(cameraWaypoints[2], cutsceneZoom, cutsceneOffset);
            yield return new WaitForSeconds(cameraZoomDuration);
            if (leverSprites[2] != null)
                leverSprites[2].sprite = activatedSprite;
        }

        // Step 4: Sequentially rotate objects and disable enemies
        for (int i = 0; i < rotatingObjects.Count; i++)
        {
            GameObject obj = rotatingObjects[i];
            float targetRotation = (i < rotationAmounts.Count) ? rotationAmounts[i] : 90f;

            if (obj != null)
            {
                yield return StartCoroutine(SmoothRotate(obj.transform, targetRotation, rotationSpeed));
                yield return new WaitForSeconds(rotationDelay);

                if (i < enemyObjects.Count && enemyObjects[i] != null)
                {
                    yield return new WaitForSeconds(enemyDisableDelay);
                    enemyObjects[i].SetActive(false);
                }
            }
        }

        // Step 5: Reset camera back to default target and zoom
        cameraScript.ResetToDefault(defaultCameraTarget);

        // Enable player movement again
        playerScript.canMove = true;
    }

    private IEnumerator SmoothRotate(Transform objTransform, float angle, float speed)
    {
        float rotated = 0f;
        while (rotated < angle)
        {
            float step = speed * Time.deltaTime;
            objTransform.Rotate(Vector3.forward, step);
            rotated += step;
            yield return null;
        }

        // Correct overshoot
        float overshoot = rotated - angle;
        if (overshoot > 0f)
            objTransform.Rotate(Vector3.forward, -overshoot);
    }
}
