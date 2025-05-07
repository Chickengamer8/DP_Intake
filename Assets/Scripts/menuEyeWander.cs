using UnityEngine;

public class menuEyeWander : MonoBehaviour
{
    [Header("Wander Settings")]
    public float wanderRadius = 0.1f;  // hoe ver de pupil mag bewegen (local space)
    public float wanderSpeed = 1f;     // snelheid van bewegen
    public float pauseTime = 1f;       // tijd tussen richtingswissels

    private Vector3 startLocalPos;
    private Vector3 targetLocalPos;
    private float timer = 0f;

    void Start()
    {
        startLocalPos = transform.localPosition;
        PickNewTarget();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // smooth bewegen naar het target
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * wanderSpeed);

        if (timer >= pauseTime)
        {
            PickNewTarget();
            timer = 0f;
        }
    }

    void PickNewTarget()
    {
        // random punt binnen een cirkel
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        targetLocalPos = startLocalPos + new Vector3(randomPoint.x, randomPoint.y, 0f);
    }
}
