//using UnityEngine;

//public class textPromptTrigger : MonoBehaviour
//{
//    public textPromptUI promptUI;
//    [TextArea(3, 10)] public string[] dialogueLines;
//    private bool triggered = false;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (!triggered && other.CompareTag("Player"))
//        {
//            triggered = true;
//            promptUI.ShowDialogue(dialogueLines);
//        }
//    }
//}
