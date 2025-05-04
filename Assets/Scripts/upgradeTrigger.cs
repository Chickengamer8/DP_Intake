using UnityEngine;

public class upgradeTrigger : MonoBehaviour
{
    [Header("Prompt UI")]
    public GameObject promptUI; // UI dat verschijnt
    public string promptTextEnter = "Press E to obtain upgrade!";
    public string promptTextAfter = "Health and Stamina increased!";
    public float slideDuration = 0.5f;

    [Header("Upgrade Settings")]
    public float healthBonus = 25f;
    public float staminaBonus = 25f;

    private bool playerInTrigger = false;
    private bool upgradeCollected = false;

    private void Update()
    {
        if (playerInTrigger && !upgradeCollected && Input.GetKeyDown(KeyCode.E))
        {
            CollectUpgrade();
        }
    }

    private void CollectUpgrade()
    {
        upgradeCollected = true;

        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.IncreaseStats(healthBonus, staminaBonus);
        }

        // Je kunt hier eventueel een timer starten om de prompt weer uit te faden
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !upgradeCollected)
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}
