using UnityEngine;
using System.Collections;

public class enemyChase : MonoBehaviour
{
    [Header("Chase Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public float killDelay = 2f;
    public float killAnimationDelay = 1f;

    public Transform player;

    [Header("Kill Setup")]
    public Transform killPosition;

    [Header("Animation")]
    public Animator enemyAnimator;

    [Header("Death Setup")]
    public Collider triggerColliderToDisable;  // sleep hier in de specifieke trigger collider

    private Rigidbody rb;
    private bool hasHitPlayer = false;
    private bool isDead = false;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDead || player == null)
        {
            if (enemyAnimator != null)
                enemyAnimator.SetBool("isWalking", false);

            rb.linearVelocity = Vector3.zero;  // stop beweging volledig
            return;
        }

        if (hasHitPlayer)
        {
            if (enemyAnimator != null)
                enemyAnimator.SetBool("isWalking", false);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 newVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            rb.linearVelocity = newVelocity;
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitPlayer || isDead) return;

        if (other.CompareTag("Player"))
        {
            playerMovement movement = other.GetComponent<playerMovement>();
            playerHealth health = other.GetComponent<playerHealth>();

            if (movement != null) movement.canMove = false;

            if (killPosition != null)
                other.transform.position = killPosition.position;

            if (health != null) StartCoroutine(DelayedKill(health));

            hasHitPlayer = true;

            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool("isWalking", false);
                enemyAnimator.SetTrigger("killPlayer");
            }

            Debug.Log("[enemyChase] Player hit. Teleported to kill position and triggering animation...");
        }
    }

    private IEnumerator DelayedKill(playerHealth health)
    {
        yield return new WaitForSeconds(killDelay);

        if (health != null)
        {
            float maxHealth = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxHealth : 100f;
            health.TakeDamage(maxHealth);
        }

        yield return new WaitForSeconds(killAnimationDelay);

        if (enemyAnimator != null)
            enemyAnimator.SetBool("killFinished", true);

        DisableEnemy();
    }

    public void DisableEnemy()
    {
        isDead = true;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (triggerColliderToDisable != null)
        {
            triggerColliderToDisable.enabled = false;
        }
        else
        {
            Debug.LogWarning("[enemyChase] No triggerColliderToDisable assigned!");
        }

        gameObject.layer = LayerMask.NameToLayer("Hide");

        Debug.Log("[enemyChase] Enemy is now dead: stopped movement, disabled trigger collider, set to Hide layer.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}