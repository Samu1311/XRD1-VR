using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using TMPro;

public class OracleOfDelphi : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float activationDistance = 3f;
    [SerializeField] private float thinkingDuration = 2.5f;
    [SerializeField] private float answerDisplayDuration = 6f;

    [Header("Input Settings")]
    [SerializeField] private float autoAskDelay = 2f;


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
    private bool hasAskedQuestion = false;
    private float closeStartTime;

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
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Find player camera
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin != null)
            playerCamera = xrOrigin.Camera.transform;

        // Hide UI initially
        if (hoverPrompt != null) hoverPrompt.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.gameObject.SetActive(false);
        if (recordingIndicator != null) recordingIndicator.SetActive(false);
        if (thinkingIndicator != null) thinkingIndicator.SetActive(false);

        // Set hover text
        if (hoverText != null)
            hoverText.text = "Get a Reading\n(Ask a yes/no question)";
    }



    private void Update()
    {
        CheckPlayerProximity();
        CheckAutoAsk();
    }

    private void CheckPlayerProximity()
    {
        if (playerCamera == null || isProcessing) return;

        float distance = Vector3.Distance(transform.position, playerCamera.position);
        bool isClose = distance <= activationDistance;

        // Show hover prompt when close
        if (hoverPrompt != null)
        {
            if (isClose && !hoverPrompt.activeSelf && !dialoguePanel.gameObject.activeSelf && !hasAskedQuestion)
            {
                hoverPrompt.SetActive(true);
                closeStartTime = Time.time;
            }
            else if (!isClose && hoverPrompt.activeSelf)
            {
                hoverPrompt.SetActive(false);
                hasAskedQuestion = false;
            }
        }

        // Make hover prompt face player
        if (hoverPrompt != null && hoverPrompt.activeSelf)
        {
            hoverPrompt.transform.LookAt(playerCamera);
            hoverPrompt.transform.Rotate(0, 180, 0);
        }
    }

    private void CheckAutoAsk()
    {
        if (isProcessing || hasAskedQuestion) return;
        if (hoverPrompt == null || !hoverPrompt.activeSelf) return;

        // Auto-ask question after player is close for a while
        if (Time.time - closeStartTime > autoAskDelay)
        {
            AskQuestion();
        }
    }

    private void AskQuestion()
    {
        hasAskedQuestion = true;

        if (hoverPrompt != null) hoverPrompt.SetActive(false);
        if (recordingIndicator != null) recordingIndicator.SetActive(false);

        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(true);
            dialoguePanel.SetText("What do you most desire to know about the future?");
        }

        // Wait a moment, then give answer
        StartCoroutine(DelayedResponse());
    }

    private IEnumerator DelayedResponse()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(OracleResponse());
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
        yield return new WaitForSeconds(thinkingDuration);

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

        // Reset so player can ask again
        yield return new WaitForSeconds(2f);
        hasAskedQuestion = false;
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
}
