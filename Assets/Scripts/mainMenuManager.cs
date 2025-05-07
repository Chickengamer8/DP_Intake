using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class mainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string fallbackScene = "GameScene";

    [Header("New Game Settings")]
    public Vector3 newGameSpawnPosition = new Vector3(0f, 1f, 0f); // Pas aan in Inspector of hardcode hier

    public void NewGame()
    {
        Debug.Log("[MainMenu] Starting new game...");

        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.SetCheckpoint(newGameSpawnPosition);
            Debug.Log($"[MainMenu] New Game: Set checkpoint to {newGameSpawnPosition}");
        }
        else
        {
            Debug.LogWarning("[MainMenu] Geen GlobalPlayerStats gevonden bij New Game!");
        }

        SceneManager.LoadScene(fallbackScene);
    }

    public void ContinueGame()
    {
        Debug.Log("[MainMenu] Attempting to continue...");

        string savePath = Path.Combine(Application.dataPath, "Data/checkpointSave.txt");

        if (File.Exists(savePath))
        {
            string[] lines = File.ReadAllLines(savePath);
            string levelID = fallbackScene;
            Vector3 checkpoint = Vector3.zero;

            foreach (string line in lines)
            {
                if (line.StartsWith("LevelID:"))
                {
                    levelID = line.Substring("LevelID:".Length).Trim();
                }
                else if (line.StartsWith("Checkpoint:"))
                {
                    string coords = line.Substring("Checkpoint:".Length).Trim();
                    string[] parts = coords.Split(',');

                    if (parts.Length == 3 &&
                        float.TryParse(parts[0], out float x) &&
                        float.TryParse(parts[1], out float y) &&
                        float.TryParse(parts[2], out float z))
                    {
                        checkpoint = new Vector3(x, y, z);
                    }
                }
            }

            if (globalPlayerStats.instance != null)
            {
                globalPlayerStats.instance.lastCheckpointPosition = checkpoint;
                globalPlayerStats.instance.hasCheckpoint = true;
                Debug.Log($"[MainMenu] Loading level: {levelID} with checkpoint at {checkpoint}");
            }
            else
            {
                Debug.LogWarning("[MainMenu] Geen GlobalPlayerStats gevonden, checkpoint wordt niet gezet!");
            }

            SceneManager.LoadScene(levelID);
        }
        else
        {
            Debug.LogWarning($"[MainMenu] No save file found at {savePath}, loading fallback scene.");

            if (globalPlayerStats.instance != null)
            {
                globalPlayerStats.instance.SetCheckpoint(newGameSpawnPosition);
                Debug.Log($"[MainMenu] No save file: fallback checkpoint set to {newGameSpawnPosition}");
            }
            else
            {
                Debug.LogWarning("[MainMenu] Geen GlobalPlayerStats gevonden voor fallback!");
            }

            SceneManager.LoadScene(fallbackScene);
        }
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenu] Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
