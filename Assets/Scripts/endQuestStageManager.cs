using UnityEngine;
using System.Collections.Generic;

public class endQuestStageManager : MonoBehaviour
{
    [Header("Objects to Enable")]
    public List<GameObject> objectsToEnable;

    [Header("Objects to Disable")]
    public List<GameObject> objectsToDisable;

    [Header("SpriteRenderers to Disable")]
    public List<SpriteRenderer> spriteRenderersToDisable;

    [Header("Platform Manager")]
    public movingPlatformManager platformManager;

    [Header("Lever Sequence")]
    public leverSequence leverSequenceScript;

    public void startAnimation()
    {
        if (platformManager != null)
            platformManager.hasSurvivor = false;

        if (leverSequenceScript != null)
            leverSequenceScript.puzzleSolved = true;

        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (SpriteRenderer sr in spriteRenderersToDisable)
        {
            if (sr != null)
                sr.enabled = false;
        }
    }
}
