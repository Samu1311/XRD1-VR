using UnityEngine;
using TMPro;

/// Simple script to display instructions on a world-space canvas in front of the player
public class InstructionTextCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private float displayDuration = 6f;
    [SerializeField] private float distanceFromPlayer = 2f;

    private Coroutine hideCoroutine;

    /// Call to show instructions
    public void ShowInstructions(string text)
    {
        if (instructionText == null) return;
        instructionText.text = text;
        gameObject.SetActive(true);
        PositionInFrontOfPlayer();
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private void PositionInFrontOfPlayer()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        transform.position = cam.transform.position + cam.transform.forward * distanceFromPlayer;
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    private System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        gameObject.SetActive(false);
    }
}
