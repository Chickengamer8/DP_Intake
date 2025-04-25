using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    [Header("Checkpoint ID (optioneel)")]
    public int checkpointID;

    private SpriteRenderer spriteRenderer;
    private bool isActive = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && inactiveSprite != null)
            spriteRenderer.sprite = inactiveSprite;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            ActivateCheckpoint(other.transform);
        }
    }

    private void ActivateCheckpoint(Transform player)
    {
        isActive = true;

        if (spriteRenderer != null && activeSprite != null)
            spriteRenderer.sprite = activeSprite;

        // Zet checkpoint data
        CheckpointManager.lastCheckpoint = transform.position;
        CheckpointManager.hasCheckpoint = true;

        // Roep eventueel extra logica aan
        playerHealth health = player.GetComponent<playerHealth>();
        if (health != null)
        {
            health.SetCheckpoint(transform.position);
        }

        Debug.Log("Checkpoint " + checkpointID + " activated!");
    }
}
