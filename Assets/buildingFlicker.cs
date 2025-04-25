using UnityEngine;

public class buildingFlicker : MonoBehaviour
{
    public Material flickerMaterial; // Sleep hier je buildingTexture materiaal in
    public Color colorA = new Color(1f, 0.6f, 0.2f); // Goud-achtig
    public Color colorB = new Color(1f, 0.8f, 0.4f); // Lichter kaarslicht

    public float minDuration = 0.1f;
    public float maxDuration = 0.5f;

    private float timer;
    private float targetBlend;
    private float currentBlend;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        if (flickerMaterial == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PickNewTarget();
        }

        currentBlend = Mathf.Lerp(currentBlend, targetBlend, Time.deltaTime * 5f);
        Color flickerColor = Color.Lerp(colorA, colorB, currentBlend);

        flickerMaterial.color = flickerColor; // Of: flickerMaterial.SetColor("_Color", flickerColor);
    }

    void PickNewTarget()
    {
        timer = Random.Range(minDuration, maxDuration);
        targetBlend = Random.Range(0f, 1f);
    }
}
