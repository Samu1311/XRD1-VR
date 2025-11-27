using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public PortalActivate portal;          // Your portal script
    public SphinxClueManager clueManager;  // Reference to the manager that tracks collected items
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            // Only activate if all required objects are collected
            if (clueManager != null && clueManager.AllItemsCollected())
            {
                triggered = true;

                if (portal != null)
                    portal.ActivatePortal();
            }
        }
    }
}
