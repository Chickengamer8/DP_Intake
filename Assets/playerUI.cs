using UnityEngine;
using UnityEngine.UI;

public class playerUI : MonoBehaviour
{
    [Header("UI Reference")]
    public Image healthBarImage;    // Sleep hier de health bar Image in
    public Image staminaBarImage;   // Sleep hier de stamina bar Image in

    [Header("Health Reference")]
    public playerHealth playerHealth;   // Sleep hier het playerHealth script in

    [Header("Stamina Reference")]
    public playerMovement playerMovement; // Sleep hier het playerMovement script in

    void Update()
    {
        UpdateHealthBar();
        UpdateStaminaBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarImage != null && playerHealth != null)
        {
            float healthPercent = playerHealth.currentHealth / playerHealth.maxHealth;
            healthBarImage.fillAmount = Mathf.Clamp01(healthPercent);
        }
    }

    void UpdateStaminaBar()
    {
        if (staminaBarImage != null && playerMovement != null)
        {
            float staminaPercent = playerMovement.currentStamina / playerMovement.maxStamina;
            staminaBarImage.fillAmount = Mathf.Clamp01(staminaPercent);
        }
    }
}
