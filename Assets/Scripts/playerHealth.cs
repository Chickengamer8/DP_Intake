using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public static class CheckpointManager
{
    public static Vector3 lastCheckpoint = Vector3.zero;
    public static bool hasCheckpoint = false;
}

public class playerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public string loadScene;

    [Header("Regen Settings")]
    public float regenDelay = 3f;
    public float regenRate = 5f;
    private float timeSinceLastDamage = 0f;

    [Header("Health UI")]
    public Image healthBarFill;

    private playerMovement playerMovementScript;
    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth;
        playerMovementScript = GetComponent<playerMovement>();
        rb = GetComponent<Rigidbody>();

        // Zoek het spawnPoint als er geen actief checkpoint is
        if (CheckpointManager.hasCheckpoint)
        {
            transform.position = CheckpointManager.lastCheckpoint;
        }
        else
        {
            GameObject spawn = GameObject.Find("spawnPoint");
            if (spawn != null)
            {
                transform.position = spawn.transform.position;
            }
            else
            {
                Debug.LogWarning("Geen spawnPoint gevonden!");
                if (spawn != null)
                    transform.position = spawn.transform.position;
            }
        }
    }

    void Update()
    {
        timeSinceLastDamage += Time.deltaTime;

        if (timeSinceLastDamage >= regenDelay && currentHealth < maxHealth)
        {
            RegenerateHealth();
        }

        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        timeSinceLastDamage = 0f;

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthUI();
    }

    void RegenerateHealth()
    {
        currentHealth += regenRate * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        CheckpointManager.lastCheckpoint = newCheckpoint;
        CheckpointManager.hasCheckpoint = true;
    }

    public void Die()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(loadScene);
    }
}
