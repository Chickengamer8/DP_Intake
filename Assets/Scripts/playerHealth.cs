using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float regenDelay = 3f;
    public float regenRate = 5f;

    [Header("Fade Panel Settings")]
    public Image fadePanel;

    [Header("References")]
    public Animator playerAnimator;

    private playerMovement playerMovementScript;
    private Rigidbody rb;
    private float timeSinceLastDamage = 0f;
    private bool isDead = false;

    private void Start()
    {
        playerMovementScript = GetComponent<playerMovement>();
        rb = GetComponent<Rigidbody>();

        if (globalPlayerStats.instance != null && globalPlayerStats.instance.currentHealth <= 0f)
            globalPlayerStats.instance.currentHealth = globalPlayerStats.instance.maxHealth;

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

            UpdateFadePanel();
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

        UpdateFadePanel();
    }

    private void RegenerateHealth()
    {
        float maxHealth = globalPlayerStats.instance.maxHealth;

        globalPlayerStats.instance.currentHealth += regenRate * Time.deltaTime;
        globalPlayerStats.instance.currentHealth = Mathf.Clamp(globalPlayerStats.instance.currentHealth, 0f, maxHealth);
    }

    private void UpdateFadePanel()
    {
        if (fadePanel == null || globalPlayerStats.instance == null) return;

        float maxHealth = (globalPlayerStats.instance.maxHealth / 2);
        float currentHealth = globalPlayerStats.instance.currentHealth;

        float healthRatio = Mathf.Clamp01(currentHealth / maxHealth);
        float targetAlpha = 1f - healthRatio;

        Color c = fadePanel.color;
        fadePanel.color = new Color(c.r, c.g, c.b, targetAlpha);
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (playerMovementScript != null)
            playerMovementScript.canMove = false;

        if (playerAnimator != null)
            playerAnimator.SetBool("Death", true);

        rb.linearVelocity = Vector3.zero;

        gameOverManager gameOver = FindFirstObjectByType<gameOverManager>();
        if (gameOver != null)
        {
            gameOver.ShowGameOverScreen(this);
        }
        else
        {
            Debug.LogWarning("[playerHealth] No gameOverManager found in scene!");
        }
    }

    public void Respawn()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Death", false);
            playerAnimator.SetBool("Respawn", true);
        }

        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.currentHealth = globalPlayerStats.instance.maxHealth;
        }

        transform.position = globalPlayerStats.instance.lastCheckpointPosition;
        isDead = false;

        if (playerMovementScript != null)
            playerMovementScript.canMove = true;

        UpdateFadePanel();

        StartCoroutine(ResetRespawnBool());
    }

    private IEnumerator ResetRespawnBool()
    {
        yield return new WaitForSeconds(5f);
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Respawn", false);
        }
    }
}
