using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;

/// Manages the Greece room puzzle progression and portal activation
/// Tracks interactions with vases, oracle, and bow/arrows
public class GreeceRoomController : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [SerializeField] private int requiredInteractions = 2;
    [SerializeField] private GameObject portal;
    [SerializeField] private float completionDelay = 2f;

    [Header("Completion Message")]
    [SerializeField] private GameObject completionTextPanel;
    [SerializeField] private TextMeshProUGUI completionText;
    [SerializeField] private float messageDisplayDuration = 5f;
    [SerializeField] private Vector3 textOffset = new Vector3(0f, 2f, 0f);

    [Header("Instructions Display")]
    [SerializeField] private GameObject bowInstructionsPanel;
    [SerializeField] private TextMeshProUGUI bowInstructionsText;
    [SerializeField] private GameObject welcomeTextPanel;
    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private float instructionDisplayDuration = 8f;
    [SerializeField] private float welcomeDisplayDuration = 6f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip completionSound;

    [Header("Events")]
    public UnityEvent OnRoomCompleted;

    // Tracking interaction states
    private bool oracleInteracted = false;
    private bool vaseInteracted = false;
    private bool bowInteracted = false;
    private int totalInteractions = 0;
    private bool isCompleted = false;

    // Instruction texts
    private readonly string[] bowInstructions = {
        "BOW & ARROW INSTRUCTIONS",
        "",
        "1. Grab the bow with one hand",
        "2. Grab the string with your other hand",
        "3. Pull the string back to nock an arrow",
        "4. Aim at the target",
        "5. Release the string to fire!",
        "",
        "TIP: Pull further back for more power"
    };

    private readonly string[] oracleInstructions = {
        "ORACLE OF DELPHI INSTRUCTIONS",
        "",
        "1. Point at the Oracle",
        "2. Hold the front button",
        "3. Ask a yes/no question aloud",
        "4. Release button when done asking",
        "5. Wait for the Oracle's wisdom",
        "",
        "TIP: Speak clearly and ask simple questions"
    };

    private readonly string[] vaseInstructions = {
        "GREEK VASE INSTRUCTIONS",
        "",
        "1. Point at any Greek vase",
        "2. Press the front button once",
        "3. Read the ancient myth story",
        "4. Learn about Greek culture",
        "",
        "TIP: Try different vases for different stories"
    };

    private void Awake()
    {
        // Setup audio source
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;

        // Initially hide portal and completion message
        if (portal != null)
            portal.SetActive(false);

        if (completionTextPanel != null)
            completionTextPanel.SetActive(false);

        if (bowInstructionsPanel != null)
            bowInstructionsPanel.SetActive(false);

        if (welcomeTextPanel != null)
            welcomeTextPanel.SetActive(false);
    }

    private void Start()
    {
        // Show welcome message when entering room
        StartCoroutine(ShowWelcomeMessageDelayed(1f));
    }

    /// Call this when the Oracle is interacted with
    public void OnOracleInteraction()
    {
        if (!oracleInteracted)
        {
            oracleInteracted = true;
            RegisterInteraction("Oracle");
            Debug.Log("Greece Room: Oracle interaction registered");

            // Show oracle instructions when first used
            ShowOracleInstructions();
        }
    }

    /// Call this when any vase is interacted with
    public void OnVaseInteraction()
    {
        if (!vaseInteracted)
        {
            vaseInteracted = true;
            RegisterInteraction("Vase");
            Debug.Log("Greece Room: Vase interaction registered");

            ShowVaseInstructions();
        }
    }

    /// Call this when bow is grabbed or arrow is fired
    public void OnBowInteraction()
    {
        if (!bowInteracted)
        {
            bowInteracted = true;
            RegisterInteraction("Bow");
            Debug.Log("Greece Room: Bow interaction registered");

            ShowBowInstructions();
        }
    }

    /// Manual method to show bow instructions
    public void ShowBowInstructions()
    {
        if (bowInstructionsPanel != null && bowInstructionsText != null)
        {
            bowInstructionsText.text = string.Join("\n", bowInstructions);
            StartCoroutine(DisplayBowInstructions());
        }
    }

    private IEnumerator ShowWelcomeMessageDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowWelcomeMessage();
    }

    /// Shows welcome message when entering the room
    public void ShowWelcomeMessage()
    {
        if (welcomeTextPanel != null && welcomeText != null)
        {
            string welcomeMessage = "WELCOME TO ANCIENT GREECE\n\n" +
                                  "Explore this ancient civilization!\n\n" +
                                  "OBJECTIVE: Complete 2 interactions\n" +
                                  "• Consult the Oracle of Delphi\n" +
                                  "• Examine ancient Greek vases\n" +
                                  "• Practice archery with the bow\n\n" +
                                  "Use your controller's front button to interact";

            welcomeText.text = welcomeMessage;
            StartCoroutine(DisplayWelcomeMessage());
        }
    }

    private IEnumerator DisplayWelcomeMessage()
    {
        welcomeTextPanel.SetActive(true);
        yield return new WaitForSeconds(welcomeDisplayDuration);
        welcomeTextPanel.SetActive(false);
    }

    private IEnumerator DisplayBowInstructions()
    {
        bowInstructionsPanel.SetActive(true);
        yield return new WaitForSeconds(instructionDisplayDuration);
        bowInstructionsPanel.SetActive(false);
    }

    /// <summary>
    /// Shows Oracle instructions when first interacted with
    /// </summary>
    public void ShowOracleInstructions()
    {
        ShowInstructionText(string.Join("\n", oracleInstructions));
    }

    /// <summary>
    /// Shows Vase instructions when first interacted with
    /// </summary>
    public void ShowVaseInstructions()
    {
        ShowInstructionText(string.Join("\n", vaseInstructions));
    }

    /// Generic method to show instruction text using the bow instructions panel
    private void ShowInstructionText(string instructionText)
    {
        if (bowInstructionsPanel != null && bowInstructionsText != null)
        {
            bowInstructionsText.text = instructionText;
            StartCoroutine(DisplayBowInstructions()); // Reuse the same display coroutine
        }
    }

    private void RegisterInteraction(string interactionType)
    {
        totalInteractions++;
        Debug.Log($"Greece Room: {interactionType} interacted. Total: {totalInteractions}/{requiredInteractions}");

        if (totalInteractions >= requiredInteractions && !isCompleted)
        {
            StartCoroutine(CompleteRoom());
        }
    }

    private IEnumerator CompleteRoom()
    {
        isCompleted = true;
        Debug.Log("Greece Room: Puzzle completed!");

        // Wait a moment for dramatic effect
        yield return new WaitForSeconds(completionDelay);

        // Show completion message
        ShowCompletionMessage();

        // Play completion sound
        if (completionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(completionSound);
        }

        // Activate portal after message display
        yield return new WaitForSeconds(messageDisplayDuration * 0.7f); // Show portal before text disappears

        if (portal != null)
        {
            portal.SetActive(true);
            Debug.Log("Greece Room: Portal activated!");
        }

        // Fire completion event
        OnRoomCompleted?.Invoke();
    }

    private void ShowCompletionMessage()
    {
        if (completionTextPanel == null || completionText == null) return;

        // Set completion message
        string message = GetCompletionMessage();
        completionText.text = message;

        // Show the message
        completionTextPanel.SetActive(true);

        // Hide after duration
        StartCoroutine(HideCompletionMessage());
    }

    private IEnumerator HideCompletionMessage()
    {
        yield return new WaitForSeconds(messageDisplayDuration);
        if (completionTextPanel != null)
        {
            completionTextPanel.SetActive(false);
        }
    }

    private string GetCompletionMessage()
    {
        string interactions = "";
        if (oracleInteracted) interactions += "Oracle ";
        if (vaseInteracted) interactions += "Vase ";
        if (bowInteracted) interactions += "Bow ";

        return $"🎉 CONGRATULATIONS! 🎉\n\n" +
               $"You have successfully explored Ancient Greece!\n\n" +
               $"You interacted with: {interactions.Trim()}\n\n" +
               $"The wisdom of the ancients flows through you.\n" +
               $"Your time travel journey continues...\n\n" +
               $"✨ Portal to Main Hub Activated ✨";
    }

    // Public methods to check interaction states (for debugging/inspector)
    public bool IsOracleInteracted => oracleInteracted;
    public bool IsVaseInteracted => vaseInteracted;
    public bool IsBowInteracted => bowInteracted;
    public int TotalInteractions => totalInteractions;
    public bool IsCompleted => isCompleted;
}