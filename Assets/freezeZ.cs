using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FreezeZ : MonoBehaviour
{
    private void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationZ;
        Debug.Log($"[FreezeZRotation] Z-rotatie bevroren op {gameObject.name}");
    }
}
