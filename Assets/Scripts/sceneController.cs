using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class sceneController : MonoBehaviour
{
    [Header("Cutscene Elementen")]
    public List<Transform> cameraPositions;
    public List<string> textLines;
    public List<MonoBehaviour> animationScripts;

    [Header("Instellingen per stap")]
    public List<int> cameraJumpIndices;
    public List<float> autoNextDelay;

    [Header("UI Referenties")]
    public Camera mainCamera;
    public TMPro.TextMeshProUGUI dialogueText;
    public UnityEvent onCutsceneEnd;

    [Header("Cinematic Bars")]
    public RectTransform topBar;
    public RectTransform bottomBar;
    public float barSlideDuration = 0.5f;

    [Header("Cinematic Bar Posities")]
    public Vector2 topBarShowPos;
    public Vector2 topBarHidePos;
    public Vector2 bottomBarShowPos;
    public Vector2 bottomBarHidePos;

    [Header("External References")]
    public playerMovement playerController;
    public Rigidbody playerRB;
    public cameraFollow cameraFollowScript;
    public Transform cameraTarget;

    [Header("Fade Manager Reference")]
    public fadeManager fadeManagerInstance;

    private int currentLine = 0;
    private int currentCameraIndex = 0;
    private bool inCutscene = false;
    private bool canSkipLine = true;
    private bool triggered = false;
    private bool isSkipping = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        StartCutscene();
    }

    private void Start()
    {
        if (topBar != null) topBar.anchoredPosition = topBarHidePos;
        if (bottomBar != null) bottomBar.anchoredPosition = bottomBarHidePos;
    }

    public void StartCutscene()
    {
        playerRB.linearVelocity = Vector3.zero;
        inCutscene = true;
        currentLine = 0;
        currentCameraIndex = 0;
        dialogueText.gameObject.SetActive(true);
        Debug.Log("Cutscene started");

        if (playerController != null)
            playerController.canMove = false;

        if (cameraFollowScript != null)
            cameraFollowScript.target = null;

        TeleportCamera(0);
        ShowCurrentLine();
        StartCoroutine(SlideBarsIn());
    }

    private void Update()
    {
        if (!inCutscene) return;

        if (canSkipLine && Input.GetKeyDown(KeyCode.Space) &&
            (autoNextDelay.Count <= currentLine || autoNextDelay[currentLine] <= 0f))
        {
            ShowNextLine();
        }

        if (Input.GetKeyDown(KeyCode.G) && !isSkipping)
        {
            Debug.Log("[Cutscene] Skipping cutscene");
            StartCoroutine(SkipEntireCutscene());
        }
    }

    private void ShowCurrentLine()
    {
        if (currentLine >= textLines.Count)
        {
            EndCutscene();
            return;
        }

        dialogueText.text = textLines[currentLine];

        if (animationScripts.Count > currentLine && animationScripts[currentLine] != null)
        {
            animationScripts[currentLine].Invoke("startAnimation", 0f);
        }

        if (cameraJumpIndices.Contains(currentLine))
        {
            int nextCamIndex = cameraJumpIndices.IndexOf(currentLine) + 1;
            if (nextCamIndex < cameraPositions.Count)
                StartCoroutine(SwitchCameraPosition(nextCamIndex));
        }

        if (autoNextDelay.Count > currentLine && autoNextDelay[currentLine] > 0f)
        {
            Invoke(nameof(ShowNextLine), autoNextDelay[currentLine]);
        }
    }

    private void ShowNextLine()
    {
        currentLine++;
        ShowCurrentLine();
    }

    private IEnumerator SwitchCameraPosition(int camIndex)
    {
        canSkipLine = false;

        if (fadeManagerInstance != null)
            yield return fadeManagerInstance.FadeOut();

        TeleportCamera(camIndex);

        if (fadeManagerInstance != null)
            yield return fadeManagerInstance.FadeIn();

        canSkipLine = true;
    }

    private void TeleportCamera(int camIndex)
    {
        if (camIndex < cameraPositions.Count && mainCamera != null)
        {
            Vector3 pos = cameraPositions[camIndex].position;
            pos.z = -10f;
            mainCamera.transform.position = pos;
            currentCameraIndex = camIndex;
        }
    }

    private void EndCutscene()
    {
        inCutscene = false;
        dialogueText.text = "";

        if (playerController != null)
            playerController.canMove = true;

        if (cameraFollowScript != null && cameraTarget != null)
            cameraFollowScript.target = cameraTarget;

        onCutsceneEnd?.Invoke();
        StartCoroutine(SlideBarsOut());
    }

    private IEnumerator SkipEntireCutscene()
    {
        isSkipping = true;

        // Trigger alle animatie scripts direct
        for (int i = currentLine; i < textLines.Count; i++)
        {
            if (animationScripts.Count > i && animationScripts[i] != null)
            {
                animationScripts[i].Invoke("startAnimation", 0f);
            }
        }

        // Zet camera naar laatste positie (optioneel)
        if (cameraPositions.Count > 0)
        {
            TeleportCamera(cameraPositions.Count - 1);
        }

        // Even kort een frame wachten om te zorgen dat alles is uitgevoerd
        yield return null;

        EndCutscene();
    }

    private IEnumerator SlideBarsIn()
    {
        float t = 0f;
        while (t < barSlideDuration)
        {
            float lerp = t / barSlideDuration;
            if (topBar != null) topBar.anchoredPosition = Vector2.Lerp(topBarHidePos, topBarShowPos, lerp);
            if (bottomBar != null) bottomBar.anchoredPosition = Vector2.Lerp(bottomBarHidePos, bottomBarShowPos, lerp);
            t += Time.deltaTime;
            yield return null;
        }

        if (topBar != null) topBar.anchoredPosition = topBarShowPos;
        if (bottomBar != null) bottomBar.anchoredPosition = bottomBarShowPos;
    }

    private IEnumerator SlideBarsOut()
    {
        float t = 0f;
        while (t < barSlideDuration)
        {
            float lerp = t / barSlideDuration;
            if (topBar != null) topBar.anchoredPosition = Vector2.Lerp(topBarShowPos, topBarHidePos, lerp);
            if (bottomBar != null) bottomBar.anchoredPosition = Vector2.Lerp(bottomBarShowPos, bottomBarHidePos, lerp);
            t += Time.deltaTime;
            yield return null;
        }

        if (topBar != null) topBar.anchoredPosition = topBarHidePos;
        if (bottomBar != null) bottomBar.anchoredPosition = bottomBarHidePos;
    }
}
