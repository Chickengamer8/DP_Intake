using UnityEngine;

public class robotSpawnAnimation : MonoBehaviour
{
    [Header("Robot Setup")]
    public GameObject robotPrefab;
    public Transform spawnPoint;
    public Transform targetPoint;

    [Header("Platform & Collider")]
    public GameObject platformToRotate;
    public GameObject colliderToEnable;

    [Header("Camera Control")]
    public cameraFollow cameraFollowScript;
    public Transform playerTarget;      // Waar de camera naar terug moet na afloop
    public float cameraZoom = 5f;
    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f); // ➔ instelbare offset

    private bool animationTriggered = false;

    public void startAnimation()
    {
        if (animationTriggered) return;
        animationTriggered = true;

        // Spawn de robot
        Vector3 spawnPosition = spawnPoint.position;
        GameObject robot = Instantiate(robotPrefab, spawnPosition, Quaternion.identity);

        // Camera laten volgen met offset
        if (cameraFollowScript != null)
        {
            cameraFollowScript.SetCutsceneTargetWithOffset(robot.transform, cameraZoom, cameraOffset);
        }

        // Geef robot zijn taken
        robotMovement robotScript = robot.GetComponent<robotMovement>();
        if (robotScript != null)
        {
            robotScript.SetTarget(targetPoint);
            robotScript.SetPlatformToRotate(platformToRotate);
            robotScript.SetColliderToEnable(colliderToEnable);
            robotScript.SetCallingAnimationScript(this);
        }
        else
        {
            Debug.LogWarning("robotSpawnAnimation: robotPrefab mist robotMovement script!");
        }
    }

    // Wordt aangeroepen als robot klaar is
    public void OnRobotAnimationComplete()
    {
        if (cameraFollowScript != null && playerTarget != null)
        {
            cameraFollowScript.ResetToDefault(playerTarget);
        }
    }
}
