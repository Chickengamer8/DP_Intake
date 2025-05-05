using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class cameraTravelAndTeleport : MonoBehaviour
{
    [Header("Camera movement")]
    public Transform pointA;
    public Transform pointB;
    public float cameraMoveSpeed = 5f;

    [Header("Apart object dat ook beweegt")]
    public GameObject movingObject;
    public float objectMoveSpeed = 2f;

    [Header("Fade & Visuals")]
    public float fadeDuration = 1f;
    public Image fadePanel;
    public Camera mainCamera;

    [Header("Optional Zoom")]
    public bool zoomOutEnabled = false;
    public float zoomOutTargetSize = 7f;
    public float zoomOutSpeed = 1f;

    [Header("Afbreekinstelling")]
    public float cameraAbortDistance = 10f;

    private bool animationStarted = false;

    public void startAnimation()
    {
        if (!animationStarted)
        {
            animationStarted = true;
            StartCoroutine(MoveAndFadeSequence());
        }
    }

    private IEnumerator MoveAndFadeSequence()
    {
        if (mainCamera == null || pointA == null || pointB == null)
        {
            Debug.LogWarning("cameraTravelAndTeleport: Referentie ontbreekt.");
            yield break;
        }

        mainCamera.transform.position = pointA.position;

        if (movingObject != null)
            movingObject.transform.position = pointA.position;

        bool cameraReached = false;
        bool objectReached = false;

        while (!cameraReached || !objectReached)
        {
            float distanceToB = Vector3.Distance(mainCamera.transform.position, pointB.position);
            if (distanceToB > cameraAbortDistance)
            {
                Debug.Log("cameraTravelAndTeleport: Beweging afgebroken, afstand tot pointB te groot.");

                if (movingObject != null)
                {
                    movingObject.SetActive(false);
                    Debug.Log("cameraTravelAndTeleport: MovingObject (watcher) is uitgeschakeld.");
                }

                break;
            }

            if (!cameraReached)
            {
                Vector3 camNextPos = Vector3.MoveTowards(mainCamera.transform.position, pointB.position, cameraMoveSpeed * Time.deltaTime);
                mainCamera.transform.position = camNextPos;

                if (Vector3.Distance(camNextPos, pointB.position) < 0.05f)
                    cameraReached = true;
            }

            if (movingObject != null && !objectReached)
            {
                Vector3 objNextPos = Vector3.MoveTowards(movingObject.transform.position, pointB.position, objectMoveSpeed * Time.deltaTime);
                movingObject.transform.position = objNextPos;

                if (Vector3.Distance(objNextPos, pointB.position) < 0.05f)
                    objectReached = true;
            }

            if (zoomOutEnabled && mainCamera != null && mainCamera.orthographic)
            {
                float currentSize = mainCamera.orthographicSize;
                mainCamera.orthographicSize = Mathf.MoveTowards(currentSize, zoomOutTargetSize, zoomOutSpeed * Time.deltaTime);
            }

            yield return null;
        }

        yield return StartCoroutine(Fade(0f, 1f));
        yield return StartCoroutine(Fade(1f, 0f));
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

    private void OnDrawGizmosSelected()
    {
        if (pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointB.position, cameraAbortDistance);
        }
    }
}
