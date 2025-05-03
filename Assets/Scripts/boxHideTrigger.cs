using UnityEngine;

public class boxHideTrigger : MonoBehaviour
{
    public string targetLayer = "Hide";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            Debug.Log("Box entered hide trigger!");

            // Zet layer
            int layerIndex = LayerMask.NameToLayer(targetLayer);
            if (layerIndex == -1)
            {
                Debug.LogError($"Layer '{targetLayer}' bestaat niet!");
                return;
            }

            other.gameObject.layer = layerIndex;
            Debug.Log($"Box layer veranderd naar '{targetLayer}'.");
        }
    }

}
