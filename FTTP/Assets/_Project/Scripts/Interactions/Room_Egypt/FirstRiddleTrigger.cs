using UnityEngine;

public class FirstRiddleTrigger : MonoBehaviour
{
    public SphinxClueManager clueManager; 
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player")) 
        {
            triggered = true;
            clueManager.StartFirstRiddle();
        }
    }
}

