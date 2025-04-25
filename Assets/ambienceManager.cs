using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceController : MonoBehaviour
{
    [Header("Audio Settings")]
    public List<AudioClip> ambienceClips;
    public AudioSource audioSource;
    public float checkInterval = 60f;
    public float chanceToPlay = 0.25f;
    public float fadeDuration = 2f;
    public float minVolume = 0f;
    public float maxVolume = 1f;

    private bool isPlaying = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(CheckForAmbience());
    }

    IEnumerator CheckForAmbience()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (!isPlaying && ambienceClips.Count > 0 && Random.value < chanceToPlay)
            {
                AudioClip clip = ambienceClips[Random.Range(0, ambienceClips.Count)];
                StartCoroutine(PlayWithFade(clip));
            }
        }
    }

    IEnumerator PlayWithFade(AudioClip clip)
    {
        isPlaying = true;

        audioSource.clip = clip;
        audioSource.volume = minVolume;
        audioSource.Play();

        // Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(minVolume, maxVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = maxVolume;

        // Wait until the clip is nearly finished
        yield return new WaitForSeconds(clip.length - fadeDuration);

        // Fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(maxVolume, minVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        isPlaying = false;
    }
}
