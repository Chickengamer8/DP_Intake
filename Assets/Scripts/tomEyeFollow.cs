using UnityEngine;

public class tomEyeFollow : MonoBehaviour
{
    [Header("Pupil instellingen")]
    public Transform pupil;               // De pupil sprite
    public float followSpeed = 5f;        // Hoe snel de pupil volgt
    public Vector2 maxOffset = new Vector2(0.3f, 0.15f); // Beweging max vanaf oogcentrum (x = links/rechts, y = omhoog/omlaag)

    [Header("Target (meestal speler)")]
    public Transform target;

    private Vector3 defaultLocalPos;

    void Start()
    {
        if (pupil != null)
            defaultLocalPos = pupil.localPosition;
    }

    void Update()
    {
        if (pupil == null || target == null) return;

        // Richting naar target in wereld
        Vector3 dir = target.position - transform.position;

        // Breng naar lokale ruimte van het oog
        Vector3 localDir = transform.InverseTransformDirection(dir.normalized);

        // Clamp richting binnen offset limieten
        Vector3 clamped = new Vector3(
            Mathf.Clamp(localDir.x, -maxOffset.x, maxOffset.x),
            Mathf.Clamp(localDir.y, -maxOffset.y, maxOffset.y),
            0f
        );

        // Smooth bewegen naar nieuwe positie
        Vector3 targetPos = defaultLocalPos + clamped;
        pupil.localPosition = Vector3.Lerp(pupil.localPosition, targetPos, Time.deltaTime * followSpeed);
    }

    void OnDrawGizmosSelected()
    {
        if (pupil == null) return;

        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + transform.rotation * defaultLocalPos;
        Vector3 size = new Vector3(maxOffset.x * 2f, maxOffset.y * 2f, 0.01f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(defaultLocalPos, size);
    }
}
