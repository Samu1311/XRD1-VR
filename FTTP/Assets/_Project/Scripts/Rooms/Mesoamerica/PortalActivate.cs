using UnityEngine;
using System.Collections;

public class PortalActivate : MonoBehaviour
{
    public Animator portalAnimator;

    public void ActivatePortal()
    {
        if (portalAnimator != null)
        {
            // Play the animation by setting the trigger
            portalAnimator.SetTrigger("Activate");

            // Stop the Animator after the animation length
            StartCoroutine(StopAnimatorAfterAnimation());
        }
    }

    private IEnumerator StopAnimatorAfterAnimation()
    {
        // Wait until the current animation finishes
        yield return new WaitForSeconds(portalAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Stop the Animator so it doesn't keep running
        portalAnimator.enabled = false;
    }
}
