using UnityEngine;
using System.IO;

public class checkpoint : MonoBehaviour
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

        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.SetCheckpoint(transform.position);
        }

        SaveCheckpointToFile();

        Debug.Log($"[Checkpoint] Activated checkpoint {checkpointID} at {transform.position}");
    }
    private void SaveCheckpointToFile()
    {
        // Zorg dat de Data-folder bestaat
        string folderPath = Path.Combine(Application.dataPath, "Data");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Maak het volledige pad aan
        string savePath = Path.Combine(folderPath, "checkpointSave.txt");

        string levelID = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Vector3 pos = globalPlayerStats.instance.lastCheckpointPosition;

        string[] lines =
        {
        $"LevelID: {levelID}",
        $"Checkpoint: {pos.x},{pos.y},{pos.z}"
    };

        File.WriteAllLines(savePath, lines);

        Debug.Log($"[Checkpoint] Saved checkpoint to {savePath}");
    }
}
