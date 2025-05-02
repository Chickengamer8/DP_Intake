using UnityEngine;

public class watcherPupilController : MonoBehaviour
{
    public Transform player;
    public Transform pupil;
    public float deadzoneRadius = 0.2f;  // max afstand dat de pupil mag bewegen

    private Vector3 initialPupilPosition;

    private void Start()
    {
        if (pupil != null)
            initialPupilPosition = pupil.localPosition;
    }

    private void Update()
    {
        if (player == null || pupil == null) return;

        Vector3 direction = player.position - transform.position;
        direction.z = 0f;  // alleen X/Y volgen

        if (direction != Vector3.zero)
        {
            Vector3 clampedOffset = Vector3.ClampMagnitude(direction.normalized * deadzoneRadius, deadzoneRadius);
            pupil.localPosition = initialPupilPosition + clampedOffset;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pupil != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + initialPupilPosition, deadzoneRadius);
        }
    }
}
