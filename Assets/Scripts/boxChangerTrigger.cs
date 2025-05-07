using UnityEngine;

public class boxChangerTrigger : MonoBehaviour
{
    [Header("Object to Enable")]
    public GameObject objectToEnable;

    [Header("Optional Sound")]
    public AudioSource audioSource; // Hier kun je optioneel een geluidje koppelen

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("insideBox"))
        {
            hasActivated = true;

            // ✅ Probeer eerst de box los te koppelen als hij wordt vastgehouden
            BoxGrabTrigger grabTrigger = other.GetComponentInParent<BoxGrabTrigger>();
            if (grabTrigger != null)
            {
                grabTrigger.ForceDetachBox();
            }

            // Disable the incoming box object
            other.gameObject.SetActive(false);

            // Enable the specified object
            if (objectToEnable != null)
                objectToEnable.SetActive(true);

            // Play sound if set
            if (audioSource != null)
                audioSource.Play();

            // Disable this trigger object
            gameObject.SetActive(false);
        }
    }
}
