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
        if (cam == null) return;

        Vector3 targetPosition = cam.transform.position + cam.transform.forward * distanceFromPlayer;
        transform.position = targetPosition;
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    // Hide the canvas after a delay
    private System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        gameObject.SetActive(false);
    }
}
