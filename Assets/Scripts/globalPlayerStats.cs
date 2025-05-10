using UnityEngine;

public class globalPlayerStats : MonoBehaviour
{
    public static globalPlayerStats instance;

    [Header("Player Stats")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public bool isHiding = true;
    public bool isInCoverWall;

    [Header("Checkpoint Data")]
    public Vector3 lastCheckpointPosition;
    public bool hasCheckpoint = false;
    public float currentHealth;

    private void Awake()
    {
        if (currentHealth <= 0f)
            currentHealth = maxHealth;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IncreaseStats(float healthAmount, float staminaAmount)
    {
        maxHealth += healthAmount;
        maxStamina += staminaAmount;
        Debug.Log($"[GlobalPlayerStats] Health +{healthAmount}, Stamina +{staminaAmount}");
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
        Debug.Log($"[GlobalPlayerStats] New checkpoint set at {position}");
    }
}
