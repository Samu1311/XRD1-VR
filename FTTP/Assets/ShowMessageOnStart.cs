using UnityEngine;
using TMPro;

public class ShowMessageOnStart : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float showDuration = 5f;

    void Start()
    {
        if (messageText != null)
            StartCoroutine(HideMessageAfterDelay());
    }

    private System.Collections.IEnumerator HideMessageAfterDelay()
    {
        messageText.enabled = true;
        yield return new WaitForSeconds(showDuration);
        messageText.enabled = false;
    }
}
