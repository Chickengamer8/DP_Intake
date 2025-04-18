using UnityEngine;
using System.Collections;

public class hornSummon : MonoBehaviour
{
    public Transform robotWaypoint;
    public Transform robotSpawnPoint;
    public Transform robotReturnPoint;
    public GameObject robotPrefab;
    public AudioClip hornSound;
    public float enemyKillDelay = 1.5f;

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
    }

    void Update()
    {
        if (playerInZone && !hasBeenUsed && Input.GetKeyDown(KeyCode.E))
        {
            hasBeenUsed = true;
            StartCoroutine(SummonRobot());
        }
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
