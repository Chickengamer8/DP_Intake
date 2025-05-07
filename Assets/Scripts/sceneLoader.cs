using UnityEngine;

public class sceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;
    public float fadeDuration = 1f;
    public fadeManager fade;

    [Header("Next Scene Spawn Point")]
    public Vector3 nextSceneSpawnPoint;  // <- vul dit in met de gewenste spawn locatie van de volgende scene

    private void Start()
    {
        if (fade == null)
        {
            Debug.LogWarning("fadeManager not found in scene. Scene transition will be instant.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Zet alvast het nieuwe checkpoint voor de volgende scene
        if (globalPlayerStats.instance != null)
        {
            globalPlayerStats.instance.SetCheckpoint(nextSceneSpawnPoint);
            Debug.Log($"[sceneLoader] Next scene spawn point set to: {nextSceneSpawnPoint}");
        }
        else
        {
            Debug.LogWarning("[sceneLoader] Geen GlobalPlayerStats gevonden om checkpoint te zetten.");
        }

        if (fade != null)
        {
            fade.StartSceneTransition(sceneToLoad, fadeDuration);
        }
        else
        {
            // Fallback zonder fade
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}
