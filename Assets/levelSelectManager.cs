using UnityEngine;
using UnityEngine.SceneManagement;

public class levelSelectManager : MonoBehaviour
{
    public void LoadLevelWithSpawn(string sceneName, Vector3 spawnPosition)
    {
        Debug.Log($"[LevelSelect] Loading level: {sceneName} at spawn {spawnPosition}");

        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.SetCheckpoint(spawnPosition);
        }
        else
        {
            Debug.LogWarning("[LevelSelect] No globalPlayerStats instance found!");
        }

        SceneManager.LoadScene(sceneName);
    }
}