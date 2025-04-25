using UnityEngine;

public class parallaxLayer : MonoBehaviour
{
    public float parallaxFactor = 0.5f; // 0 = stil, 1 = beweegt mee als camera
    private Transform cam;
    private Vector3 previousCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        previousCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - previousCamPos;
        Vector3 movement = new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0f);
        transform.position += movement;
        previousCamPos = cam.position;
    }
}
