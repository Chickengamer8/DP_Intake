using UnityEngine;

public class cameraTargetFollow : MonoBehaviour
{
    public Transform player;              // De speler die gevolgd wordt
    public float followSpeed = 5f;        // Hoe snel het target de speler volgt

    void Update()
    {
        if (player == null) return;

        // Smoothly beweeg richting de speler positie
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(player.position.x, player.position.y, transform.position.z),
            Time.deltaTime * followSpeed
        );
    }
}
