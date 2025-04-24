using UnityEngine;
using UnityEngine.UI;

public class leverController : MonoBehaviour
{
    public GameObject targetObject; // Het object dat 90 graden draait
    public RectTransform promptUI; // De UI prompt die naar beneden moet sliden
    public Vector2 visiblePosition;
    public Vector2 hiddenPosition;
    public float slideSpeed = 5f;

    private bool playerInRange = false;
    private bool promptVisible = false;
    private bool isRotated = false;

    private void Start()
    {
        if (promptUI != null)
            promptUI.anchoredPosition = hiddenPosition;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            RotateTarget();
        }

        // Slide prompt in/out
        if (promptUI != null)
        {
            Vector2 targetPos = playerInRange ? visiblePosition : hiddenPosition;
            promptUI.anchoredPosition = Vector2.Lerp(promptUI.anchoredPosition, targetPos, Time.deltaTime * slideSpeed);
        }
    }

    private void RotateTarget()
    {
        if (targetObject != null && !isRotated)
        {
            targetObject.transform.Rotate(0f, 0f, 90f);
            isRotated = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
