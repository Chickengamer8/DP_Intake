using UnityEngine;

public class robotState : MonoBehaviour
{
    public static robotState Instance;
    public int insertedBatteries = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InsertBattery()
    {
        insertedBatteries++;
        Debug.Log("Batterij toegevoegd! Totaal: " + insertedBatteries);
    }
}