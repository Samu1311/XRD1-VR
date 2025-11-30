using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class OracleOfDelphi : MonoBehaviour
{
    [Header("Oracle Settings")]
    [SerializeField] private float answerDisplayDuration = 6f;
    [SerializeField] private float thinkingTime = 2.5f;

    [Header("UI References")]
    [SerializeField] private MysticTextPanel dialoguePanel;

    private bool isProcessing = false;
    private bool isWaitingForQuestion = false;
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

        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }

        interactable.selectEntered.AddListener(OnOracleClicked);

        // Ensure dialogue panel starts hidden
        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnOracleClicked);
        }
    }



    private void OnOracleClicked(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        if (isProcessing) return;

        if (!isWaitingForQuestion)
        {
            Debug.Log("Oracle: First click - starting interaction");

            // Show initial message
            ShowDialogue("The Oracle awakens... Speak your question aloud, then click again when you are finished.");
            isWaitingForQuestion = true;
        }
        else
        {
            Debug.Log("Oracle: Second click - question completed");

            // Player has finished asking their question
            ShowDialogue("The Oracle contemplates...");
            isWaitingForQuestion = false;

            // Start Oracle response
            StartCoroutine(OracleResponse());
        }
    }



    private IEnumerator OracleResponse()
    {
        isProcessing = true;
        yield return new WaitForSeconds(thinkingTime);
        string answer = GenerateAnswer();
        ShowDialogue(answer);

        yield return new WaitForSeconds(answerDisplayDuration);

        // Notify room controller that interaction is complete
        roomController?.OnOracleInteraction();

        dialoguePanel?.gameObject.SetActive(false);
        isProcessing = false;
        // Ready for another interaction
    }

    private string GenerateAnswer()
    {
        int roll = Random.Range(0, 10);
        if (roll < 4) return yesAnswers[Random.Range(0, yesAnswers.Length)];
        if (roll < 7) return noAnswers[Random.Range(0, noAnswers.Length)];
        return uncertainAnswers[Random.Range(0, uncertainAnswers.Length)];
    }



    private void ShowDialogue(string message)
    {
        dialoguePanel?.gameObject.SetActive(true);
        dialoguePanel?.SetText(message);
    }


}
