using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float regenDelay = 3f;
    public float regenRate = 5f;

    [Header("Health UI")]
    public Image healthBarFill;

    private playerMovement playerMovementScript;
    private Rigidbody rb;
    private float timeSinceLastDamage = 0f;
    private bool isDead = false;

    private void Start()
    {
        playerMovementScript = GetComponent<playerMovement>();
        rb = GetComponent<Rigidbody>();

        // Initialiseer currentHealth als die nog niet gezet is
        if (globalPlayerStats.instance != null)
        {
            if (globalPlayerStats.instance.currentHealth <= 0f)
                globalPlayerStats.instance.currentHealth = globalPlayerStats.instance.maxHealth;
        }

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

        if (globalPlayerStats.instance != null)
        {
            float maxHealth = globalPlayerStats.instance.maxHealth;

            if (timeSinceLastDamage >= regenDelay && globalPlayerStats.instance.currentHealth < maxHealth)
            {
                RegenerateHealth();
            }

            UpdateHealthUI();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || globalPlayerStats.instance == null) return;

        float maxHealth = globalPlayerStats.instance.maxHealth;

        globalPlayerStats.instance.currentHealth -= damage;
        globalPlayerStats.instance.currentHealth = Mathf.Clamp(globalPlayerStats.instance.currentHealth, 0f, maxHealth);
        timeSinceLastDamage = 0f;

        if (globalPlayerStats.instance.currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthUI();
    }

    private void RegenerateHealth()
    {
        float maxHealth = globalPlayerStats.instance.maxHealth;

        globalPlayerStats.instance.currentHealth += regenRate * Time.deltaTime;
        globalPlayerStats.instance.currentHealth = Mathf.Clamp(globalPlayerStats.instance.currentHealth, 0f, maxHealth);
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null && globalPlayerStats.instance != null)
        {
            float maxHealth = globalPlayerStats.instance.maxHealth;
            healthBarFill.fillAmount = globalPlayerStats.instance.currentHealth / maxHealth;
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

        respawnManager.RespawnPlayer(transform);

        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.currentHealth = globalPlayerStats.instance.maxHealth;
        }

        isDead = false;

        if (playerMovementScript != null)
        {
            playerMovementScript.canMove = true;
        }
    }
}
