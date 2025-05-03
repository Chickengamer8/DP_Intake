using UnityEngine;

public class watcherScare : MonoBehaviour
{
    public Transform watcher;
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 5f;
    public AudioSource scareAudio;

    private bool hasTriggered = false;
    private bool movingToB = false;

    private void Update()
    {
        if (movingToB && watcher != null && pointB != null)
        {
            watcher.position = Vector3.MoveTowards(watcher.position, pointB.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(watcher.position, pointB.position) < 0.1f)
            {
                movingToB = false;
                if (watcher.gameObject.activeSelf)
                {
                    watcher.gameObject.SetActive(false);
                    Debug.Log("[WatcherScare] Watcher reached Point B and was disabled.");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        hasTriggered = true;

        if (watcher != null && pointA != null)
        {
            watcher.gameObject.SetActive(true);  // just in case it was off
            watcher.position = pointA.position;
            movingToB = true;
            Debug.Log("[WatcherScare] Watcher reset to Point A and started moving to Point B.");
        }

        if (scareAudio != null)
        {
            scareAudio.Play();
            Debug.Log("[WatcherScare] Scare audio played.");
        }
    }
}
