using UnityEngine;
using System.Collections;

public class uiPromptUniversal : MonoBehaviour
{
    [Header("UI Element to Slide")]
    public RectTransform uiElement;

    [Header("Slide Settings")]
    public Vector3 showPosition;
    public Vector3 hidePosition;
    public float slideSpeed = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(SlideToPosition(showPosition));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(SlideToPosition(hidePosition));
        }
    }

    private IEnumerator SlideToPosition(Vector3 targetPos)
    {
        while (Vector3.Distance(uiElement.anchoredPosition, targetPos) > 0.01f)
        {
            uiElement.anchoredPosition = Vector3.Lerp(uiElement.anchoredPosition, targetPos, Time.deltaTime * slideSpeed);
            yield return null;
        }

        uiElement.anchoredPosition = targetPos;
    }
}
