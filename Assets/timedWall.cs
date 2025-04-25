using UnityEngine;

public class timedWall : MonoBehaviour
{
    public Transform endPoint;
    public float slideSpeed = 2f;

    private Vector3 startPoint;
    private bool slidingDown = false;
    private bool slidingUp = false;

    void Start()
    {
        startPoint = transform.position;
    }

    void Update()
    {
        if (slidingDown)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, slideSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, endPoint.position) < 0.01f)
            {
                transform.position = endPoint.position;
                slidingDown = false;
            }
        }

        if (slidingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint, slideSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPoint) < 0.01f)
            {
                transform.position = startPoint;
                slidingUp = false;
            }
        }
    }

    public void SlideDown()
    {
        slidingDown = true;
        slidingUp = false;
    }

    public void SlideUp()
    {
        slidingUp = true;
        slidingDown = false;
    }
}
