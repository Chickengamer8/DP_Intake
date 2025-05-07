using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ghostBoxEnable : MonoBehaviour
{
    public GameObject ghostBox;

    public void startAnimation()
    {
        ghostBox.SetActive(true);
    }
}
