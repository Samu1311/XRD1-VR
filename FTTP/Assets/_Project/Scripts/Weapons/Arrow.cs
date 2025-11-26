using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Arrow : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float drag = 0.1f;
    [SerializeField] private float angularDrag = 0.5f;
    [SerializeField] private float gravityMultiplier = 1f;

    [Header("Impact")]
    [SerializeField] private float minimumImpactForce = 5f;
    [SerializeField] private float stickDepth = 0.1f;
    [SerializeField] private LayerMask stickToLayers = -1;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip whooshSound;
    [SerializeField] private AudioClip[] impactSounds;
    [SerializeField, Range(0f, 1f)] private float whooshVolume = 0.5f;
    [SerializeField] private float whooshSpeedThreshold = 3f;

    private Rigidbody rb;
    private Collider arrowCollider;
    private bool isFlying;
    private bool isStuck;
    private float lifetime;
    private float spawnTime;
    private Vector3 previousVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        arrowCollider = GetComponent<Collider>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.loop = false;

        ConfigureRigidbody(false);
    }

    public void Initialize(float arrowLifetime)
    {
        lifetime = arrowLifetime;
        spawnTime = Time.time;
        isFlying = false;
        isStuck = false;
    }

    public void Release(Vector3 direction, float force)
    {
        ConfigureRigidbody(true);

        rb.AddForce(direction * force, ForceMode.Impulse);
        isFlying = true;

        if (whooshSound != null)
        {
            audioSource.PlayOneShot(whooshSound, whooshVolume);
        }
    }

    private void FixedUpdate()
    {
        if (!isFlying || isStuck) return;

        // Apply custom gravity
        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

        // Orient arrow in the direction of travel 
        if (rb.velocity.magnitude > 0.1f)
        {
            transform.forward = rb.velocity.normalized;
        }

        // Whoosh sound based on speed, weehee
        if (whooshSound != null && rb.velocity.magnitude > whooshSpeedThreshold)
        {
            if (!audioSource.isPlaying)
            {
                float speedFactor = Mathf.Clamp01(rb.velocity.magnitude / 20f);
                audioSource.pitch = 0.9f + speedFactor * 0.3f;
                audioSource.PlayOneShot(whooshSound, whooshVolume * speedFactor);
            }
        }

        previousVelocity = rb.velocity;

        // Check lifetime for self-destruction
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isStuck || !isFlying) return;

        float impactForce = previousVelocity.magnitude;

        if (impactForce < minimumImpactForce)
        {
            // Bounce off weakly
            PlayImpactSound(0.3f);
            return;
        }

        // Check if we should stick?
        if (((1 << collision.gameObject.layer) & stickToLayers) != 0)
        {
            StickToSurface(collision);
        }
        else
        {
            // Othrwise just stop the arrow
            isStuck = true;
            ConfigureRigidbody(false);
        }

        PlayImpactSound(Mathf.Clamp01(impactForce / 20f));
    }

    private void StickToSurface(Collision collision)
    {
        isStuck = true;
        isFlying = false;

        // Disable physics, who needs them
        ConfigureRigidbody(false);

        // Position the arrow slightly into surface
        ContactPoint contact = collision.contacts[0];
        transform.position = contact.point + contact.normal * stickDepth;

        // Parent to hit object if it has a rigidbody (for moving targets)
        if (collision.rigidbody != null)
        {
            transform.SetParent(collision.transform);
        }

        // Disable collider to prevent further collisions
        if (arrowCollider != null)
        {
            arrowCollider.enabled = false;
        }
    }

    private void ConfigureRigidbody(bool flying)
    {
        if (flying)
        {
            rb.isKinematic = false;
            rb.useGravity = false; // We apply custom gravity (defyiiiing graavityyyy)
            rb.drag = drag;
            rb.angularDrag = angularDrag;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        else
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void PlayImpactSound(float volumeMultiplier)
    {
        if (impactSounds == null || impactSounds.Length == 0) return;

        AudioClip clip = impactSounds[Random.Range(0, impactSounds.Length)];
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volumeMultiplier);
        }
    }

    private void OnDrawGizmos()
    {
        if (isFlying && !isStuck)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 0.3f);
        }
    }
}
