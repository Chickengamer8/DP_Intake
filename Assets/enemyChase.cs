using UnityEngine;
using System.Collections;

public class enemyChase : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float killDelay = 2f; // Tijd voor de speler doodgaat
    public Transform player;

    private Rigidbody rb;
    private bool hasHitPlayer = false;
    private Vector3 originalScale;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (hasHitPlayer || player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 movement = new Vector3(direction.x, 0f, 0f) * moveSpeed;

        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, 0f);

        // Flip de vijand afhankelijk van richting
        if (direction.x < 0f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitPlayer) return;

        if (other.CompareTag("Player"))
        {
            playerMovement movement = other.GetComponent<playerMovement>();
            playerHealth health = other.GetComponent<playerHealth>();

            if (movement != null) movement.canMove = false;
            if (health != null) StartCoroutine(DelayedKill(health));

            hasHitPlayer = true;
            Debug.Log("Vijand heeft speler geraakt — animatie start & dood volgt.");
        }
    }

    IEnumerator DelayedKill(playerHealth health)
    {
        yield return new WaitForSeconds(killDelay);

        if (health != null)
        {
            health.TakeDamage(health.maxHealth); // Instant dood na delay
        }
    }
}
