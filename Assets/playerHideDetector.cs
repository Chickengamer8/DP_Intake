using UnityEngine;

public class playerHideDetector : MonoBehaviour
{
    private int hideZoneCount = 0;
    public bool isHidden => hideZoneCount > 0;
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
        Debug.Log(hideZoneCount);
        if (hideZoneCount <= 0) vision.canSeePlayer = true;
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
            hideZoneCount = Mathf.Max(0, hideZoneCount - 1);
        }
    }
}
