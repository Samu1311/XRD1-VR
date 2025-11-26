using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class VRBow : MonoBehaviour
{
    [Header("Bow Configuration")]
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Transform stringStartPoint;
    [SerializeField] private Transform stringEndPoint;
    [SerializeField] private Transform stringPullPoint;
    [SerializeField] private float maxPullDistance = 0.5f;
    [SerializeField] private float arrowForceMultiplier = 1000f;

    [Header("Arrow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowLifetime = 10f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stringTensionSound;
    [SerializeField] private AudioClip releaseSound;
    [SerializeField] private AudioClip nockArrowSound;
    [SerializeField, Range(0f, 1f)] private float tensionVolume = 0.3f;

    [Header("Visuals")]
    [SerializeField] private LineRenderer stringRenderer;
    [SerializeField] private float stringWidth = 0.01f;

    private XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor pullHandInteractor;
    private Arrow currentArrow;
    private float currentPull;
    private bool isNocked;
    private Vector3 originalStringPullPosition;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.loop = false;

        SetupStringRenderer();
        originalStringPullPosition = stringPullPoint.localPosition;
    }
    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnBowGrabbed);
        grabInteractable.selectExited.AddListener(OnBowReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnBowGrabbed);
        grabInteractable.selectExited.RemoveListener(OnBowReleased);
    }

    private void Update()
    {
        if (grabInteractable.isSelected && pullHandInteractor != null)
        {
            UpdateBowPull();
        }

        UpdateStringVisuals();
    }

    private void OnBowGrabbed(SelectEnterEventArgs args)
    {
        if (!isNocked && currentArrow == null)
        {
            SpawnArrow();
        }
    }

    private void OnBowReleased(SelectExitEventArgs args)
    {
        // Optionally clean up arrow if not nocked properly
    }

    public void OnStringGrabbed(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
    {
        if (!grabInteractable.isSelected) return;

        pullHandInteractor = interactor;

        if (currentArrow != null && !isNocked)
        {
            isNocked = true;
            PlaySound(nockArrowSound);
        }
    }

    public void OnStringReleased()
    {
        if (isNocked && currentArrow != null)
        {
            ReleaseArrow();
        }

        pullHandInteractor = null;
        currentPull = 0f;
        stringPullPoint.localPosition = originalStringPullPosition;
    }

    private void UpdateBowPull()
    {
        if (pullHandInteractor == null || !isNocked) return;

        Vector3 pullPosition = pullHandInteractor.transform.position;
        Vector3 bowForward = transform.forward;
        Vector3 arrowDirection = arrowSpawnPoint.position - pullPosition;

        // Calculate pull along bow's forward axis
        float pullAmount = Vector3.Dot(arrowDirection, bowForward);
        pullAmount = Mathf.Clamp(pullAmount, 0f, maxPullDistance);

        currentPull = pullAmount / maxPullDistance;

        // Update string pull point position
        Vector3 pullOffset = -bowForward * pullAmount;
        stringPullPoint.position = arrowSpawnPoint.position + pullOffset;

        // Update arrow position
        if (currentArrow != null)
        {
            currentArrow.transform.position = stringPullPoint.position;
            currentArrow.transform.rotation = Quaternion.LookRotation(bowForward);
        }

        // Tension sound feedback
        if (stringTensionSound != null && currentPull > 0.1f && !audioSource.isPlaying)
        {
            audioSource.pitch = 0.8f + currentPull * 0.4f;
            audioSource.PlayOneShot(stringTensionSound, tensionVolume * currentPull);
        }
    }

    private void ReleaseArrow()
    {
        if (currentArrow == null) return;

        float force = currentPull * arrowForceMultiplier;
        currentArrow.Release(transform.forward, force);

        PlaySound(releaseSound);

        currentArrow = null;
        isNocked = false;

        // Spawn new arrow after brief delay
        Invoke(nameof(SpawnArrow), 0.3f);
    }

    private void SpawnArrow()
    {
        if (arrowPrefab == null || currentArrow != null) return;

        GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        currentArrow = arrowObj.GetComponent<Arrow>();

        if (currentArrow != null)
        {
            currentArrow.Initialize(arrowLifetime);
        }
    }

    private void SetupStringRenderer()
    {
        if (stringRenderer == null)
        {
            GameObject stringObj = new GameObject("BowString");
            stringObj.transform.SetParent(transform);
            stringRenderer = stringObj.AddComponent<LineRenderer>();
        }

        stringRenderer.positionCount = 3;
        stringRenderer.startWidth = stringWidth;
        stringRenderer.endWidth = stringWidth;
        stringRenderer.material = new Material(Shader.Find("Sprites/Default"));
        stringRenderer.startColor = Color.white;
        stringRenderer.endColor = Color.white;
    }

    private void UpdateStringVisuals()
    {
        if (stringRenderer == null) return;

        stringRenderer.SetPosition(0, stringStartPoint.position);
        stringRenderer.SetPosition(1, stringPullPoint.position);
        stringRenderer.SetPosition(2, stringEndPoint.position);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmos()
    {
        if (arrowSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(arrowSpawnPoint.position, 0.02f);
            Gizmos.DrawRay(arrowSpawnPoint.position, transform.forward * 0.3f);
        }
    }
}
