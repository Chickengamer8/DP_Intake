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
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        Color color = fadePanel.color;
        color.a = 1f;
        fadePanel.color = color;

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

    public IEnumerator FadeOut()
    {
        float t = 0f;
        Color color = fadePanel.color;
        color.a = 0f;
        fadePanel.color = color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = t / fadeDuration;
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;
    }

    public void StartSceneTransition(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        yield return FadeOut();
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
