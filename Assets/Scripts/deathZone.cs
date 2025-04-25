using UnityEngine;

public class deathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth playerHealthScript = other.GetComponent<playerHealth>();
            if (playerHealthScript != null)
            {
                playerHealthScript.Die();
            }
        }
    }
}
