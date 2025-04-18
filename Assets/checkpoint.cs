using UnityEngine;

public class checkpoint : MonoBehaviour
{
    public Material inactiveMaterial;
    public Material activeMaterial;

    private Renderer checkpointRenderer;
    private bool isActivated = false;

    void Start()
    {
        checkpointRenderer = GetComponent<Renderer>();
        checkpointRenderer.material = inactiveMaterial;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            playerHealth playerHealthScript = other.GetComponent<playerHealth>();
            if (playerHealthScript != null)
            {
                playerHealthScript.SetCheckpoint(transform.position);
            }

            checkpointRenderer.material = activeMaterial;
            isActivated = true;
        }
    }
}
