using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class transparentTrigger3D : MonoBehaviour
{
    [Header("Fade Settings")]
    [Range(0f, 1f)] public float fadedAlpha = 0.3f;
    public float fadeDuration = 0.5f;

    private MeshRenderer meshRenderer;
    private Material materialInstance;
    private float originalAlpha;
    private Coroutine fadeCoroutine;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Belangrijk: we maken een instance zodat we niet het shared material beïnvloeden
        materialInstance = meshRenderer.material;

        if (materialInstance.HasProperty("_Color"))
        {
            originalAlpha = materialInstance.color.a;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} material has no _Color property.");
            enabled = false;
        }
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
        Color currentColor = materialInstance.color;
        float startAlpha = currentColor.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            materialInstance.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        materialInstance.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }
}
