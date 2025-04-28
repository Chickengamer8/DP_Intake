using UnityEngine;

public static class respawnManager
{
    public static void RespawnPlayer(Transform playerTransform)
    {
        if (globalPlayerStats.instance != null && globalPlayerStats.instance.hasCheckpoint)
        {
            playerTransform.position = globalPlayerStats.instance.lastCheckpointPosition;
            Debug.Log($"[respawnManager] Respawned player at checkpoint: {globalPlayerStats.instance.lastCheckpointPosition}");
        }
        else
        {
            GameObject spawn = GameObject.Find("spawnPoint");
            if (spawn != null)
            {
                playerTransform.position = spawn.transform.position;
                Debug.Log("[respawnManager] Respawned player at spawnPoint.");
            }
            else
            {
                Debug.LogWarning("[respawnManager] No checkpoint or spawnPoint found! Player stays at current position.");
            }
        }
    }
}
