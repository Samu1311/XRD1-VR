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
    [SerializeField] private PortalActivate portalActivate;
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
    private System.Collections.Generic.HashSet<string> interactedVases = new System.Collections.Generic.HashSet<string>();
    private bool oracleInteracted = false;
    private int totalInteractions = 0;
    private bool isCompleted = false;


    private void Awake()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        StartCoroutine(ShowWelcomeMessageDelayed());
    }

    private System.Collections.IEnumerator ShowWelcomeMessageDelayed()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("GreeceRoomController: Showing welcome message after 5 second delay");
        ShowWelcomeMessage();
    }

    private void ShowWelcomeMessage()
    {
        string welcomeMessage = "WELCOME TO ANCIENT GREECE\n\n" +
                              "OBJECTIVE:\n" +
                              "Complete 2 interactions to unlock the portal\n\n" +
                              "• Find the Oracle of Delphi in front of the colourful\n" +
                              "  Temple of Delphi and ask her a yes/no question!\n\n" +
                              "• Examine ancient Greek vases to learn about myths!\n" +
                              "Use your controller's side button to interact.";
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

            // Oracle instructions are provided in welcome message
        }
    }

    public void OnVaseInteraction(string vaseId = null)
    {
        // If no vase ID provided, generate a default one
        if (string.IsNullOrEmpty(vaseId))
        {
            vaseId = "UnknownVase_" + UnityEngine.Random.Range(1000, 9999);
            Debug.LogWarning("Greece Room: Vase interaction without ID. Use OnVaseInteractionWithId() instead for proper tracking.");
        }

        // Only register if this is a new vase
        if (interactedVases.Add(vaseId))
        {
            RegisterInteraction($"Vase ({vaseId})");
            Debug.Log($"Greece Room: New vase interaction registered: {vaseId}. Total unique vases: {interactedVases.Count}");
        }
        else
        {
            Debug.Log($"Greece Room: Vase {vaseId} already interacted with. No progress.");
        }
    }


    // Method for vase interaction scripts to call with their unique identifier
    public void OnVaseInteractionWithId(string vaseId)
    {
        OnVaseInteraction(vaseId);
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

        yield return new WaitForSeconds(completionDelay);

        // Show completion message and play sound if we end up adding it
        ShowCompletionMessage();

        if (completionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(completionSound);
        }

        // Activate portal after message display
        yield return new WaitForSeconds(messageDisplayDuration * 0.7f); // Show portal before text disappears

        if (portalActivate != null)
        {
            portalActivate.ActivatePortal();
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

        if (interactedVases.Count > 0)
            interactionsList.Add(interactedVases.Count == 1 ? "• Interacted with 1 unique vase" : $"• Interacted with {interactedVases.Count} unique vases");
        if (oracleInteracted)
            interactionsList.Add("• Spoken with the Oracle");

        string details = interactionsList.Count > 0 ? string.Join("\n", interactionsList) : "• None";

        return $"You have successfully explored Ancient Greece!\n\n" +
               $"You have:\n{details}\n\n" +
               $"The wisdom of the ancients flows through you!\n\n" +
               $"The portal to the next era has been activated!\n" +
               $"You can continue exploring Greece or\n" +
               $"head to the Parthenon to travel further...\n";
    }

}