using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timedEventTrigger : MonoBehaviour
{
    [Header("Plate Settings")]
    public Transform plateVisual;
    public float pressDepth = 0.1f;
    public float returnSpeed = 1f;

    [Header("Cooldown")]
    public float triggerCooldown = 5f;

    [Header("Timed Wall Settings")]
    public List<timedWall> targetWalls;
    public float delayBetweenSignals = 0.3f;

    [Header("Alarms")]
    public GameObject[] alarms;

    private Vector3 originalPosition;
    private bool isActivated = false;
    private Collider plateCollider;

    void Start()
    {
        if (plateVisual != null)
            originalPosition = plateVisual.localPosition;

        plateCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            // Deactiveer tijdelijk collider zodat speler niet opnieuw kan triggeren
            if (plateCollider != null)
                plateCollider.enabled = false;

            // Druk visueel in
            if (plateVisual != null)
                plateVisual.localPosition = originalPosition - new Vector3(0, pressDepth, 0);

            // Activeer alarms
            foreach (GameObject alarm in alarms)
                alarm.SetActive(true);

            // Start sequence
            StartCoroutine(TriggerSequence());
        }
    }

    IEnumerator TriggerSequence()
    {
        // Laat muren naar beneden zakken met delay
        foreach (timedWall wall in targetWalls)
        {
            if (wall != null)
                wall.SlideDown();

            yield return new WaitForSeconds(delayBetweenSignals);
        }

        // Wacht de volledige tijd voor de trap active blijft
        yield return new WaitForSeconds(3f);

        // Zet alarms uit
        foreach (GameObject alarm in alarms)
            alarm.SetActive(false);

        // Laat plate langzaam omhoog komen
        float t = 0;
        Vector3 pressedPos = plateVisual.localPosition;
        while (t < 1f)
        {
            t += Time.deltaTime * returnSpeed;
            plateVisual.localPosition = Vector3.Lerp(pressedPos, originalPosition, t);
            yield return null;
        }

        // Laat muren weer omhoogkomen, ook weer met delay
        foreach (timedWall wall in targetWalls)
        {
            if (wall != null)
                wall.SlideUp();

            yield return new WaitForSeconds(delayBetweenSignals);
        }

        // Cooldown voordat plaat weer actief wordt
        yield return new WaitForSeconds(triggerCooldown);
        if (plateCollider != null)
            plateCollider.enabled = true;

        isActivated = false;
    }
}
