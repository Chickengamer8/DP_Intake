using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button continueButton;
    public Button quitButton;

    public static bool isGamePaused = false;  // 🔥 GLOBAL

    private void Start()
    {
        pauseMenuPanel.SetActive(false);

        continueButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenuPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        isGamePaused = false;  // Zorg dat het reset!
        globalPlayerStats.instance.currentHealth = 100f;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
