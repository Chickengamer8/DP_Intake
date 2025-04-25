using UnityEngine;

public class eyeVision : MonoBehaviour
{
    [Header("Player Tracking")]
    public Transform player;
    public float lookRadius = 0.5f;
    public float followSpeed = 5f;

    [Header("Detection Settings")]
    public float detectionRange = 20f;
    public float damageInterval = 1f;
    public float damagePerTick = 10f;
    public LayerMask hideLayer;

    private playerHealth playerHealth;
    private float damageTimer;

    [Header("Ramp-up Damage Settings")]
    public float damageMultiplier = 1f;
    public float damageIncreaseRate = 0.5f;
    public float maxDamageMultiplier = 5f;
    public float recoveryRate = 1f;

    public bool canSeePlayer = true;

    void Start()
    {
        if (player != null)
            playerHealth = player.GetComponent<playerHealth>();

        damageTimer = damageInterval;
    }

    void Update()
    {
        if (player == null || playerHealth == null)
            return;

        if (canSeePlayer)
        {
            damageMultiplier += damageIncreaseRate * Time.deltaTime;
            damageMultiplier = Mathf.Clamp(damageMultiplier, 1f, maxDamageMultiplier);

            ApplyDamageOverTime();
        }
        else
        {
            damageMultiplier -= recoveryRate * Time.deltaTime;
            damageMultiplier = Mathf.Clamp(damageMultiplier, 1f, maxDamageMultiplier);
            damageTimer = damageInterval;
        }
    }

    void ApplyDamageOverTime()
    {
        damageTimer -= Time.deltaTime;

        if (damageTimer <= 0f)
        {
            float scaledDamage = damagePerTick * damageMultiplier;
            playerHealth.TakeDamage(scaledDamage);
            damageTimer = damageInterval;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
