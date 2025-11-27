using UnityEngine;
using UnityEngine.Events;

public class ArrowTarget : MonoBehaviour
{
    [Header("Target Configuration")]
    [SerializeField] private bool destroyArrowOnHit = false;
    [SerializeField] private float destroyDelay = 0.1f;
    [SerializeField] private int points = 10;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float effectLifetime = 2f;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitColorDuration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip targetHitSound;

    [Header("Events")]
    public UnityEvent<int> onTargetHit;
    public UnityEvent onTargetDestroyed;

    private Renderer targetRenderer;
    private Color originalColor;
    private bool isHit;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            originalColor = targetRenderer.material.color;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Arrow arrow = collision.gameObject.GetComponent<Arrow>();
        if (arrow != null && !isHit)
        {
            OnArrowHit(arrow, collision);
        }
    }

    private void OnArrowHit(Arrow arrow, Collision collision)
    {
        isHit = true;

        // Invoke events
        onTargetHit?.Invoke(points);

        // Visual feedback if we have time
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, collision.contacts[0].point, Quaternion.identity);
            Destroy(effect, effectLifetime);
        }

        if (targetRenderer != null)
        {
            StartCoroutine(FlashColor());
        }

        if (targetHitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(targetHitSound);
        }

        // Handle arrow destruction
        if (destroyArrowOnHit)
        {
            Destroy(arrow.gameObject, destroyDelay);
        }
    }

    private System.Collections.IEnumerator FlashColor()
    {
        if (targetRenderer == null) yield break;

        Material mat = targetRenderer.material;
        mat.color = hitColor;

        float elapsed = 0f;
        while (elapsed < hitColorDuration)
        {
            elapsed += Time.deltaTime;
            mat.color = Color.Lerp(hitColor, originalColor, elapsed / hitColorDuration);
            yield return null;
        }

        mat.color = originalColor;
        isHit = false;
    }

    // Public method for score systems or game managers
    public int GetPoints() => points;
}
