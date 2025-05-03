using UnityEngine;
using System.Collections;

public class showBatteries : MonoBehaviour
{
    public GameObject[] objectsToActivate;
    public float delayBetween = 1f;

    private bool activated = false;

    public void startAnimation()
    {
        if (!activated)
        {
            activated = true;
            StartCoroutine(ActivateSequence());
        }
    }

    private IEnumerator ActivateSequence()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null) obj.SetActive(true);
            yield return new WaitForSeconds(delayBetween);
        }
    }
}
