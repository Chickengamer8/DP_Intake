using UnityEngine;
using TMPro;

public class hideAttackHornSummon : MonoBehaviour
{
    [Header("References")]
    public GameObject robot;
    public GameObject hideVisual;
    public GameObject attackVisual;
    public Transform player;
    public Transform[] enemies;
    public Transform movementLimitPoint;

    [Header("Horn Trigger")]
    public KeyCode activateKey = KeyCode.E;
    public RectTransform promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E to blow the horn";
    public Vector2 visiblePosition;
    public Vector2 hiddenPosition;
    public float slideSpeed = 5f;

    [Header("Settings")]
    public float followSpeed = 3f;
    public float attackSpeed = 5f;
    public float followDistance = 2f;
    public float enemyDetectionRange = 5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hornSound;

    private bool playerInZone = false;
    private bool robotActive = false;
    private bool inAttackMode = false;
    private GameObject targetEnemy = null;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(activateKey) && !robotActive)
        {
            ActivateRobot();
        }

        HandlePromptUI();

        if (robotActive)
        {
            if (inAttackMode)
                HandleAttackMode();
            else
            {
                SearchForEnemies();
                FollowPlayer();
            }
        }
    }

    private void HandlePromptUI()
    {
        if (promptUI != null)
        {
            Vector2 targetPos = playerInZone ? visiblePosition : hiddenPosition;
            promptUI.anchoredPosition = Vector2.Lerp(promptUI.anchoredPosition, targetPos, Time.deltaTime * slideSpeed);
        }

        if (promptText != null)
        {
            promptText.text = playerInZone ? promptMessage : "";
        }
    }

    private void ActivateRobot()
    {
        robotActive = true;
        inAttackMode = false;

        if (audioSource != null && hornSound != null)
        {
            audioSource.PlayOneShot(hornSound);
        }

        if (hideVisual != null) hideVisual.SetActive(true);
        if (attackVisual != null) attackVisual.SetActive(false);
    }

    private void FollowPlayer()
    {
        Vector3 targetPos = player.position + Vector3.left * followDistance;

        if (movementLimitPoint != null)
        {
            // Robot mag speler volgen, maar niet verder dan het limit point
            if (targetPos.x > movementLimitPoint.position.x)
            {
                targetPos.x = movementLimitPoint.position.x;
            }
        }

        // Verplaats robot alleen als de afstand groter is dan een kleine drempel
        if (Vector3.Distance(robot.transform.position, targetPos) > 0.1f)
        {
            robot.transform.position = Vector3.MoveTowards(robot.transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }

    private void SearchForEnemies()
    {
        foreach (Transform enemy in enemies)
        {
            if (enemy != null && Vector3.Distance(robot.transform.position, enemy.position) < enemyDetectionRange)
            {
                inAttackMode = true;
                if (hideVisual != null) hideVisual.SetActive(false);
                if (attackVisual != null) attackVisual.SetActive(true);
                targetEnemy = enemy.gameObject;
                break;
            }
        }
    }

    private void HandleAttackMode()
    {
        if (targetEnemy == null)
        {
            ReturnToPlayer();
            return;
        }

        robot.transform.position = Vector3.MoveTowards(robot.transform.position, targetEnemy.transform.position, attackSpeed * Time.deltaTime);

        if (Vector3.Distance(robot.transform.position, targetEnemy.transform.position) < 1f)
        {
            Destroy(targetEnemy);
            targetEnemy = null;
        }
    }

    private void ReturnToPlayer()
    {
        inAttackMode = false;
        if (hideVisual != null) hideVisual.SetActive(true);
        if (attackVisual != null) attackVisual.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }
}
