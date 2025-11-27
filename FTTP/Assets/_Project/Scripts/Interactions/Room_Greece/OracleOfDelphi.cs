using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;
using TMPro;

public class OracleOfDelphi : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float activationDistance = 3f;
    [SerializeField] private float answerDisplayDuration = 6f;

    [Header("Input Settings")]
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float thinkingTime = 3f;


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
    private float recordStartTime;
    private XRSimpleInteractable interactable;
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

        // Ensure Oracle is on Default layer for VR interaction
        if (gameObject.layer != 0)
        {
            Debug.LogWarning($"Oracle {gameObject.name} is on layer {gameObject.layer}, changing to Default (0) for VR interaction");
            gameObject.layer = 0;
        }

        Debug.Log($"Oracle {gameObject.name} setup complete - Layer: {gameObject.layer}, Collider: {collider?.GetType().Name}, IsTrigger: {collider?.isTrigger}");

        // Hide UI initially
        if (hoverPrompt != null) hoverPrompt.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.gameObject.SetActive(false);
        if (recordingIndicator != null) recordingIndicator.SetActive(false);
        if (thinkingIndicator != null) thinkingIndicator.SetActive(false);

        // Set hover text for trigger interaction
        if (hoverText != null)
            hoverText.text = "Press trigger to consult Oracle";
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
        if (isRecording)
        {
            float recordTime = Time.time - recordStartTime;

            // Update recording UI
            if (dialoguePanel != null)
            {
                float remaining = holdDuration - recordTime;
                dialoguePanel.SetText($"Speak your question to the Oracle...\n({remaining:F1}s remaining)");
            }

            // Check if recording is complete
            if (recordTime >= holdDuration)
            {
                CompleteRecording();
            }
        }
    }

    private void OnOraclePressed(SelectEnterEventArgs args)
    {
        Debug.Log($"Oracle pressed by {args.interactorObject.transform.name}");

        if (isRecording || isProcessing)
        {
            Debug.Log("Oracle busy - recording or processing");
            return;
        }

        StartRecording();
    }

    private void OnOracleReleased(SelectExitEventArgs args)
    {
        if (isRecording && Time.time - recordStartTime < holdDuration)
        {
            CancelRecording();
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
            dialoguePanel.SetText($"Speak your question to the Oracle...\n({holdDuration:F1}s remaining)");
        }
    }

    private void CompleteRecording()
    {
        isRecording = false;

        if (recordingIndicator != null) recordingIndicator.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetText("The Oracle has heard your question...");

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
        if (dialoguePanel != null) dialoguePanel.gameObject.SetActive(false);
    }


    private IEnumerator OracleResponse()
    {
        isProcessing = true;

        // Oracle is thinking
        if (dialoguePanel != null)
            dialoguePanel.SetText("The Oracle consults the gods...");

        if (thinkingIndicator != null) thinkingIndicator.SetActive(true);
        if (mysticEffect != null) mysticEffect.Play();
        if (glowLight != null) glowLight.intensity = 2f;

        if (oracleAnimator != null)
            oracleAnimator.SetTrigger("Think");

        if (thinkingSound != null && audioSource != null)
            audioSource.PlayOneShot(thinkingSound);

        // Wait while thinking
        yield return new WaitForSeconds(thinkingTime);

        // Give answer
        if (thinkingIndicator != null) thinkingIndicator.SetActive(false);

        string answer = GenerateAnswer();
        if (dialoguePanel != null)
            dialoguePanel.SetText(answer);

        if (oracleAnimator != null)
            oracleAnimator.SetTrigger("Speak");

        if (answerSound != null && audioSource != null)
            audioSource.PlayOneShot(answerSound);

        if (glowLight != null) glowLight.intensity = 3f;

        // Display answer
        yield return new WaitForSeconds(answerDisplayDuration);

        // Reset
        if (mysticEffect != null) mysticEffect.Stop();
        if (glowLight != null) glowLight.intensity = 1f;
        if (dialoguePanel != null) dialoguePanel.gameObject.SetActive(false);

        isProcessing = false;
    }
    private string GenerateAnswer()
    {
        // Randomly pick answer type: yes, no, or uncertain
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
