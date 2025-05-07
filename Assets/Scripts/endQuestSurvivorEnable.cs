using UnityEngine;

public class endQuestSurvivorEnable : MonoBehaviour
{
    public GameObject firstSurvivor;

    public void startAnimation()
    {
        firstSurvivor.SetActive(true);
    }
}
