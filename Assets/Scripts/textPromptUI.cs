using System.Collections;
using UnityEngine;
using TMPro;

public class textPromptUI : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI skipHintText;

    public Vector2 hiddenPosition;
    public Vector2 visiblePosition;

    public float slideDuration = 0.4f;
    public float timeBetweenLines = 2f;

    public cutsceneController controller;

    private RectTransform rectTransform;
    private bool skipLineRequested = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = hiddenPosition;

        if (skipHintText != null)
        {
            skipHintText.text = "Press [Space] to skip line";
            skipHintText.gameObject.SetActive(false); // tekst mag wel blijven staan als child
        }
    }

    public void ShowDialogue(string[] lines)
    {
        StartCoroutine(PlayDialogue(lines));
    }

    private IEnumerator PlayDialogue(string[] lines)
    {
        // Slide in de zwarte balk (dialogue paneel)
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = visiblePosition;

        // Toon elke zin
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

        // Slide out de zwarte balk
        elapsed = 0f;
        while (elapsed < slideDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, elapsed / slideDuration);
            elapsed += Time.deltaTime;
            controller.sidebarsControl(false);
            yield return null;
        }

        rectTransform.anchoredPosition = hiddenPosition;
    }
}
