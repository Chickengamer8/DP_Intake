using UnityEngine;

public class glowPulse : MonoBehaviour
{
    public Renderer targetRenderer;
    public Color glowColor = Color.cyan;
    public float pulseSpeed = 2f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;

    private Material materialInstance;
    private float pulseTimer;

    void Start()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        materialInstance = targetRenderer.material;
        materialInstance.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        pulseTimer += Time.deltaTime * pulseSpeed;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(pulseTimer) + 1f) / 2f);
        Color finalColor = glowColor * intensity;

        materialInstance.SetColor("_EmissionColor", finalColor);
    }
}
