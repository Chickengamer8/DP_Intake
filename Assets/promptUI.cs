using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class promptUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public float slideDuration = 0.3f;
    public float visibleDuration = 2f;
    public Vector2 hiddenPosition;
    public Vector2 visiblePosition;

    private RectTransform rectTransform;
    private Coroutine currentRoutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = hiddenPosition;
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimatePrompt(message));
    }

    private IEnumerator AnimatePrompt(string message)
    {
        promptText.text = message;

        // Slide in
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = visiblePosition;

        // Wait
        yield return new WaitForSeconds(visibleDuration);

        // Slide out
        elapsed = 0f;
        while (elapsed < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = hiddenPosition;

        currentRoutine = null;
    }
}
