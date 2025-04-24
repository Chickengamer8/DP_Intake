using UnityEngine;

public class boxLanding : MonoBehaviour
{
    public LayerMask groundLayer;

    private void OnCollisionEnter(Collision collision)
    {
        // Controleer of het object waartegen we botsen tot de groundLayer behoort
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                Destroy(rb); // Verwijder de Rigidbody om hem 'stil' te zetten
                // Alternatief: rb.isKinematic = true; als je hem wilt behouden
            }
        }
    }
}
