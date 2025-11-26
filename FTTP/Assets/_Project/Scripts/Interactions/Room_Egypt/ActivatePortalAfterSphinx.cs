using UnityEngine;

public class ActivatePortalAfterSphinx : MonoBehaviour
{
    public SphinxTalk sphinxTalk;      // Drag your SphinxTalk script here
    public GameObject portalObject;    // Drag the portal prefab or portal object
    public float delayAfterAudio = 1f; // Extra delay before showing the portal

    private bool portalActivated = false;

    void Update()
    {
        // If we already spawned the portal ? stop checking
        if (portalActivated) return;

        // Check if the Sphinx is playing the final audio
        if (!sphinxTalk.audioSource.isPlaying && sphinxTalk.finalClip != null)
        {
            // Portal appears ONLY after final clip was previously played
            if (sphinxTalk.lastClipPlayed == sphinxTalk.finalClip)
            {
                ActivatePortal();
            }
        }
    }

    private void ActivatePortal()
    {
        portalActivated = true;

        // Optional delay
        Invoke(nameof(ShowPortal), delayAfterAudio);
    }

    private void ShowPortal()
    {
        portalObject.SetActive(true);
    }
}
