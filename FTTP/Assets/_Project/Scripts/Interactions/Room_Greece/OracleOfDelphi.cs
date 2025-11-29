using UnityEngine;

using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class OracleOfDelphi : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float activationDistance = 3f;
    [SerializeField] private float answerDisplayDuration = 6f;
    [SerializeField] private float minRecordingTime = 1f;
    [SerializeField] private float thinkingTime = 2.5f;

    [Header("XR Input Actions")]
    [SerializeField] private InputActionReference gripAction;
    [SerializeField] private InputActionReference triggerAction;

    [Header("UI References")]
    [SerializeField] private GameObject hoverPrompt;
    [SerializeField] private TextMeshProUGUI hoverText;
    [SerializeField] private MysticTextPanel dialoguePanel;
    [SerializeField] private GameObject recordingIndicator;
    [SerializeField] private GameObject thinkingIndicator;

    [Header("Oracle Character")]
    [SerializeField] private Animator oracleAnimator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip answerSound;

    private Transform playerCamera;
    private bool isProcessing = false;
    private bool isRecording = false;
    private float recordStartTime;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    private GreeceRoomController roomController;

    // Thematic yes/no responses
    private string[] yesAnswers = new string[]
    {
        "The gods look favourably upon you in this matter.",
        "Yes, the Fates smile upon your path.",
        "The omens are clear... it shall be so.",
        "The divine powers align in your favor.",
        "I see fortune shining upon your endeavor."
    };

    private string[] noAnswers = new string[]
    {
        "It does not seem so.",
        "The gods advise caution... I sense misfortune.",
        "No, the stars warn against this path.",
        "The Fates have woven a different destiny.",
        "Alas, the portents are not in your favour."
    };

    private string[] uncertainAnswers = new string[]
    {
        "I'm afraid the stars are unclear on this.",
        "The future is shrouded in mist... I cannot say.",
        "The gods remain silent. The answer lies within you.",
        "The path diverges... both outcomes are possible.",
        "Ask again when the moon changes its face."
    };


    private void Awake()
    {
        roomController = FindObjectOfType<GreeceRoomController>();
        audioSource ??= GetComponent<AudioSource>();
        playerCamera = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>()?.Camera.transform;

        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>() ?? gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.selectEntered.AddListener(_ => StartInteraction());
        interactable.selectExited.AddListener(_ => EndInteraction());

        EnsureColliderSetup();
        SetUIActive(false);
    }

    private void Update()
    {
        if (!isProcessing) CheckPlayerProximity();
        if (isRecording) HandleRecording();
    }

    private void CheckPlayerProximity()
    {
        float distance = Vector3.Distance(transform.position, playerCamera.position);
        bool isClose = distance <= activationDistance;
        SetUIActive(isClose && !isProcessing);
    }

    private void HandleRecording()
    {
        float recordTime = Time.time - recordStartTime;
        if (!IsButtonPressed() && recordTime >= minRecordingTime) CompleteRecording();
    }

    private void StartInteraction()
    {
        if (isProcessing || isRecording) return;
        ShowDialogue("Hold the button and ask your question.");
    }

    private void EndInteraction()
    {
        if (isRecording) CancelRecording();
    }

    private void StartRecording()
    {
        isRecording = true;
        recordStartTime = Time.time;
        SetUIActive(false);
        recordingIndicator?.SetActive(true);
        ShowDialogue("Waiting...");
    }

    private void CompleteRecording()
    {
        isRecording = false;
        recordingIndicator?.SetActive(false);
        ShowDialogue("Thinking...");
        roomController?.OnOracleInteraction();
        StartCoroutine(OracleResponse());
    }

    private void CancelRecording()
    {
        isRecording = false;
        recordingIndicator?.SetActive(false);
        ShowDialogue("Recording cancelled. Hold the button to try again.");
    }

    private IEnumerator OracleResponse()
    {
        isProcessing = true;
        thinkingIndicator?.SetActive(true);
        oracleAnimator?.SetTrigger("Think");
        yield return new WaitForSeconds(thinkingTime);

        thinkingIndicator?.SetActive(false);
        string answer = GenerateAnswer();
        ShowDialogue(answer);
        oracleAnimator?.SetTrigger("Speak");
        audioSource?.PlayOneShot(answerSound);

        yield return new WaitForSeconds(answerDisplayDuration);
        SetUIActive(false);
        isProcessing = false;
    }

    private string GenerateAnswer()
    {
        int roll = Random.Range(0, 10);
        if (roll < 4) return yesAnswers[Random.Range(0, yesAnswers.Length)];
        if (roll < 7) return noAnswers[Random.Range(0, noAnswers.Length)];
        return uncertainAnswers[Random.Range(0, uncertainAnswers.Length)];
    }

    private bool IsButtonPressed()
    {
        bool isGripPressed = gripAction?.action.ReadValue<float>() > 0.5f;
        bool isTriggerPressed = triggerAction?.action.ReadValue<float>() > 0.5f;
        return isGripPressed || isTriggerPressed;
    }

    private void ShowDialogue(string message)
    {
        dialoguePanel?.gameObject.SetActive(true);
        dialoguePanel?.SetText(message);
    }

    private void SetUIActive(bool active)
    {
        hoverPrompt?.SetActive(active);
        if (active) hoverPrompt?.transform.LookAt(playerCamera);
    }

    private void EnsureColliderSetup()
    {
        var collider = GetComponent<Collider>() ?? gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
    }
}
