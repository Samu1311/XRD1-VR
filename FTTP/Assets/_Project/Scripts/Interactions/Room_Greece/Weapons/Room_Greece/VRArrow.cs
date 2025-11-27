using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class VRArrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private float flightSpeed = 20f;
    [SerializeField] private float stickForce = 100f;
    [SerializeField] private LayerMask targetLayers = -1;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip hitSound;

    private Rigidbody arrowRigidbody;
    private XRGrabInteractable grabInteractable;
    private bool isNocked = false;
    private bool isFired = false;
    private VRBow currentBow;
    private AudioSource audioSource;

    private void Awake()
    {
        arrowRigidbody = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Set up for grabbing with proper physics
        arrowRigidbody.useGravity = true;
        arrowRigidbody.isKinematic = false;
        arrowRigidbody.mass = 0.1f; // Light arrow
        arrowRigidbody.drag = 0.5f; // Some air resistance

        // Setup grab events for proper physics handling
        grabInteractable.selectEntered.AddListener(OnArrowGrabbed);
        grabInteractable.selectExited.AddListener(OnArrowReleased);
    }

    public void NockToBow(VRBow bow, Transform nockPoint)
    {
        if (isNocked || isFired) return;

        currentBow = bow;
        isNocked = true;

        // Disable physics while nocked
        arrowRigidbody.isKinematic = true;
        arrowRigidbody.useGravity = false;

        // Attach to bow
        transform.SetParent(nockPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Disable grabbing while nocked
        grabInteractable.enabled = false;

        Debug.Log("Arrow nocked to bow");
    }

    public void FireArrow(Vector3 shootDirection, float pullStrength)
    {
        if (!isNocked || isFired) return;

        isFired = true;
        isNocked = false;

        // Detach from bow
        transform.SetParent(null);

        // Enable physics
        arrowRigidbody.isKinematic = false;
        arrowRigidbody.useGravity = true;

        // Calculate velocity based on pull strength
        float actualSpeed = flightSpeed * Mathf.Clamp01(pullStrength);
        Vector3 velocity = shootDirection.normalized * actualSpeed;

        // Apply velocity
        arrowRigidbody.velocity = velocity;

        // Point arrow in flight direction
        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity);
        }

        // Play shoot sound
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        Debug.Log($"Arrow fired with speed: {actualSpeed}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isFired) return; // Only stick after being fired

        // Check if we hit a valid target
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            StickToTarget(collision);
        }
    }

    private void StickToTarget(Collision collision)
    {
        // Stop the arrow
        arrowRigidbody.velocity = Vector3.zero;
        arrowRigidbody.angularVelocity = Vector3.zero;
        arrowRigidbody.isKinematic = true;

        // Stick to the target
        transform.SetParent(collision.transform);

        // Play hit sound
        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Check for target script
        var target = collision.gameObject.GetComponent<ArcheryTarget>();
        if (target != null)
        {
            target.OnArrowHit(this);
        }

        Debug.Log($"Arrow stuck to: {collision.gameObject.name}");
    }

    public void ResetArrow()
    {
        // Reset arrow to grabbable state
        isNocked = false;
        isFired = false;
        currentBow = null;

        transform.SetParent(null);
        arrowRigidbody.isKinematic = false;
        arrowRigidbody.useGravity = true;
        arrowRigidbody.velocity = Vector3.zero;

        grabInteractable.enabled = true;
    }

    private void OnArrowGrabbed(SelectEnterEventArgs args)
    {
        // Disable physics while being held
        arrowRigidbody.isKinematic = true;
        arrowRigidbody.useGravity = false;
    }

    private void OnArrowReleased(SelectExitEventArgs args)
    {
        // Only enable physics if arrow is not nocked to bow
        if (!isNocked)
        {
            arrowRigidbody.isKinematic = false;
            arrowRigidbody.useGravity = true;
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnArrowGrabbed);
            grabInteractable.selectExited.RemoveListener(OnArrowReleased);
        }
    }

    // Public properties for external access
    public bool IsNocked => isNocked;
    public bool IsFired => isFired;
}