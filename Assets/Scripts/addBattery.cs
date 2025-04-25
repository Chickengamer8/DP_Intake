using UnityEngine;

public class addBattery : MonoBehaviour
{
    public GameObject[] batterySlots; // Alle mogelijke visuele plaatsen
    private bool playerInZone = false;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            TryInsertBattery();
        }
    }

    void TryInsertBattery()
    {
        if (robotState.Instance.insertedBatteries >= batterySlots.Length) return;

        int nextSlotIndex = robotState.Instance.insertedBatteries;

        batterySlots[nextSlotIndex].SetActive(true);
        robotState.Instance.InsertBattery();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }
}