using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class robotSummon : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float killDelay = 1f;
    public float stayDuration = 3f;
    public float scaleUpSpeed = 1f;

    private Transform targetPoint;
    private Transform returnPoint;
    public GameObject helpPlatform;

    private Vector3 originalPlatformScale;

    [Header("Assigned Enemy List")]
    public List<enemyChase> enemyList = new List<enemyChase>();  // ← handmatig vijanden invullen in Inspector

    public void StartSequence(Transform waypoint, Transform returnPos, float killDelay)
    {
        targetPoint = waypoint;
        returnPoint = returnPos;
        this.killDelay = killDelay;
        StartCoroutine(MoveToWaypoint());
    }

    private IEnumerator MoveToWaypoint()
    {
        while (Vector3.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return StartCoroutine(KillEnemies());
    }

    private IEnumerator KillEnemies()
    {
        foreach (var enemy in enemyList)
        {
            if (enemy != null)
            {
                Animator enemyAnimator = enemy.GetComponentInChildren<Animator>();
                if (enemyAnimator != null)
                {
                    enemyAnimator.SetBool("isDead", true);
                }

                enemy.DisableEnemy();
                Debug.Log($"[robotSummon] Enemy {enemy.name} killed via assigned list.");
            }
        }

        yield return new WaitForSeconds(stayDuration);

        yield return StartCoroutine(ReturnToStart());
    }

    private IEnumerator ReturnToStart()
    {
        while (Vector3.Distance(transform.position, returnPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, returnPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        if (helpPlatform != null)
        {
            helpPlatform.SetActive(true);
            originalPlatformScale = helpPlatform.transform.localScale;
            helpPlatform.transform.localScale = Vector3.zero;

            Debug.Log("[robotSummon] Help platform activated, starting scale-up.");

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * scaleUpSpeed;
                helpPlatform.transform.localScale = Vector3.Lerp(Vector3.zero, originalPlatformScale, t);
                yield return null;
            }

            helpPlatform.transform.localScale = originalPlatformScale;

            Debug.Log("[robotSummon] Help platform scaled up to full size.");
        }

        Debug.Log("[robotSummon] Returned to start position.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
