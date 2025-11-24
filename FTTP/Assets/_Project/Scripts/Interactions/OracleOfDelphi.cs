// Uncomment to enable desktop testing with mouse/keyboard
#define ENABLE_DESKTOP_TESTING

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
#if ENABLE_DESKTOP_TESTING
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Oracle of Delphi dialogue system with voice/button input for prophetic questions.
/// Attach to the Oracle character.
/// </summary>
#if ENABLE_DESKTOP_TESTING
public class OracleOfDelphi : MonoBehaviour, IInteractable
#else
public class OracleOfDelphi : MonoBehaviour
#endif
{
    [System.Serializable]
    public class PredefinedQuestion
    {
        public string question;
        public string[] possibleAnswers;
    }

    [Header("Dialogue Settings")]
    [SerializeField] private float thinkingDuration = 2f;
    [SerializeField] private float answerDisplayDuration = 5f;
    [SerializeField] private float activationDistance = 3f;

    [Header("Predefined Questions")]
    [SerializeField] private List<PredefinedQuestion> questions = new List<PredefinedQuestion>();

    [Header("Voice Input Settings")]
    [SerializeField] private bool useVoiceInput = true;
    [SerializeField] private KeyCode voiceInputButton = KeyCode.JoystickButton0; // Controller trigger
    [SerializeField] private float maxRecordingTime = 10f;

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI oracleResponseText;
    [SerializeField] private TextMeshProUGUI recordingIndicator;
    [SerializeField] private GameObject questionMenu;
    [SerializeField] private Transform questionButtonContainer;
    [SerializeField] private GameObject questionButtonPrefab;

    [Header("Oracle Character")]
    [SerializeField] private Animator oracleAnimator;
    [SerializeField] private Transform playerTransform;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem mysticalEffect;
    [SerializeField] private Light oracleLight;
    [SerializeField] private Color thinkingColor = new Color(0.5f, 0f, 1f);
    [SerializeField] private Color answeringColor = new Color(1f, 0.8f, 0f);

    [Header("Audio")]
    [SerializeField] private AudioClip thinkingSound;
    [SerializeField] private AudioClip answerSound;
    [SerializeField] private AudioClip ambientSound;

    private AudioSource audioSource;
    private bool isProcessingQuestion = false;
    private bool isRecording = false;
    private float recordingStartTime;
    private Transform xrCamera;
    private List<GameObject> questionButtons = new List<GameObject>();

    // Generic prophetic responses
    private string[] genericResponses;

    private void Awake()
    {
        // Initialize generic responses
        genericResponses = new string[]
        {
            "The fates have spoken... Yes, but beware the path ahead.",
            "The oracle sees uncertainty in your future... Perhaps.",
            "The gods smile upon your endeavor. Yes.",
            "The omens are unclear... The answer lies within you.",
            "No, the gods advise against this course.",
            "The future is shrouded in mist... Yes, but with great difficulty.",
            "The oracle sees a favorable outcome.",
            "The gods remain silent on this matter... Seek wisdom elsewhere.",
            "Yes, if you prove yourself worthy.",
            "No, but do not lose hope.",
            "The path diverges before you... Choose wisely.",
            "The gods test your resolve... Yes, but not without sacrifice."
        };

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Find XR camera
        if (playerTransform == null)
        {
            var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin != null)
            {
                xrCamera = xrOrigin.Camera.transform;
                playerTransform = xrOrigin.transform;
            }
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (questionMenu != null)
            questionMenu.SetActive(false);

        if (recordingIndicator != null)
            recordingIndicator.gameObject.SetActive(false);

        // Initialize predefined questions if empty
        if (questions.Count == 0)
        {
            InitializeDefaultQuestions();
        }

        // Create question buttons
        CreateQuestionButtons();
    }

    private void Start()
    {
        // Play ambient sound
        if (ambientSound != null && audioSource != null)
        {
            audioSource.clip = ambientSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void Update()
    {
#if ENABLE_DESKTOP_TESTING
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            // Desktop testing: Number keys 1-5 to select questions quickly
            if (!isProcessingQuestion && questionMenu != null && questionMenu.activeSelf)
            {
                if (keyboard.digit1Key.wasPressedThisFrame) OnQuestionSelected(0);
                if (keyboard.digit2Key.wasPressedThisFrame) OnQuestionSelected(1);
                if (keyboard.digit3Key.wasPressedThisFrame) OnQuestionSelected(2);
                if (keyboard.digit4Key.wasPressedThisFrame) OnQuestionSelected(3);
                if (keyboard.digit5Key.wasPressedThisFrame) OnQuestionSelected(4);
            }

            // ESC to close menu
            if (keyboard.escapeKey.wasPressedThisFrame && questionMenu != null && questionMenu.activeSelf && !isProcessingQuestion)
            {
                questionMenu.SetActive(false);
                Debug.Log("[Desktop Test] Oracle menu closed with ESC");
            }
        }
#endif

        CheckPlayerProximity();
        CheckVoiceInputButton();
    }

    // IInteractable implementation
    public void OnHoverEnter()
    {
#if ENABLE_DESKTOP_TESTING
        if (oracleLight != null)
        {
            oracleLight.enabled = true;
            oracleLight.color = new Color(0.5f, 0.8f, 1f, 1f);
        }
#endif
    }

    public void OnHoverExit()
    {
#if ENABLE_DESKTOP_TESTING
        if (oracleLight != null)
        {
            oracleLight.enabled = false;
        }
#endif
    }

    public void OnInteract()
    {
#if ENABLE_DESKTOP_TESTING
        if (!isProcessingQuestion && questionMenu != null)
        {
            bool newState = !questionMenu.activeSelf;
            questionMenu.SetActive(newState);
            Debug.Log($"[Desktop Test] Oracle menu {(newState ? "opened" : "closed")}. Press 1-5 to select questions.");
        }
#endif
    }

    public string GetInteractionPrompt()
    {
        return "Ask the Oracle";
    }

    private void CheckPlayerProximity()
    {
        if (xrCamera == null || isProcessingQuestion) return;

        float distance = Vector3.Distance(transform.position, xrCamera.position);

        if (distance <= activationDistance && !questionMenu.activeSelf && !dialoguePanel.activeSelf)
        {
            ShowQuestionMenu(true);
        }
        else if (distance > activationDistance && questionMenu.activeSelf && !isProcessingQuestion)
        {
            ShowQuestionMenu(false);
        }
    }

    private void CheckVoiceInputButton()
    {
        if (!useVoiceInput || isProcessingQuestion) return;

#if ENABLE_DESKTOP_TESTING
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            // Desktop testing: Hold V key to "record" voice question
            if (keyboard.vKey.wasPressedThisFrame)
            {
                Debug.Log("[Desktop Test] Hold V to ask question, release to submit");
                StartRecording();
            }

            if (isRecording && keyboard.vKey.wasReleasedThisFrame)
            {
                Debug.Log("[Desktop Test] Voice question submitted!");
                StopRecording();
            }
        }
#else
        // Check if voice input button is pressed
        if (Input.GetKeyDown(voiceInputButton))
        {
            StartRecording();
        }
        
        if (isRecording && Input.GetKeyUp(voiceInputButton))
        {
            StopRecording();
        }
#endif

        // Auto-stop after max recording time
        if (isRecording && Time.time - recordingStartTime > maxRecordingTime)
        {
            StopRecording();
        }
    }
    private void StartRecording()
    {
        if (Vector3.Distance(transform.position, xrCamera.position) > activationDistance) return;

        isRecording = true;
        recordingStartTime = Time.time;

        if (recordingIndicator != null)
        {
            recordingIndicator.gameObject.SetActive(true);
            recordingIndicator.text = "Recording... Release to ask Oracle";
        }

        if (questionMenu != null)
            questionMenu.SetActive(false);

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            questionText.text = "Speak your question to the Oracle...";
        }
    }

    private void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;

        if (recordingIndicator != null)
            recordingIndicator.gameObject.SetActive(false);

        // Simulate voice-to-text (in real implementation, use speech recognition)
        string voiceQuestion = "O Oracle, what does the future hold?"; // Placeholder

        ProcessQuestion(voiceQuestion);
    }

    private void ShowQuestionMenu(bool show)
    {
        if (questionMenu != null)
            questionMenu.SetActive(show);
    }

    private void CreateQuestionButtons()
    {
        if (questionButtonPrefab == null || questionButtonContainer == null) return;

        // Clear existing buttons
        foreach (var btn in questionButtons)
        {
            if (btn != null) Destroy(btn);
        }
        questionButtons.Clear();

        // Create button for each predefined question
        for (int i = 0; i < questions.Count; i++)
        {
            GameObject button = Instantiate(questionButtonPrefab, questionButtonContainer);

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = questions[i].question;
            }

            // Add click listener
            int index = i; // Capture for closure
            var xrButton = button.GetComponent<UnityEngine.UI.Button>();
            if (xrButton != null)
            {
                xrButton.onClick.AddListener(() => OnQuestionSelected(index));
            }

            questionButtons.Add(button);
        }
    }

    public void OnQuestionSelected(int questionIndex)
    {
        if (questionIndex < 0 || questionIndex >= questions.Count) return;

        string question = questions[questionIndex].question;
        ProcessQuestion(question, questionIndex);
    }

    private void ProcessQuestion(string question, int questionIndex = -1)
    {
        if (isProcessingQuestion) return;

        StartCoroutine(OracleResponseSequence(question, questionIndex));
    }

    private IEnumerator OracleResponseSequence(string question, int questionIndex)
    {
        isProcessingQuestion = true;

        // Hide menu, show dialogue
        if (questionMenu != null)
            questionMenu.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Display question
        if (questionText != null)
        {
            questionText.text = $"\"{question}\"";
        }

        if (oracleResponseText != null)
        {
            oracleResponseText.text = "";
        }

        // Oracle thinking animation
        if (oracleAnimator != null)
        {
            oracleAnimator.SetTrigger("Think");
        }

        // Visual effects
        if (mysticalEffect != null)
            mysticalEffect.Play();

        if (oracleLight != null)
            oracleLight.color = thinkingColor;

        // Audio
        if (thinkingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(thinkingSound);
        }

        // Wait (thinking)
        yield return new WaitForSeconds(thinkingDuration);

        // Generate answer
        string answer = GenerateAnswer(questionIndex);

        // Display answer
        if (oracleResponseText != null)
        {
            oracleResponseText.text = answer;
        }

        // Oracle speaking animation
        if (oracleAnimator != null)
        {
            oracleAnimator.SetTrigger("Speak");
        }

        if (oracleLight != null)
            oracleLight.color = answeringColor;

        // Audio
        if (answerSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(answerSound);
        }

        // Keep answer displayed
        yield return new WaitForSeconds(answerDisplayDuration);

        // Reset
        if (mysticalEffect != null)
            mysticalEffect.Stop();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (oracleLight != null)
            oracleLight.color = Color.white;

        isProcessingQuestion = false;

        // Show menu again if player is still close
        if (Vector3.Distance(transform.position, xrCamera.position) <= activationDistance)
        {
            ShowQuestionMenu(true);
        }
    }

    private string GenerateAnswer(int questionIndex)
    {
        // Use predefined answer if available
        if (questionIndex >= 0 && questionIndex < questions.Count)
        {
            var possibleAnswers = questions[questionIndex].possibleAnswers;
            if (possibleAnswers != null && possibleAnswers.Length > 0)
            {
                return possibleAnswers[Random.Range(0, possibleAnswers.Length)];
            }
        }

        // Otherwise use generic response
        return genericResponses[Random.Range(0, genericResponses.Length)];
    }

    private void InitializeDefaultQuestions()
    {
        questions.Add(new PredefinedQuestion
        {
            question = "Will I find success in my endeavors?",
            possibleAnswers = new string[]
            {
                "Yes, if you remain steadfast and true to your purpose.",
                "The gods see great potential, but the path is perilous.",
                "Success comes to those who seek wisdom before action."
            }
        });

        questions.Add(new PredefinedQuestion
        {
            question = "Should I trust those around me?",
            possibleAnswers = new string[]
            {
                "Trust, but verify. Not all who smile are friends.",
                "The bonds of loyalty are tested in times of trial.",
                "Look to those who have proven themselves in deed, not word."
            }
        });

        questions.Add(new PredefinedQuestion
        {
            question = "What do the gods have in store for my future?",
            possibleAnswers = new string[]
            {
                "Great trials await, but also great rewards for the worthy.",
                "The Fates weave a complex tapestry... Your thread shines bright.",
                "The gods test mortals to separate heroes from the meek."
            }
        });

        questions.Add(new PredefinedQuestion
        {
            question = "Will I overcome my current challenges?",
            possibleAnswers = new string[]
            {
                "Yes, but you must call upon your inner strength.",
                "Victory comes not from avoiding challenges, but facing them.",
                "The gods favor the brave. Stand firm and you shall prevail."
            }
        });

        questions.Add(new PredefinedQuestion
        {
            question = "Is now the right time to act?",
            possibleAnswers = new string[]
            {
                "The stars align favorably. Strike while fortune smiles.",
                "Patience. The moment is not yet ripe.",
                "Timing is everything. Wait for the next full moon."
            }
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
