using UnityEngine;

public class SceneIntroTrigger : MonoBehaviour
{
    public SphinxTalk sphinxTalk;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            if (sphinxTalk != null)
                sphinxTalk.PlayIntro();
        }
    }
}
