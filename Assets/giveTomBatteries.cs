//using UnityEngine;

//public class giveTomBatteries : MonoBehaviour
//{
//    public mainSpotlightManager spotlightManager;
//    public GameObject objectToEnable;
//    private bool hasGiven = false;
//    private bool playerInZone = false;

//    private void Update()
//    {
//        if (playerInZone && !hasGiven && Input.GetKeyDown(KeyCode.E))
//        {
//            spotlightManager.tomHasBatteries = true;
//            hasGiven = true;

//            if (objectToEnable != null)
//                objectToEnable.SetActive(true);

//            Debug.Log("Tom now has batteries.");
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//            playerInZone = true;
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("Player"))
//            playerInZone = false;
//    }
//}
