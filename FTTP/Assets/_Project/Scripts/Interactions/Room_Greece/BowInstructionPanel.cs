using UnityEngine;
using TMPro;

public class BowInstructionPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private float displayDuration = 10f;

    [Header("Instructions")]
    [SerializeField]
    private string[] instructions = new string[]
    {
        "VR Bow Instructions:",
        "",
        "1. Grab the bow with one controller",
        "2. Reach behind and grab the string with your other controller",
        "3. Pull the string back to nock an arrow",
        "4. Aim by pointing the bow",
        "5. Release the string controller to fire!"
    };

    private void Start()
    {
        ShowInstructions();
    }

    private void ShowInstructions()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);

            if (instructionText != null)
            {
                instructionText.text = string.Join("\n", instructions);
            }

            // Hide after display duration
            Invoke(nameof(HideInstructions), displayDuration);
        }
    }

    private void HideInstructions()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }

    // Public method to manually show instructions
    public void DisplayInstructions()
    {
        CancelInvoke(nameof(HideInstructions));
        ShowInstructions();
    }
}