using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Regen Settings")]
    public float regenDelay = 3f;            // Seconden wachten na damage
    public float regenRate = 5f;             // HP per seconde
    private float timeSinceLastDamage = 0f;

    [Header("Health UI")]
    public Image healthBarFill;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Regen timer opbouwen
        timeSinceLastDamage += Time.deltaTime;

        // Als genoeg tijd is verstreken, begin met healen
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

        timeSinceLastDamage = 0f; // Reset regen timer

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

    void Die()
    {
        Debug.Log("Speler is dood.");
        // TODO: Respawn of Game Over logic
    }
}
