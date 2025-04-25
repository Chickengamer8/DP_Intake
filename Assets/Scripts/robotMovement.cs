using UnityEngine;

public class robotMovement : MonoBehaviour
{
    private Transform target;
    private GameObject platformToActivate;
    public float speed = 2f;
    private bool shouldMove = false;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        shouldMove = true;
    }

    public void SetPlatform(GameObject platform)
    {
        platformToActivate = platform;
    }

    private void Update()
    {
        if (!shouldMove || target == null) return;

        // Zorg dat hij in Z=0 blijft tijdens movement
        Vector3 targetPosition = target.position;
        targetPosition.z = 0f;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            shouldMove = false;

            if (platformToActivate != null)
            {
                platformToActivate.SetActive(true);
            }

            // Optioneel: idle animatie starten of iets zeggen
        }
    }
}
