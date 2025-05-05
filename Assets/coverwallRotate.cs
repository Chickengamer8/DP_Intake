using UnityEngine;

public class coverwallRotate : MonoBehaviour
{
    [Header("Player Reference")]
    public playerMovement playerScript;
    public Rigidbody playerRb;

    [Header("Rotation Settings")]
    public Transform rotatingChild;
    public float rotationSpeed = 50f;

    [Header("Camera Zoom Settings")]
    public Camera mainCamera;
    public cameraFollow cameraFollowScript;  // optioneel
    public float zoomOutSize = 7f;
    public float zoomSpeed = 2f;

    private float originalZoomSize;
    private bool playerInTrigger = false;
    private bool isHoldingRMB = false;

    private void Start()
    {
        if (cameraFollowScript != null)
        {
            originalZoomSize = cameraFollowScript.defaultZoom;
        }
        else if (mainCamera != null)
        {
            originalZoomSize = mainCamera.orthographicSize;
        }
    }

    private void Update()
    {
        if (!playerInTrigger) return;

        if (Input.GetMouseButtonDown(1))
        {
            isHoldingRMB = true;
            if (playerScript != null)
                playerScript.canMove = false;

            if (playerRb != null)
                playerRb.linearVelocity = Vector3.zero;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isHoldingRMB = false;
            if (playerScript != null)
                playerScript.canMove = true;
        }

        if (isHoldingRMB && rotatingChild != null)
        {
            bool pressA = Input.GetKey(KeyCode.A);
            bool pressD = Input.GetKey(KeyCode.D);

            if (pressA && !pressD)
            {
                rotatingChild.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            }
            else if (pressD && !pressA)
            {
                rotatingChild.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
            }
        }

        HandleCameraZoom();
    }

    private void HandleCameraZoom()
    {
        float targetZoom = isHoldingRMB ? zoomOutSize : originalZoomSize;

        if (cameraFollowScript != null)
        {
            cameraFollowScript.SetZoom(targetZoom);
        }
        else if (mainCamera != null)
        {
            mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;

            if (isHoldingRMB && playerScript != null)
            {
                isHoldingRMB = false;
                playerScript.canMove = true;
            }
        }
    }
}
