using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class leverInteract : MonoBehaviour
{
    [Header("Prompt Settings")]
    public GameObject promptUI;
    public string promptText = "Press E to pull the lever";
    public TextMeshProUGUI promptTextComponent;
    public Vector3 slideInPosition;
    public Vector3 slideOutPosition;
    public float slideSpeed = 5f;

    [Header("Lever Settings")]
    public doorController connectedDoor;
    public SpriteRenderer leverSpriteRenderer;
    public Sprite activatedSprite;

    private bool playerNearby = false;
    private bool isActivated = false;
    private bool shouldSlideIn = false;
    private bool shouldSlideOut = false;

    void Start()
    {
        if (promptUI != null)
        {
            promptUI.transform.localPosition = slideOutPosition;
            promptUI.SetActive(false);
        }

        if (promptTextComponent != null)
            promptTextComponent.text = promptText;
    }

    void Update()
    {
        if (playerNearby && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            ActivateLever();
        }

        HandlePromptSlide();
    }

    void ActivateLever()
    {
        isActivated = true;
        StartSlideOut();

        if (leverSpriteRenderer != null && activatedSprite != null)
            leverSpriteRenderer.sprite = activatedSprite;

        if (connectedDoor != null)
            connectedDoor.OpenDoor();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            playerNearby = true;
            if (promptUI != null)
            {
                promptUI.SetActive(true);
                StartSlideIn();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            StartSlideOut();
        }
    }

    void StartSlideIn()
    {
        shouldSlideIn = true;
        shouldSlideOut = false;
    }

    void StartSlideOut()
    {
        shouldSlideOut = true;
        shouldSlideIn = false;
    }

    void HandlePromptSlide()
    {
        if (promptUI == null)
            return;

        if (shouldSlideIn)
        {
            promptUI.transform.localPosition = Vector3.MoveTowards(
                promptUI.transform.localPosition,
                slideInPosition,
                slideSpeed * Time.deltaTime
            );
        }
        else if (shouldSlideOut)
        {
            promptUI.transform.localPosition = Vector3.MoveTowards(
                promptUI.transform.localPosition,
                slideOutPosition,
                slideSpeed * Time.deltaTime
            );

            if (Vector3.Distance(promptUI.transform.localPosition, slideOutPosition) < 0.01f)
            {
                promptUI.SetActive(false);
                shouldSlideOut = false;
            }
        }
    }
}