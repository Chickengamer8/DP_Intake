using UnityEngine;

public class batteryPickup : MonoBehaviour
{
    public promptUI prompt;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Battery picked up");
        prompt.ShowMessage("+1 Battery");
        Destroy(gameObject);
    }
}