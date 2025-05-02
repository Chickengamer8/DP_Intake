using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target; // Default volgtarget, bijv. cameraTarget
    public Vector3 defaultOffset = new Vector3(0f, 0f, -10f);
    private Vector3 currentOffset;

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
        currentOffset = defaultOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }

    // 🔹 Call this to override camera for cutscene (with offset)
    public void SetCutsceneTargetWithOffset(Transform newTarget, float zoomLevel, Vector3 customOffset)
    {
        target = newTarget;
        targetZoom = zoomLevel;
        currentOffset = customOffset;
        overrideTarget = true;
    }

    // 🔹 Reset camera back to player
    public void ResetToDefault(Transform defaultTarget)
    {
        target = defaultTarget;
        targetZoom = defaultZoom;
        currentOffset = defaultOffset;
        overrideTarget = false;
    }

    // 🔹 Optional: change zoom only
    public void SetZoom(float newZoom)
    {
        targetZoom = newZoom;
    }
}