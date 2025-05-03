using UnityEngine;

public class flipSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = transform.parent.GetComponent<Rigidbody>();

        if (spriteRenderer == null)
            Debug.LogError("playerFlip: SpriteRenderer niet gevonden op dit object!");

        if (rb == null)
            Debug.LogError("playerFlip: Rigidbody2D niet gevonden op dit object!");
    }

    void Update()
    {
        if (rb == null || spriteRenderer == null)
            return;

        // Flip afhankelijk van snelheid op X-as
        if (rb.linearVelocity.x > 0.1f)
        {
            spriteRenderer.flipX = false; // Rechts kijken
        }
        else if (rb.linearVelocity.x < -0.1f)
        {
            spriteRenderer.flipX = true; // Links kijken
        }
    }
}
