using UnityEngine;

public class textureGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float spacing = 1f;

    void Start()
    {
        for (float x = startPoint.x; x < endPoint.x; x += spacing)
        {
            for (float y = startPoint.y; y < endPoint.y; y += spacing)
            {
                Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
            }
        }
    }
}
