using UnityEngine;

public class ParallaxFollowY : MonoBehaviour
{
    public Transform cameraTransform; // Sleep hier je Main Camera in

    void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        // Alleen de Y aanpassen naar die van de camera
        transform.position = new Vector3(transform.position.x, cameraTransform.position.y, transform.position.z);
    }
}
