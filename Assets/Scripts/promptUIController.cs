using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class promptUIController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform uiElement;
    public TMP_Text promptText;
    public Image promptImage;

    [Header("Slide Positions")]
    public Vector3 hidePosition;
    public Vector3 showPosition;
    public float slideSpeed = 5f;

    private Coroutine slideCoroutine;

    private void Start()
    {
        if (uiElement != null)
            uiElement.anchoredPosition = hidePosition;
    }

    public void ShowPrompt(string message, Sprite image)
    {
        if (promptText != null)
            promptText.text = message;

        if (promptImage != null)
            promptImage.sprite = image;

        StartSlide(showPosition);
    }

    public void HidePrompt()
    {
        StartSlide(hidePosition);
    }

    private void StartSlide(Vector3 targetPosition)
    {
        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SlideToPosition(targetPosition));
    }

    private IEnumerator SlideToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(uiElement.anchoredPosition, targetPosition) > 0.01f)
        {
            uiElement.anchoredPosition = Vector3.Lerp(uiElement.anchoredPosition, targetPosition, Time.deltaTime * slideSpeed);
            yield return null;
        }

        uiElement.anchoredPosition = targetPosition;
    }
}