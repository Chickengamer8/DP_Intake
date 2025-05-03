using UnityEngine;
using System.Collections;

public class fadeSpriteAnimation : MonoBehaviour
{
    [Header("Fade Settings")]
    [Range(0f, 1f)] public float fadedAlpha = 0.3f;
    public float fadeDuration = 0.5f;
    public float holdDuration = 5f; // Hoe lang blijven we op fadedAlpha voordat we teruggaan

    [Header("Target")]
    public SpriteRenderer targetSpriteRenderer; // 👉 sleep hier het target in!

    private float originalAlpha;
    private Coroutine fadeCoroutine;
    private bool animationStarted = false;

    void Start()
    {
        if (targetSpriteRenderer == null)
        {
            Debug.LogWarning("fadeSpriteAnimation: Geen targetSpriteRenderer ingesteld, script uitgeschakeld.");
            enabled = false;
            return;
        }

        originalAlpha = targetSpriteRenderer.color.a;
    }

    public void startAnimation()
    {
        if (animationStarted) return;
        animationStarted = true;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAndIn());
    }

    private IEnumerator FadeOutAndIn()
    {
        yield return StartCoroutine(FadeToAlpha(fadedAlpha));
        yield return new WaitForSeconds(holdDuration);
        yield return StartCoroutine(FadeToAlpha(originalAlpha));
    }

    private IEnumerator FadeToAlpha(float targetAlpha)
    {
        Color currentColor = targetSpriteRenderer.color;
        float startAlpha = currentColor.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            targetSpriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        targetSpriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }
}
