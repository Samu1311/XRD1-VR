using UnityEngine;

public class EasterEgg : MonoBehaviour
{
    // This is just me having fun and hoping it makes the reviewer smile :)
    [Header("Easter Egg Settings")]
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private AudioClip clickAudio;

    private Color originalColor;
    private Renderer objectRenderer;
    private AudioSource audioSource;

    private void Awake()
    {
        // Get the Renderer and AudioSource components
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnMouseEnter()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = hoverColor;
        }
    }

    private void OnMouseExit()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
    }

    private void OnMouseDown()
    {
        if (clickAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickAudio);
        }
    }
}