using System.Collections;
using UnityEngine;
using TMPro;

public class textPromptUI : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI skipHintText;

    public Vector2 hiddenPosition;
    public Vector2 visiblePosition;

    public Vector2 skipHintHiddenPosition;
    public Vector2 skipHintVisiblePosition;

    public float slideDuration = 0.4f;
    public float timeBetweenLines = 2f;

    public cutsceneController controller;

    private RectTransform rectTransform;
    private RectTransform skipHintRectTransform;

    private bool skipLineRequested = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = hiddenPosition;

        if (skipHintText != null)
        {
            skipHintRectTransform = skipHintText.GetComponent<RectTransform>();
            skipHintRectTransform.anchoredPosition = skipHintHiddenPosition;
        }
    }

    public void ShowDialogue(string[] lines)
    {
        StartCoroutine(PlayDialogue(lines));
    }

    private IEnumerator PlayDialogue(string[] lines)
    {
        // Slide in main dialogue
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, elapsed / slideDuration);
            if (skipHintText != null)
                skipHintRectTransform.anchoredPosition = Vector2.Lerp(skipHintHiddenPosition, skipHintVisiblePosition, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = visiblePosition;
        if (skipHintText != null)
        {
            skipHintRectTransform.anchoredPosition = skipHintVisiblePosition;
            skipHintText.text = "Press [Space] to skip line";
            skipHintText.gameObject.SetActive(true);
        }

        // Show each line
        for (int i = 0; i < lines.Length; i++)
        {
            dialogueText.text = lines[i];
            float timer = 0f;
            skipLineRequested = false;

            while (timer < timeBetweenLines && !skipLineRequested)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    skipLineRequested = true;

                timer += Time.deltaTime;
                yield return null;
            }
        }

        // Slide out
        elapsed = 0f;
        if (skipHintText != null)
            skipHintText.gameObject.SetActive(false);

        while (elapsed < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, elapsed / slideDuration);
            if (skipHintText != null)
                skipHintRectTransform.anchoredPosition = Vector2.Lerp(skipHintVisiblePosition, skipHintHiddenPosition, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            controller.sidebarsControl(false);
            yield return null;
        }

        rectTransform.anchoredPosition = hiddenPosition;
        if (skipHintText != null)
            skipHintRectTransform.anchoredPosition = skipHintHiddenPosition;
    }
}
