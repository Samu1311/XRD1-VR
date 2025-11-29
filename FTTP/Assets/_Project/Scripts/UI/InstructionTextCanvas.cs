using UnityEngine;
using TMPro;

/// Simple script to display instructions on a world-space canvas in front of the player

public class InstructionTextCanvas : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Display Settings")]
    [SerializeField] private float displayDuration = 6f;
    [SerializeField] private float distanceFromPlayer = 2f;

    private Coroutine hideCoroutine;

    private void Start()
    {
        // Initially hide the canvas - it will be shown when ShowInstructions is called
        gameObject.SetActive(false);
    }

    // Show instructions and position canvas once in front of player
    public void ShowInstructions(string text)
    {
        if (instructionText == null) return;
        instructionText.text = text;
        PositionInFrontOfPlayer();
        gameObject.SetActive(true);
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    // Position the canvas in front of the player
    private void PositionInFrontOfPlayer()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            // Try to find the main camera if not found
            cam = FindObjectOfType<Camera>();
            if (cam == null) return;
        }

        // Position slightly elevated and in front of player
        Vector3 forward = cam.transform.forward;
        forward.y = 0; // Keep canvas at player's eye level, not angled down
        forward.Normalize();

        Vector3 targetPosition = cam.transform.position + forward * distanceFromPlayer;
        targetPosition.y += 0.1f; // Slight upward offset for better visibility

        transform.position = targetPosition;
        // Look at the camera (player)
        transform.LookAt(cam.transform.position);
        // Correct the rotation so text faces the player
        transform.Rotate(0, 180f, 0);
    }

    // Hide the canvas after a delay
    private System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        gameObject.SetActive(false);
    }
}
