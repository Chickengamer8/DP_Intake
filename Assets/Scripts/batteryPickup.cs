using UnityEngine;

public class batteryPickup : MonoBehaviour
{
    public promptUI prompt;
    public GameObject batteryUI;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Battery picked up");
        prompt.ShowMessage("+1 Battery");
        Destroy(gameObject);
        batteryUI.SetActive(true);
    }
}