using UnityEngine;
using UnityEngine.UI;

public class playerUI : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBarImage;
    public Image staminaBarImage;

    [Header("Player References")]
    public playerHealth playerHealth;
    public playerMovement playerMovement;

    void Update()
    {
        UpdateHealthBar();
        UpdateStaminaBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarImage != null && playerHealth != null)
        {
            float maxHealth = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxHealth : 100f;
            float healthPercent = playerHealth.currentHealth / maxHealth;
            healthBarImage.fillAmount = Mathf.Clamp01(healthPercent);
        }
    }

    private void UpdateStaminaBar()
    {
        if (staminaBarImage != null && playerMovement != null)
        {
            float maxStamina = globalPlayerStats.instance != null ? globalPlayerStats.instance.maxStamina : 100f;
            float staminaPercent = playerMovement.currentStamina / maxStamina;
            staminaBarImage.fillAmount = Mathf.Clamp01(staminaPercent);
        }
    }
}
