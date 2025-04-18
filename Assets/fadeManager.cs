using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class fadeManager : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1f;

    void Start()
    {
        // Start fade-in als de scene begint
        if (fadePanel != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void StartSceneTransition(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        Color color = fadePanel.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = 1f - (t / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 0f;
        fadePanel.color = color;
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        float t = 0f;
        Color color = fadePanel.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = t / fadeDuration;
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;

        SceneManager.LoadScene(sceneName);
    }

    public void StartSceneTransition(string sceneName, float duration)
    {
        StartCoroutine(FadeOutAndLoad(sceneName, duration));
    }

    IEnumerator FadeOutAndLoad(string sceneName, float duration)
    {
        float t = 0f;
        Color color = fadePanel.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = t / duration;
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;

        SceneManager.LoadScene(sceneName);
    }
}
