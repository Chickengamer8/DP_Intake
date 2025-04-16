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

    private playerHealth playerHealth;
    private playerHideDetector hideDetector;
    private float damageTimer;

    [Header("Ramp-up Damage Settings")]
    public float damageMultiplier = 1f;
    public float damageIncreaseRate = 0.5f; // Hoe snel damage stijgt per seconde
    public float maxDamageMultiplier = 5f;
    public float recoveryRate = 1f; // Hoe snel het afneemt als je verborgen bent

    void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<playerHealth>();
            hideDetector = player.GetComponent<playerHideDetector>();
        }

        damageTimer = damageInterval;
    }

    void Update()
    {
        if (player == null || playerHealth == null || hideDetector == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool isInSight = distance <= detectionRange && !hideDetector.isHidden;

        if (isInSight)
        {
            // Bouw schade op zolang speler zichtbaar is
            damageMultiplier += damageIncreaseRate * Time.deltaTime;
            damageMultiplier = Mathf.Clamp(damageMultiplier, 1f, maxDamageMultiplier);

            ApplyDamageOverTime();
        }
        else
        {
            // Reset langzaam naar normale waarde
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
