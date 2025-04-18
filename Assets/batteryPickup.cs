using UnityEngine;

public class batteryPickup : MonoBehaviour
{
    public promptUI prompt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            prompt.ShowMessage("+1 Battery");
            Destroy(gameObject);
        }
    }
}