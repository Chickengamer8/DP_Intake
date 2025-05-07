using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameOverManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverPanel;
    public Button continueButton;
    public Button quitButton;

    private playerHealth playerHealthScript;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        continueButton.onClick.AddListener(HandleContinue);
        quitButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ShowGameOverScreen(playerHealth player)
    {
        playerHealthScript = player;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    private void HandleContinue()
    {
        if (playerHealthScript != null)
        {
            Time.timeScale = 1f;
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            playerHealthScript.Respawn();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log("[GameOverManager] Player respawned via Game Over screen.");
        }
        else
        {
            Debug.LogWarning("[GameOverManager] No playerHealthScript reference found.");
        }
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Zorg dat tijd niet bevroren blijft
        SceneManager.LoadScene("MainMenu"); // Zorg dat je scene naam klopt
    }
}
