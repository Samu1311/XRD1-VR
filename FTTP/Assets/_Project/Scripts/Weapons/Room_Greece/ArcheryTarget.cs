using UnityEngine;

public class ArcheryTarget : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private int pointValue = 10;
    [SerializeField] private float hitEffectDuration = 2f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private Color hitColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;

    private AudioSource audioSource;
    private Renderer targetRenderer;
    private Color originalColor;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            originalColor = targetRenderer.material.color;
        }

        // Ensure we have a trigger collider
        var collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError($"ArcheryTarget {gameObject.name} needs a Collider component!");
        }
        else if (!collider.isTrigger)
        {
            Debug.LogWarning($"ArcheryTarget {gameObject.name} collider should be set as Trigger for proper detection");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if an arrow hit us
        var arrow = other.GetComponent<VRArrow>();
        if (arrow != null)
        {
            OnArrowHit(arrow);
        }
    }

    public void OnArrowHit(VRArrow arrow)
    {
        Debug.Log($"Target hit! Points: {pointValue}");

        // Play hit sound
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Show hit effects if we make 'em
        ShowHitEffects(arrow.transform.position);

        RegisterAction();

        var arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.velocity = Vector3.zero;
            arrowRb.isKinematic = true;
        }
    }

    private void ShowHitEffects(Vector3 hitPosition)
    {
        // Change target color briefly
        if (targetRenderer != null)
        {
            StartCoroutine(FlashColor());
        }

        // Play particle effect
        if (hitEffect != null)
        {
            hitEffect.transform.position = hitPosition;
            hitEffect.Play();
        }

        // Show hit marker
        if (hitMarker != null)
        {
            GameObject marker = Instantiate(hitMarker, hitPosition, Quaternion.identity);
            Destroy(marker, hitEffectDuration);
        }
    }

    private System.Collections.IEnumerator FlashColor()
    {
        if (targetRenderer != null)
        {
            targetRenderer.material.color = hitColor;
            yield return new WaitForSeconds(0.2f);
            targetRenderer.material.color = originalColor;
        }
    }

    private void RegisterAction()
    {
        Debug.Log($"Great shot! Arrow hit the target!");
    }
}