using UnityEngine;

public class playerHideDetector : MonoBehaviour
{
    private int hideZoneCount = 0;
    public bool isHidden => hideZoneCount > 1;
    public eyeVision vision;

    void Start()
    {
        // Initial overlap check bij spawn
        Collider[] overlapping = Physics.OverlapBox(
            transform.position,
            GetComponent<Collider>().bounds.extents,
            Quaternion.identity,
            LayerMask.GetMask("Hide")
        );

        hideZoneCount = overlapping.Length;
    }

    private void Update()
    {
        Debug.Log("Hide layers:");
        Debug.Log(hideZoneCount);
        if (!isHidden) vision.canSeePlayer = true;
        else vision.canSeePlayer = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hide"))
        {
            hideZoneCount++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hide"))
        {
            hideZoneCount--;
        }
    }
}
