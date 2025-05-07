using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class checkpoint : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    [Header("Checkpoint ID (optioneel)")]
    public int checkpointID;

    [Header("Level State Changes")]
    public List<GameObject> objectsToEnable = new List<GameObject>();
    public List<GameObject> objectsToDisable = new List<GameObject>();
    public List<Transform> objectsToRotate = new List<Transform>();
    public List<Vector3> rotationAngles = new List<Vector3>(); // ✅ XYZ per item

    [Header("Object Transforms")]
    public List<Transform> objectsToTransform = new List<Transform>();
    public List<Vector3> newPositions = new List<Vector3>();

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
        ApplyLevelStateChanges();

        Debug.Log($"[Checkpoint] Activated checkpoint {checkpointID} at {transform.position}");
    }

    private void SaveCheckpointToFile()
    {
        string folderPath = Path.Combine(Application.dataPath, "Data");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

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

    private void ApplyLevelStateChanges()
    {
        // ✅ Enable objects
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // ✅ Disable objects
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // ✅ Rotate objects (XYZ)
        for (int i = 0; i < objectsToRotate.Count; i++)
        {
            Transform target = objectsToRotate[i];
            if (target != null)
            {
                Vector3 rotationAmount = (i < rotationAngles.Count) ? rotationAngles[i] : Vector3.zero;
                target.Rotate(rotationAmount); // XYZ tegelijk
            }
        }

        // ✅ Move objects
        for (int i = 0; i < objectsToTransform.Count; i++)
        {
            Transform target = objectsToTransform[i];
            if (target != null)
            {
                Vector3 newPos = (i < newPositions.Count) ? newPositions[i] : target.position;
                target.position = newPos;
            }
        }
    }
}
