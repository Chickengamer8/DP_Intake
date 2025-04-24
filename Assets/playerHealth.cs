using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public static class CheckpointManager
{
    public static Vector3 lastCheckpoint = Vector3.zero;
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

        // Als er een checkpoint is opgeslagen, spawn daar
        if (CheckpointManager.lastCheckpoint != Vector3.zero)
        {
            transform.position = CheckpointManager.lastCheckpoint;
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
    }

    public void Die()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Zet velocity naar nul en geef ��n korte sprong
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);

        // Zet movement tijdelijk uit
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // Wacht even zodat speler "valt"
        yield return new WaitForSeconds(1.5f);

        // Laad de scene opnieuw
        SceneManager.LoadScene(loadScene);
    }
}