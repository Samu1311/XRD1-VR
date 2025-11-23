using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GongInteraction : MonoBehaviour
{
    private AudioSource gongAudio;

    void Awake()
    {
        gongAudio = GetComponent<AudioSource>();
    }

    public void PlayGong(SelectEnterEventArgs args)
    {
        if (gongAudio != null)
            gongAudio.Play();

        Debug.Log("Gong was hit!");
    }
}
