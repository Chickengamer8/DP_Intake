using UnityEngine;
using System.Collections;

public class robotMovement : MonoBehaviour
{
    private Transform target;
    private GameObject platformToRotate;
    private GameObject colliderToEnable;

    public float speed = 2f;
    [SerializeField] private float platformRotateTime = 1f;

    private bool shouldMove = false;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        shouldMove = true;
    }

    public void SetPlatformToRotate(GameObject platform)
    {
        platformToRotate = platform;
    }

    public void SetColliderToEnable(GameObject colliderObj)
    {
        colliderToEnable = colliderObj;
    }

    private void Update()
    {
        if (!shouldMove || target == null) return;

        Vector3 targetPosition = target.position;
        targetPosition.z = 0f;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            shouldMove = false;

            if (platformToRotate != null)
            {
                StartCoroutine(RotatePlatform());
            }
        }
    }

    private IEnumerator RotatePlatform()
    {
        platformToRotate.SetActive(true);

        Quaternion startRot = platformToRotate.transform.rotation;
        Quaternion endRot = Quaternion.Euler(90f, 0f, -180f);

        float elapsed = 0f;
        while (elapsed < platformRotateTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / platformRotateTime);
            platformToRotate.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        if (colliderToEnable != null)
        {
            BoxCollider box = colliderToEnable.GetComponent<BoxCollider>();
            if (box != null) box.enabled = true;
        }
    }
}
