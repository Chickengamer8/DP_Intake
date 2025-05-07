using UnityEngine;
using UnityEngine.SceneManagement;

public class levelSelectButton : MonoBehaviour
{
    [Header("Level Info")]
    public string sceneName;
    public Vector3 spawnPosition;

    public void LoadThisLevel()
    {
        Debug.Log($"[LevelSelectButton] Loading {sceneName} at {spawnPosition}");

        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.SetCheckpoint(spawnPosition);
        }
        else
        {
            Debug.LogWarning("[LevelSelectButton] No globalPlayerStats found!");
        }

        SceneManager.LoadScene(sceneName);
    }
}
