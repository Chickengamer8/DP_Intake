using UnityEngine;

public class hideHornSummon : MonoBehaviour
{
    [Header("Robot settings")]
    public GameObject robotPrefab;
    public Transform spawnPoint;
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    [Header("Audio")]
    public AudioClip hornSound;

    private GameObject spawnedRobot;
    private Vector3 targetPoint;
    private AudioSource audioSource;

    private bool playerInZone = false;
    private bool robotActive = false;
    private bool hasStartedPatrolling = false;
    private Transform player;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = hornSound;
    }

    void Update()
    {
        if (playerInZone && !robotActive && Input.GetKeyDown(KeyCode.E))
        {
            ActivateHorn();
        }

        if (robotActive && spawnedRobot != null)
        {
            // Wacht op de speler die voorbij de robot loopt in Z-richting
            if (!hasStartedPatrolling && player != null)
            {
                if (player.position.z > spawnedRobot.transform.position.z + 0.5f)
                {
                    hasStartedPatrolling = true;
                    targetPoint = pointB.position;
                }
                return;
            }

            // Robot beweegt heen en weer
            if (hasStartedPatrolling)
            {
                spawnedRobot.transform.position = Vector3.MoveTowards(
                    spawnedRobot.transform.position,
                    targetPoint,
                    moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(spawnedRobot.transform.position, targetPoint) < 0.1f)
                {
                    targetPoint = targetPoint == pointA.position ? pointB.position : pointA.position;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }

    private void ActivateHorn()
    {
        spawnedRobot = Instantiate(robotPrefab, pointA.position, Quaternion.identity);
        robotActive = true;
        hasStartedPatrolling = false;

        if (hornSound != null)
        {
            audioSource.Play();
        }
    }
}
