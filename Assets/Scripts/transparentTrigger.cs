using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class transparentTrigger : MonoBehaviour
{
    [Header("Fade Settings")]
    [Range(0f, 1f)] public float fadedAlpha = 0.3f;
    public float fadeDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private float originalAlpha;
    private Coroutine fadeCoroutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalAlpha = spriteRenderer.color.a;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(fadedAlpha);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(originalAlpha);
        }
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToAlpha(targetAlpha));
    }

    private System.Collections.IEnumerator FadeToAlpha(float targetAlpha)
    {
        Color currentColor = spriteRenderer.color;
        float startAlpha = currentColor.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }
}
