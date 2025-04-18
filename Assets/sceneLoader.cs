using UnityEngine;

public class sceneLoader : MonoBehaviour
{
    public string sceneToLoad;
    public float fadeDuration = 1f;
    private fadeManager fade;

    private void Start()
    {
        fade = FindObjectOfType<fadeManager>();
        if (fade == null)
        {
            Debug.LogWarning("fadeManager not found in scene. Scene transition will be instant.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

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