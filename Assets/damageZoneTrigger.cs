using UnityEngine;

public class damageZoneTrigger : MonoBehaviour
{
    public float damagePerSecond = 20f;
    public bool canDealDamage = true;  // ← nieuw: aan/uit schakelaar

    private void OnTriggerStay(Collider other)
    {
        if (!canDealDamage) return;  // ← check toegevoegd

        if (!other.CompareTag("Player")) return;

        playerHealth playerHealthScript = other.GetComponent<playerHealth>();
        if (playerHealthScript != null)
        {
            playerHealthScript.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}