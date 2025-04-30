//using UnityEngine;
//using System.Collections;

//public class slideInOffset : MonoBehaviour
//{
//    [Header("Slide-in Instellingen")]
//    public GameObject objectToSlide;
//    public Vector3 offset = new Vector3(-500f, 0f, 0f); // Vanaf links bijv.
//    public float slideDuration = 1f;

//    private bool hasStarted = false;

//    public void startAnimation()
//    {
//        if (hasStarted) return;
//        hasStarted = true;

//        if (objectToSlide == null)
//        {
//            Debug.LogWarning("slideInOffset: Geen object ingesteld.");
//            return;
//        }

//        objectToSlide.SetActive(true);
//        RectTransform rt = objectToSlide.GetComponent<RectTransform>();

//        if (rt != null)
//        {
//            // UI-element (RectTransform)
//            rt.anchoredPosition = offset;
//            StartCoroutine(SlideUI(rt));
//        }
//        else
//        {
//            // 3D object
//            objectToSlide.transform.localPosition = offset;
//            StartCoroutine(SlideObject(objectToSlide.transform));
//        }
//    }

//    private IEnumerator SlideUI(RectTransform rt)
//    {
//        float t = 0f;
//        Vector3 start = offset;
//        Vector3 end = Vector3.zero;

//        while (t < slideDuration)
//        {
//            float lerp = t / slideDuration;
//            rt.anchoredPosition = Vector3.Lerp(start, end, lerp);
//            t += Time.deltaTime;
//            yield return null;
//        }

//        rt.anchoredPosition = end;
//    }

//    private IEnumerator SlideObject(Transform tf)
//    {
//        float t = 0f;
//        Vector3 start = offset;
//        Vector3 end = Vector3.zero;

//        while (t < slideDuration)
//        {
//            float lerp = t / slideDuration;
//            tf.localPosition = Vector3.Lerp(start, end, lerp);
//            t += Time.deltaTime;
//            yield return null;
//        }

//        tf.localPosition = end;
//    }
//}
