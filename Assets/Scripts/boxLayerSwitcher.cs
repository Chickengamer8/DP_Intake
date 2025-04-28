using UnityEngine;

public class boxLayerSwitcher : MonoBehaviour
{
    public LayerMask solidLayerName;
    public LayerMask nonSolidLayerName;
    public LayerMask addColliderLayerName;

    private Rigidbody rb;
    private Collider col;

    private bool isNonSolid = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (isNonSolid)
            SetNonSolid();
        else
            SetSolid();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            // Tijdens grijpen mag hij grijpen ook als hij non-solid is
            if (isNonSolid)
                SetSolid();
        }
        else
        {
            if (isNonSolid)
                SetNonSolid();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == addColliderLayerName)
        {
            SetSolid();
        }
    }

    private void SetSolid()
    {
        gameObject.layer = solidLayerName;
        // Verander de layers van child colliders niet!
    }

    private void SetNonSolid()
    {
        gameObject.layer = nonSolidLayerName;
        // Verander de layers van child colliders niet!
    }

}
