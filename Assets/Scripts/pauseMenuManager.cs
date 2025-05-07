using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button continueButton;
    public Button quitButton;

    private bool isPaused = false;

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
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenuPanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Make sure time is running
        SceneManager.LoadScene("MainMenu");
    }
}
