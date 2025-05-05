using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ghostBoxTrigger : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    [Header("Objects to Enable")]
    public List<GameObject> objectsToEnable;

    [Header("Objects to Disable")]
    public List<GameObject> objectsToDisable;

    [Header("Platform Reference")]
    public movingPlatformManager platformScript;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Box"))
        {
            hasTriggered = true;
            StartCoroutine(HandleSequence());
        }
    }

    private IEnumerator HandleSequence()
    {
        yield return StartCoroutine(Fade(0f, 1f));  // fade out

        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (platformScript != null)
            platformScript.hasSurvivor = true;

        yield return StartCoroutine(Fade(1f, 0f));  // fade in

        // Disable this GameObject after everything is done
        gameObject.SetActive(false);
    }

    private IEnumerator Fade(float start, float end)
    {
        if (fadePanel == null) yield break;

        Color c = fadePanel.color;
        float t = 0f;

        fadePanel.gameObject.SetActive(true);

        while (t < fadeDuration)
        {
            float alpha = Mathf.Lerp(start, end, t / fadeDuration);
            fadePanel.color = new Color(c.r, c.g, c.b, alpha);
            t += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = new Color(c.r, c.g, c.b, end);

        if (end == 0f)
            fadePanel.gameObject.SetActive(false);
    }
}
