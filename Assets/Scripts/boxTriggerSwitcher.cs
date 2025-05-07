using UnityEngine;

public class boxTriggerSwitcher : MonoBehaviour
{
    [Header("Box Setup")]
    public GameObject dynamicBox;   // de bewegende box boven
    public GameObject staticBox;    // de vaste box onder, die wordt geactiveerd

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == dynamicBox)
        {
            Debug.Log("[BoxTriggerSwitcher] Dynamic box hit trigger, switching to static box.");

            dynamicBox.SetActive(false);
            staticBox.SetActive(true);
        }
    }
}
