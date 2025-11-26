using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GongInteraction : MonoBehaviour
{
    [Header("Door Animation")]
    public Animator doorAnimator;          // Assign in Inspector
    public string doorTriggerName = "OpenDoor";

    private AudioSource gongAudio;

    void Awake()
    {
        gongAudio = GetComponent<AudioSource>();
    }

    public void PlayGong(SelectEnterEventArgs args)
    {
        // Play gong sound
        if (gongAudio != null)
            gongAudio.Play();

        Debug.Log("Gong was hit!");

        // Trigger door opening animation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(doorTriggerName);
            Debug.Log("Door opening triggered!");
        }
        else
        {
            Debug.LogWarning("Door Animator not assigned on GongInteraction!");
        }
    }
}
