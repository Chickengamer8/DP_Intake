using UnityEngine;

public class playerHideDetector : MonoBehaviour
{
    public bool isHidden { get; private set; } = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hide"))
        {
            isHidden = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hide"))
        {
            isHidden = false;
        }
    }
}
