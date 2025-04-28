using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float currentHealth;
    public float regenDelay = 3f;
    public float regenRate = 5f;
    public string loadScene; // Niet meer gebruikt bij respawn, maar optioneel als fallback.

    [Header("Health UI")]
    public Image healthBarFill;

    private playerMovement playerMovementScript;
    private Rigidbody rb;
    private float timeSinceLastDamage = 0f;
    private bool isDead = false;

    private void Start()
    {
        float maxHealth = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxHealth : 100f;
        currentHealth = maxHealth;

        playerMovementScript = GetComponent<playerMovement>();
        rb = GetComponent<Rigidbody>();

        // Spawn op checkpoint als die bestaat
        if (globalPlayerStats.instance != null && globalPlayerStats.instance.hasCheckpoint)
        {
            transform.position = globalPlayerStats.instance.lastCheckpointPosition;
        }
        else
        {
            GameObject spawn = GameObject.Find("spawnPoint");
            if (spawn != null)
                transform.position = spawn.transform.position;
            else
                Debug.LogWarning("[playerHealth] Geen spawnPoint gevonden in scene!");
        }
    }

    private void Update()
    {
        if (isDead) return;

        timeSinceLastDamage += Time.deltaTime;

        float maxHealth = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxHealth : 100f;

        if (timeSinceLastDamage >= regenDelay && currentHealth < maxHealth)
        {
            RegenerateHealth();
        }

        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        float maxHealth = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxHealth : 100f;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        timeSinceLastDamage = 0f;

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthUI();
    }

    private void RegenerateHealth()
    {
        float maxHealth = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxHealth : 100f;

        currentHealth += regenRate * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            float maxHealth = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxHealth : 100f;
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    public void Die()
    {
        if (!isDead)
        {
            isDead = true;
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.canMove = false;
        }

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);

        yield return new WaitForSeconds(1.5f);

        // Gebruik respawnManager om te respawnen
        respawnManager.RespawnPlayer(transform);

        // Reset local player health
        float maxHealth = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxHealth : 100f;
        currentHealth = maxHealth;
        isDead = false;

        if (playerMovementScript != null)
        {
            playerMovementScript.canMove = true;
        }
    }
}
