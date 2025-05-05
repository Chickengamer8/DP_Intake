using UnityEngine;

public class switchBossWatcher : MonoBehaviour
{
    public GameObject watcher;
    public GameObject bossWatcher;
    public void startAnimation()
    {
        watcher.SetActive(false);
        bossWatcher.SetActive(true);
    }
}
