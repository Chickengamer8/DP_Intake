using UnityEngine;

public class bigBoxGrabDetector : MonoBehaviour
{
    private moveBigBox bigBox;

    void Start()
    {
        bigBox = GetComponentInParent<moveBigBox>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bigBox.SetPlayerInZone(true, other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bigBox.SetPlayerInZone(false, null);
        }
    }
}
