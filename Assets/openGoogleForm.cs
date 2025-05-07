using UnityEngine;

public class openGoogleForm : MonoBehaviour
{
    // Zet hier je Google Form URL
    public string formURL = "https://forms.gle/JEEXAMPLEDRAFMlink";

    public void OpenForm()
    {
        Application.OpenURL(formURL);
    }
}