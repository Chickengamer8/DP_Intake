using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class robotSummon : MonoBehaviour
{
    public float moveSpeed = 5f;
    private List<Transform> enemies = new List<Transform>();
    private float killDelay = 1.5f;
    public GameObject platform;

    public void StartSequence(Transform robotWaypoint, Transform returnTo, float delayBetweenKills)
    {
        killDelay = delayBetweenKills;
        StartCoroutine(ApproachAndDestroy(robotWaypoint, returnTo));
    }

    private IEnumerator ApproachAndDestroy(Transform target, Transform returnTo)
    {
        yield return MoveTo(target.position);

        // Verzamel alle vijanden in de scene
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemyObjs)
        {
            if (enemy == null) continue;
            enemies.Add(enemy.transform);
        }

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            yield return MoveTo(enemy.position);

            // Kill enemy: zet rigidbody & colliders uit
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            Collider[] colliders = enemy.GetComponents<Collider>();

            if (rb != null) rb.isKinematic = true;
            foreach (Collider col in colliders)
            {
                if (col != null) col.enabled = false;
            }

            // (Later animatie of effect)
            Debug.Log("Enemy eliminated by robot: " + enemy.name);
            yield return new WaitForSeconds(killDelay);
        }

        // Ga terug naar oorspronkelijke locatie
        if (returnTo != null)
        {
            yield return MoveTo(returnTo.position);
        }

        platform.SetActive(true);
        Debug.Log("All enemies cleared.");
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.2f)
        {
            Vector3 direction = (destination - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
