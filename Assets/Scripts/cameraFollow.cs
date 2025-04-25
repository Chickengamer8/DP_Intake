using UnityEngine;
using System.Collections;

public class cameraFollow : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target; // Default = CameraTarget (meebewegend met speler)
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float followSpeed = 5f;

    [Header("Zoom")]
    public float defaultZoom = 5f;
    private float targetZoom;
    public float zoomSpeed = 3f;

    private Camera cam;
    private bool overrideTarget = false;

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = defaultZoom;
        cam.orthographicSize = defaultZoom;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Smooth zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }

    // 🔹 Call this to override the target (e.g. Automa Tom)
    public void SetCutsceneTarget(Transform newTarget, float zoomLevel)
    {
        target = newTarget;
        targetZoom = zoomLevel;
        overrideTarget = true;
    }

    // 🔹 Call this to return to player follow
    public void ResetToDefault(Transform defaultTarget)
    {
        target = defaultTarget;
        targetZoom = defaultZoom;
        overrideTarget = false;
    }

    // 🔹 Optional: for smooth zoom only
    public void SetZoom(float newZoom)
    {
        targetZoom = newZoom;
    }
}