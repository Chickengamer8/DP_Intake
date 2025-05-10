using UnityEngine;

public class flipSprite : MonoBehaviour
{
    private Rigidbody rb;
    private bool facingRight = true;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("flipSprite: Rigidbody niet gevonden op het parent object!");
    }

    void Update()
    {
        if (rb == null)
            return;

        float moveX = rb.linearVelocity.x;

        if (moveX > 0.1f && !facingRight)
        {
            Flip();
        }
        else if (moveX < -0.1f && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;

        facingRight = !facingRight;
    }
}
