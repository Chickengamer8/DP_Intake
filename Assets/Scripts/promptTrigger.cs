using UnityEngine;

public class promptTrigger : MonoBehaviour
{
    [Header("Prompt Settings")]
    public string promptMessage = "Press E to interact";
    public Sprite promptIcon;

    [Header("References")]
    public promptUIController promptController;
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && promptController != null)
        {
            promptController.ShowPrompt(promptMessage, promptIcon);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && promptController != null)
        {
            promptController.HidePrompt();
        }
    }
}