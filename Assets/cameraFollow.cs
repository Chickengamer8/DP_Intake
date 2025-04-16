using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform target;              // Meestal de CameraTarget
    public Vector3 offset = new Vector3(0f, 0f, -10f);  // Standaard camera offset

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
}
