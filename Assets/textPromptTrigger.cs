using UnityEngine;
using System.Collections;

public class textPromptTrigger : MonoBehaviour
{
    [Header("Prompt Instellingen")]
    public RectTransform promptContainer;           // Het hele blok (image + tekst)
    public Vector2 hiddenPosition = new Vector2(0, -200);
    public Vector2 shownPosition = new Vector2(0, 0);
    public float slideDuration = 0.3f;

    private Coroutine currentSlide;

    private void Start()
    {
        if (promptContainer != null)
            promptContainer.anchoredPosition = hiddenPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartSlide(shownPosition);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartSlide(hiddenPosition);
    }

    private void StartSlide(Vector2 targetPos)
    {
        if (currentSlide != null)
            StopCoroutine(currentSlide);

        currentSlide = StartCoroutine(SlidePrompt(targetPos));
    }

    private IEnumerator SlidePrompt(Vector2 targetPos)
    {
        Vector2 startPos = promptContainer.anchoredPosition;
        float t = 0f;

        while (t < slideDuration)
        {
            promptContainer.anchoredPosition = Vector2.Lerp(startPos, targetPos, t / slideDuration);
            t += Time.deltaTime;
            yield return null;
        }

        promptContainer.anchoredPosition = targetPos;
    }
}
