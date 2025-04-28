using UnityEngine;

public class colliderLayerSwitcher : MonoBehaviour
{
    [Header("Settings")]
    public string targetLayerOnEnter = "Box";    // Layer om naar te switchen bij binnenkomen
    public string targetLayerOnExit = "Hide";    // Layer om naar te switchen bij verlaten
    public Material solidMaterial;               // Materiaal als de box solid wordt
    public Material originalMaterial;            // Oorspronkelijke materiaal (Hide toestand)

    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform.parent;
        if (parent != null && parent.gameObject.layer == LayerMask.NameToLayer(targetLayerOnExit))
        {
            parent.gameObject.layer = LayerMask.NameToLayer(targetLayerOnEnter);
            Debug.Log($"[ColliderLayerAndMaterialSwitcher] {parent.name} switched to {targetLayerOnEnter}");

            // Probeer de Renderer te pakken en material te veranderen
            Renderer renderer = parent.GetComponentInChildren<Renderer>();
            if (renderer != null && solidMaterial != null)
            {
                renderer.material = solidMaterial;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform parent = other.transform.parent;
        if (parent != null && parent.gameObject.layer == LayerMask.NameToLayer(targetLayerOnEnter))
        {
            parent.gameObject.layer = LayerMask.NameToLayer(targetLayerOnExit);
            Debug.Log($"[ColliderLayerAndMaterialSwitcher] {parent.name} switched back to {targetLayerOnExit}");

            // Renderer en material terugzetten
            Renderer renderer = parent.GetComponentInChildren<Renderer>();
            if (renderer != null && originalMaterial != null)
            {
                renderer.material = originalMaterial;
            }
        }
    }
}
