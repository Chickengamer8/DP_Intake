using UnityEngine;

public class doorController : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;
    private bool shouldOpen = false;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Start()
    {
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(transform.eulerAngles + Vector3.forward * openAngle);
    }

    void Update()
    {
        if (shouldOpen)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    public void OpenDoor()
    {
        shouldOpen = true;
    }
}
