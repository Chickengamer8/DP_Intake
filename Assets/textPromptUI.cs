using System.Collections;
using UnityEngine;
using TMPro;

public class textPromptUI : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Vector2 hiddenPosition;
    public Vector2 visiblePosition;
    public float slideDuration = 0.4f;
    public float timeBetweenLines = 2f;
    public cutsceneController controller;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = hiddenPosition;
    }

    public void ShowDialogue(string[] lines)
    {
        StartCoroutine(PlayDialogue(lines));
    }

    private IEnumerator PlayDialogue(string[] lines)
    {
        // Slide in
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = visiblePosition;

        // First line
        for (int i = 0; i < lines.Length; i++)
        {
            dialogueText.text = lines[i];
            yield return new WaitForSeconds(timeBetweenLines);
        }

        // Slide out
        elapsed = 0f;
        while (elapsed < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            // Slide-out zwarte balken
            controller.sidebarsControl(false);
            yield return null;
        }
        rectTransform.anchoredPosition = hiddenPosition;
    }
}
