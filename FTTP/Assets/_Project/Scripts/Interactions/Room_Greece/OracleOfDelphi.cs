using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class OracleOfDelphi : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float activationDistance = 3f;
    [SerializeField] private float answerDisplayDuration = 6f;

    [Header("Input Settings")]
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
    [SerializeField] private AudioClip thinkingSound;
    [SerializeField] private AudioClip answerSound;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem mysticEffect;
    [SerializeField] private Light glowLight;

    private Transform playerCamera;
    private bool isProcessing = false;
    private bool isRecording = false;
    private bool hasInitialClick = false;
    private float recordStartTime;
    private XRSimpleInteractable interactable;
    private GreeceRoomController roomController;
    private IXRInteractor currentInteractor;

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
        // Find the room controller
        roomController = FindObjectOfType<GreeceRoomController>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Find player camera
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin != null)
            playerCamera = xrOrigin.Camera.transform;

        // Get or create XR interaction component
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        }

        // Ensure Oracle has proper setup for VR interaction
        var collider = GetComponent<Collider>();
        if (collider == null)
        {
            // Add Box Collider if none exists
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(2f, 3f, 2f); // Reasonable size for Oracle interaction
            Debug.Log($"Oracle {gameObject.name}: Added Box Collider for VR interaction");
        }
        else
        {
            // Ensure collider is set as trigger for proper VR interaction
            collider.isTrigger = true;
            Debug.Log($"Oracle {gameObject.name}: Collider configured as trigger");
        }

        interactable.selectEntered.AddListener(OnOraclePressed);
        interactable.selectExited.AddListener(OnOracleReleased);


        Debug.Log($"Oracle {gameObject.name} setup complete - Layer: {gameObject.layer}, Collider: {collider?.GetType().Name}, IsTrigger: {collider?.isTrigger}");

        // Hide UI initially
        if (hoverPrompt != null) hoverPrompt.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.gameObject.SetActive(false);
        if (recordingIndicator != null) recordingIndicator.SetActive(false);
        if (thinkingIndicator != null) thinkingIndicator.SetActive(false);

        if (hoverText != null)
            hoverText.text = "Click to consult the Oracle";
    }



    private void Update()
    {
        CheckPlayerProximity();
        HandleRecording();
    }
    private void CheckPlayerProximity()
    {
        if (playerCamera == null || isProcessing) return;

        float distance = Vector3.Distance(transform.position, playerCamera.position);
        bool isClose = distance <= activationDistance;

        // Show hover prompt when close
        if (hoverPrompt != null)
        {
            if (isClose && !hoverPrompt.activeSelf && !dialoguePanel.gameObject.activeSelf)
            {
                hoverPrompt.SetActive(true);
            }
            else if (!isClose && hoverPrompt.activeSelf)
            {
                hoverPrompt.SetActive(false);
            }
        }

        // Make hover prompt face player
        if (hoverPrompt != null && hoverPrompt.activeSelf)
        {
            hoverPrompt.transform.LookAt(playerCamera);
            hoverPrompt.transform.Rotate(0, 180, 0);
        }
    }

    private void HandleRecording()
    {
        if (isRecording && currentInteractor != null)
        {
            float recordTime = Time.time - recordStartTime;
            bool isButtonPressed = IsControllerButtonPressed();

            // Update recording UI with time elapsed
            if (dialoguePanel != null)
            {
                dialoguePanel.SetText($"Waiting for question...\n({recordTime:F1}s elapsed)");
            }

            // Check if button was released and minimum time has passed
            if (!isButtonPressed && recordTime >= minRecordingTime)
            {
                CompleteRecording();
            }
        }
    }

    private bool IsControllerButtonPressed()
    {
        if (currentInteractor == null) return false;

        // Check if grip or trigger is pressed
        bool gripPressed = gripAction != null && gripAction.action.ReadValue<float>() > 0.5f;
        bool triggerPressed = triggerAction != null && triggerAction.action.ReadValue<float>() > 0.5f;

        return gripPressed || triggerPressed;
    }

    private void OnOraclePressed(SelectEnterEventArgs args)
    {
        Debug.Log($"Oracle pressed by {args.interactorObject.transform.name}");

        currentInteractor = args.interactorObject;

        if (isRecording || isProcessing)
        {
            Debug.Log("Oracle busy - recording or processing");
            return;
        }

        if (!hasInitialClick)
        {
            // First click - show prompt to ask question
            ShowQuestionPrompt();
        }
        else
        {
            // Already showed prompt, start recording if button is held
            if (IsControllerButtonPressed())
            {
                StartRecording();
            }
        }
    }

    private void OnOracleReleased(SelectExitEventArgs args)
    {
        currentInteractor = null;

        if (isRecording)
        {
            float recordTime = Time.time - recordStartTime;
            if (recordTime >= minRecordingTime)
            {
                CompleteRecording();
            }
            else
            {
                CancelRecording();
            }
        }
    }

    private void ShowQuestionPrompt()
    {
        hasInitialClick = true;

        if (hoverPrompt != null) hoverPrompt.SetActive(false);

        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(true);
            dialoguePanel.SetText("Hold down the controller button and ask your question to the Oracle.");
        }

        // Start checking for button press to begin recording
        StartCoroutine(WaitForButtonPress());
    }

    private IEnumerator WaitForButtonPress()
    {
        while (hasInitialClick && !isRecording && !isProcessing)
        {
            if (currentInteractor != null && IsControllerButtonPressed())
            {
                StartRecording();
                break;
            }
            yield return null;
        }
    }

    private void StartRecording()
    {
        isRecording = true;
        recordStartTime = Time.time;

        if (hoverPrompt != null) hoverPrompt.SetActive(false);
        if (recordingIndicator != null) recordingIndicator.SetActive(true);

        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(true);
            dialoguePanel.SetText("Waiting for question...\n(0.0s elapsed)");
        }

        Debug.Log("Oracle: Started recording question");
    }

    private void CompleteRecording()
    {
        isRecording = false;
        float recordTime = Time.time - recordStartTime;

        if (recordingIndicator != null) recordingIndicator.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetText("The Oracle has heard your question...");

        Debug.Log($"Oracle: Completed recording after {recordTime:F1}s");

        // Notify room controller of Oracle interaction
        if (roomController != null)
        {
            roomController.OnOracleInteraction();
        }

        StartCoroutine(OracleResponse());
    }

    private void CancelRecording()
    {
        isRecording = false;

        if (recordingIndicator != null) recordingIndicator.SetActive(false);

        if (dialoguePanel != null)
        {
            dialoguePanel.SetText("Hold down the controller button and ask your question to the Oracle.");
        }

        Debug.Log("Oracle: Recording cancelled - button released too quickly");

        // Continue waiting for button press
        StartCoroutine(WaitForButtonPress());
    }


    private IEnumerator OracleResponse()
    {
        isProcessing = true;

        // Oracle is thinking
        if (dialoguePanel != null)
            dialoguePanel.SetText("Searching for answers...");

        if (thinkingIndicator != null) thinkingIndicator.SetActive(true);
        if (mysticEffect != null) mysticEffect.Play();
        if (glowLight != null) glowLight.intensity = 2f;

        if (oracleAnimator != null)
            oracleAnimator.SetTrigger("Think");

        if (thinkingSound != null && audioSource != null)
            audioSource.PlayOneShot(thinkingSound);

        yield return new WaitForSeconds(thinkingTime);

        // Gives answer
        if (thinkingIndicator != null) thinkingIndicator.SetActive(false);

        string answer = GenerateAnswer();
        if (dialoguePanel != null)
            dialoguePanel.SetText(answer);

        if (oracleAnimator != null)
            oracleAnimator.SetTrigger("Speak");

        if (answerSound != null && audioSource != null)
            audioSource.PlayOneShot(answerSound);

        if (glowLight != null) glowLight.intensity = 3f;

        // Displays answer
        yield return new WaitForSeconds(answerDisplayDuration);

        // Reset
        if (mysticEffect != null) mysticEffect.Stop();
        if (glowLight != null) glowLight.intensity = 1f;
        if (dialoguePanel != null) dialoguePanel.gameObject.SetActive(false);

        // Reset interaction state
        hasInitialClick = false;
        currentInteractor = null;
        isProcessing = false;
    }
    private string GenerateAnswer()
    {
        // Randomly pick answer type between yes, no, or eeeh idk
        int answerType = Random.Range(0, 10);

        if (answerType < 4) // 40% yes
            return yesAnswers[Random.Range(0, yesAnswers.Length)];
        else if (answerType < 7) // 30% no
            return noAnswers[Random.Range(0, noAnswers.Length)];
        else // 30% uncertain
            return uncertainAnswers[Random.Range(0, uncertainAnswers.Length)];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Oracle trigger entered by: {other.gameObject.name} (Layer: {other.gameObject.layer})");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"Oracle trigger exited by: {other.gameObject.name}");
    }


}
