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
    [SerializeField] private float messageDisplayDuration = 5f;

    [Header("Instructions Display")]
    [SerializeField] private InstructionTextCanvas instructionCanvas;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip completionSound;

    [Header("Events")]
    public UnityEvent OnRoomCompleted;

    // Tracking interaction states
    private int vaseInteractionCount = 0;
    private bool oracleInteracted = false;
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
        "1. Find the Oracle in the temple of Delphi",
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
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;

        // Initially hide portal and completion message
        if (portal != null)
            portal.SetActive(false);

        // InstructionTextCanvas will be managed by this script
    }

    private void Start()
    {
        ShowWelcomeMessage();
    }

    private void ShowWelcomeMessage()
    {
        string welcomeMessage = "WELCOME TO ANCIENT GREECE\n\n" +
                              "Explore this ancient civilization!\n\n" +
                              "OBJECTIVE: Complete 2 interactions to move forward to the past\n" +
                              "• Consult the Oracle of Delphi in the colourful temple of Delphi\n" +
                              "• Examine ancient Greek vases and learn about different myths\n" +
                              "• Practice archery with the bow\n\n" +
                              "Use your controller's front button to interact";
        ShowInstructionText(welcomeMessage);
    }

    /// Call  the following methods when following interactions occur
    public void OnOracleInteraction()
    {
        if (!oracleInteracted)
        {
            oracleInteracted = true;
            RegisterInteraction("Oracle");
            Debug.Log("Greece Room: Oracle interaction registered");

            ShowOracleInstructions();
        }
    }

    public void OnVaseInteraction()
    {
        if (vaseInteractionCount > 0)
        {
            vaseInteractionCount++;
            RegisterInteraction("Vase");
            Debug.Log("Greece Room: Vase interaction registered");

            ShowVaseInstructions();
        }
    }

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
        ShowInstructionText(string.Join("\n", bowInstructions));
    }




    /// Shows instructions when first interacted with
    public void ShowOracleInstructions()
    {
        ShowInstructionText(string.Join("\n", oracleInstructions));
    }

    public void ShowVaseInstructions()
    {
        ShowInstructionText(string.Join("\n", vaseInstructions));
    }

    private void ShowInstructionText(string textToShow)
    {
        if (instructionCanvas == null)
            instructionCanvas = FindObjectOfType<InstructionTextCanvas>();

        if (instructionCanvas != null)
            instructionCanvas.ShowInstructions(textToShow);
    }

    // Canvas positioning and hiding is now handled by InstructionTextCanvas script

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

        // Show completion message and play sound if we end up adding it
        ShowCompletionMessage();

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

        // Completion event!
        OnRoomCompleted?.Invoke();
    }

    private void ShowCompletionMessage()
    {
        string message = GetCompletionMessage();
        ShowInstructionText(message);
    }

    private string GetCompletionMessage()
    {
        var interactionsList = new System.Collections.Generic.List<string>();

        if (vaseInteractionCount > 0)
            interactionsList.Add(vaseInteractionCount == 1 ? "• Interacted with 1 vase" : $"• Interacted with {vaseInteractionCount} vases");
        if (bowInteracted)
            interactionsList.Add("• Tried archery");
        if (oracleInteracted)
            interactionsList.Add("• Spoken with the Oracle");

        string details = interactionsList.Count > 0 ? string.Join("\n", interactionsList) : "• None";

        return $"You have successfully explored Ancient Greece!\n\n" +
               $"You have:\n{details}\n\n" +
               $"The wisdom of the ancients flows through you!\n" +
               $"As a reward, the portal to the next room has been activated! Head to the Parthenon to find it and continue your journey elsewhere...\n";
    }

}