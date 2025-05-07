using UnityEngine;
using System.Collections;

public class uiPromptUniversal : MonoBehaviour
{
    [Header("UI Element to Slide")]
    public RectTransform uiElement;

    [Header("Slide Settings")]
    public Vector3 showPosition;
    public Vector3 hidePosition;
    public float slideSpeed = 5f;

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }
}
