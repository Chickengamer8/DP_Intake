using UnityEngine;
using System.Collections;

public class objectSlide : MonoBehaviour
{
    [Header("Te sliden object")]
    public GameObject objectToSlide;

    [Header("Slide van A naar B")]
    public Transform pointA;
    public Transform pointB;
    public float slideDuration = 1f;

    private bool hasStarted = false;

    public void startAnimation()
    {
        if (hasStarted) return;
        hasStarted = true;

        if (objectToSlide == null || pointA == null || pointB == null)
        {
            Debug.LogWarning("objectSlide: Vereiste referenties ontbreken.");
            return;
        }

        objectToSlide.SetActive(true);
        objectToSlide.transform.position = pointA.position;
        StartCoroutine(SlideToPosition());
    }

    private IEnumerator SlideToPosition()
    {
        float t = 0f;
        Vector3 start = pointA.position;
        Vector3 end = pointB.position;

        while (t < slideDuration)
        {
            objectToSlide.transform.position = Vector3.Lerp(start, end, t / slideDuration);
            t += Time.deltaTime;
            yield return null;
        }

        objectToSlide.transform.position = end;
    }
}
