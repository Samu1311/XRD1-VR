using UnityEngine;
using System.Collections;

public class SphinxTalk : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip introClip;
    public AudioClip clue1Clip;
    public AudioClip clue2Clip;
    public AudioClip clue3Clip;
    public AudioClip finalClip;

    public Animator animator;

    // Track which audio was last played
    public AudioClip lastClipPlayed;

    public void PlayIntro() => PlayClip(introClip);
    public void SayClue1() => PlayClip(clue1Clip);
    public void SayClue2() => PlayClip(clue2Clip);
    public void SayClue3() => PlayClip(clue3Clip);
    public void SayFinalMessage() => PlayClip(finalClip);

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        // Remember the clip that is currently being played
        lastClipPlayed = clip;

        // Start talking animation
        if (animator != null)
            animator.SetBool("IsTalking", true);

        audioSource.clip = clip;
        audioSource.Play();

        // Start coroutine to stop talking when audio ends
        StartCoroutine(StopTalkingAfterClip());
    }

    private IEnumerator StopTalkingAfterClip()
    {
        yield return new WaitForSeconds(audioSource.clip.length);

        if (animator != null)
            animator.SetBool("IsTalking", false);
    }
}
