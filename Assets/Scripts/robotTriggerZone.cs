using UnityEngine;

public class robotTriggerZone : MonoBehaviour
{
    public GameObject robotPrefab;
    public Transform spawnPoint;
    public Transform targetPoint;
    public GameObject platformToActivate;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;

        triggered = true;

        // Zorg dat Z = 0 voor zichtbaarheid in orthographic camera
        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.z = 0f;

        GameObject robot = Instantiate(robotPrefab, spawnPosition, Quaternion.identity);

        // Geef target en platform door
        robotMovement robotScript = robot.GetComponent<robotMovement>();
        robotScript.SetTarget(targetPoint);
        robotScript.SetPlatform(platformToActivate);
    }
}
