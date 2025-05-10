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

    [Header("Mouse Influence")]
    public float mouseSensitivity = 0.1f;       // Hoeveel invloed muisbeweging heeft
    public float maxMouseOffset = 1f;           // Max offset in units
    public float mouseReturnSpeed = 2f;         // Hoe snel offset terug naar 0 gaat

    private Vector3 mouseOffset = Vector3.zero; // Dynamische offset gebaseerd op muis

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

        // ✅ Lees muisinput
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ✅ Pas de mouseOffset aan
        Vector3 mouseMovement = new Vector3(mouseX, mouseY, 0f) * mouseSensitivity;
        mouseOffset += mouseMovement;

        // ✅ Beperk tot max offset
        mouseOffset.x = Mathf.Clamp(mouseOffset.x, -maxMouseOffset, maxMouseOffset);
        mouseOffset.y = Mathf.Clamp(mouseOffset.y, -maxMouseOffset, maxMouseOffset);

        // ✅ Smooth terug naar 0 als je stopt met bewegen
        mouseOffset = Vector3.Lerp(mouseOffset, Vector3.zero, mouseReturnSpeed * Time.deltaTime);

        // ✅ Bereken positie mét mouse offset
        Vector3 desiredPosition = target.position + currentOffset + mouseOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // ✅ Zoom bijwerken
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