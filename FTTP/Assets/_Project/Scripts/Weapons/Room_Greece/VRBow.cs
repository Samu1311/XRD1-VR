using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(XRGrabInteractable))]
public class VRBow : MonoBehaviour
{
    [Header("Bow Configuration")]
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Transform nockPoint;
    [SerializeField] private Transform stringStartPoint;
    [SerializeField] private Transform stringEndPoint;
    [SerializeField] private Transform stringPullPoint;
    [SerializeField] private float maxPullDistance = 0.5f;
    [SerializeField] private float arrowForce = 1000f;

    [Header("Arrow Settings")]
    // Arrows are placed manually in scene - no spawning

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stringTensionSound;
    [SerializeField] private AudioClip releaseSound;
    [SerializeField] private AudioClip nockArrowSound;
    [SerializeField] private AudioClip shootArrowSound;
    [SerializeField, Range(0f, 1f)] private float tensionVolume = 0.3f;

    [Header("Visuals")]
    [SerializeField] private LineRenderer stringRenderer;
    [SerializeField] private float stringWidth = 0.01f;

    [Header("Instruction UI")]
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private float instructionDisplayTime = 3f;

    private XRGrabInteractable grabInteractable;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor pullHandInteractor;
    private GameObject currentArrow;
    private GameObject nearbyArrow;
    private float currentPull;
    private bool isNocked;
    private bool isArrowNearBow;
    private Vector3 originalStringPullPosition;
    private GreeceRoomController roomController;
    private bool hasNotifiedInteraction = false;

    private void Awake()
    {
        // Find the room controller
        roomController = FindObjectOfType<GreeceRoomController>();

        grabInteractable = GetComponent<XRGrabInteractable>();

        // Ensure proper Rigidbody setup
        var rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        rigidbody.mass = 0.5f; // Light but not too light
        rigidbody.drag = 2f; // Prevent wild swinging
        rigidbody.angularDrag = 5f; // Prevent spinning
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;

        // Ensure collider is properly sized
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.material = null; // Remove any physics material that might cause issues
        }

        // Configure bow for realistic VR archery grip
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;
        grabInteractable.retainTransformParent = false;

        // Set attach transform for proper bow grip (vertical hold)
        // The bow should be held with the grip vertical, string facing away from player
        var attachTransform = new GameObject("BowGrip");
        attachTransform.transform.SetParent(transform);
        attachTransform.transform.localPosition = Vector3.zero;
        attachTransform.transform.localRotation = Quaternion.identity;
        grabInteractable.attachTransform = attachTransform.transform;

        // Smooth movement settings for natural feel
        grabInteractable.smoothPosition = true;
        grabInteractable.smoothRotation = true;
        grabInteractable.tightenPosition = 0.5f;
        grabInteractable.smoothPositionAmount = 8f;
        grabInteractable.smoothRotationAmount = 8f;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.loop = false;

        SetupStringRenderer();
        originalStringPullPosition = stringPullPoint.localPosition;

        // Create string interactable for second hand
        CreateStringInteractable();

        // Hide instruction panel initially
        if (instructionPanel != null)
            instructionPanel.SetActive(false);
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
        CheckForNearbyArrows();

        // Handle input for nocking when arrow is nearby
        if (isArrowNearBow && !isNocked && nearbyArrow != null)
        {
            HandleNockingInput();
        }
    }

    private void OnBowGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Bow grabbed!");

        // Let XR system handle physics while grabbed - don't override

        // Orient bow to face user properly
        // Bow should be vertical with string facing the player for proper archery stance
        var interactor = args.interactorObject.transform;

        // Calculate proper bow orientation - string should face towards player
        Vector3 toPlayer = (interactor.position - transform.position).normalized;
        Vector3 bowUp = Vector3.up;
        Vector3 bowForward = toPlayer; // Bow faces the player

        // Apply orientation correction
        Quaternion targetRotation = Quaternion.LookRotation(bowForward, bowUp);
        StartCoroutine(SmoothOrientBow(targetRotation));

        // Notify room controller of bow interaction (first time only)
        if (!hasNotifiedInteraction && roomController != null)
        {
            roomController.OnBowInteraction();
            hasNotifiedInteraction = true;
        }

        // Auto-nocking is now disabled - arrows must be manually nocked with trigger press
        // This allows for the new two-step instruction system
    }

    private void OnBowReleased(SelectExitEventArgs args)
    {
        Debug.Log("Bow released!");
        if (pullHandInteractor != null)
        {
            OnStringReleased();
        }

        // Physics are already properly configured in Awake()

        // Hide instruction panel when bow is released
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        // Reset states
        isArrowNearBow = false;
        nearbyArrow = null;
    }

    public void OnStringGrabbed(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor)
    {
        if (!grabInteractable.isSelected) return;

        Debug.Log("String grabbed!");
        pullHandInteractor = interactor;

        // No auto-nocking - player must manually bring arrow to bow first

        if (currentArrow != null && !isNocked)
        {
            var vrArrow = currentArrow.GetComponent<VRArrow>();
            if (vrArrow != null)
            {
                vrArrow.NockToBow(this, stringPullPoint);
                isNocked = true;
                PlaySound(nockArrowSound);
            }
        }
    }

    public void OnStringReleased()
    {
        if (pullHandInteractor == null) return;

        Debug.Log($"String released! Nocked: {isNocked}, Arrow exists: {currentArrow != null}");

        if (isNocked && currentArrow != null)
        {
            var vrArrow = currentArrow.GetComponent<VRArrow>();
            if (vrArrow != null)
            {
                Vector3 shootDirection = (arrowSpawnPoint.position - stringPullPoint.position).normalized;
                vrArrow.FireArrow(shootDirection, currentPull);
                PlaySound(releaseSound);

                currentArrow = null;
                isNocked = false;
            }
        }

        pullHandInteractor = null;
        currentPull = 0f;
        stringPullPoint.localPosition = originalStringPullPosition;
    }

    private void UpdateBowPull()
    {
        if (pullHandInteractor == null || !isNocked) return;

        Vector3 pullHandPosition = pullHandInteractor.transform.position;
        Vector3 bowHandPosition = grabInteractable.GetAttachTransform(null).position;

        // Calculate the pull direction from bow grip to pulling hand
        Vector3 pullDirection = (pullHandPosition - bowHandPosition).normalized;
        Vector3 bowForward = transform.forward;

        // Calculate pull distance (how far back the string is pulled)
        Vector3 stringCenter = (stringStartPoint.position + stringEndPoint.position) * 0.5f;
        Vector3 pullVector = pullHandPosition - stringCenter;

        // Project pull onto the bow's backward direction (opposite of forward)
        float pullAmount = Vector3.Dot(pullVector, -bowForward);
        pullAmount = Mathf.Clamp(pullAmount, 0f, maxPullDistance);

        currentPull = pullAmount / maxPullDistance;

        // Update string pull point - move it back from the bow center
        Vector3 restPosition = (stringStartPoint.position + stringEndPoint.position) * 0.5f;
        stringPullPoint.position = restPosition + (-bowForward * pullAmount);

        // Update arrow position and rotation to match string pull
        if (currentArrow != null)
        {
            var vrArrow = currentArrow.GetComponent<VRArrow>();
            if (vrArrow != null && vrArrow.IsNocked)
            {
                // Position arrow at the string pull point
                currentArrow.transform.position = stringPullPoint.position;
                // Point arrow forward along bow direction
                currentArrow.transform.rotation = Quaternion.LookRotation(bowForward, transform.up);
            }
        }

        // Tension sound feedback with realistic curve
        if (stringTensionSound != null && currentPull > 0.15f && !audioSource.isPlaying)
        {
            float tensionIntensity = currentPull * currentPull; // Quadratic for more realistic tension buildup
            audioSource.pitch = 0.9f + tensionIntensity * 0.3f;
            audioSource.PlayOneShot(stringTensionSound, tensionVolume * tensionIntensity);
        }
    }

    // Manual nocking system - no spawning
    // Arrows are placed manually in scene and picked up by player

    private void SetupStringRenderer()
    {
        if (stringRenderer == null)
        {
            GameObject stringObj = new GameObject("BowString");
            stringObj.transform.SetParent(transform);
            stringRenderer = stringObj.AddComponent<LineRenderer>();

            // Prevent string from interfering with physics
            stringObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        stringRenderer.positionCount = 3;
        stringRenderer.startWidth = stringWidth;
        stringRenderer.endWidth = stringWidth;
        stringRenderer.material = new Material(Shader.Find("Sprites/Default"));
        stringRenderer.startColor = new Color(0.6f, 0.4f, 0.2f); // Brown color
        stringRenderer.endColor = new Color(0.6f, 0.4f, 0.2f);

        // Make string completely non-physical
        stringRenderer.useWorldSpace = true;
        stringRenderer.generateLightingData = false;

        // Put string on UI layer to avoid all physics
        stringRenderer.gameObject.layer = LayerMask.NameToLayer("UI");

        // Remove any colliders from string
        var colliders = stringRenderer.GetComponents<Collider>();
        foreach (var col in colliders)
        {
            DestroyImmediate(col);
        }

        // Remove rigidbody if it exists
        var rb = stringRenderer.GetComponent<Rigidbody>();
        if (rb != null)
        {
            DestroyImmediate(rb);
        }
    }

    private void UpdateStringVisuals()
    {
        if (stringRenderer == null) return;

        // Create a more realistic bow string curve with multiple points
        int stringPoints = 5;
        stringRenderer.positionCount = stringPoints;

        Vector3 startPos = stringStartPoint.position;
        Vector3 endPos = stringEndPoint.position;
        Vector3 pullPos = stringPullPoint.position;

        // Calculate string positions with realistic curve
        for (int i = 0; i < stringPoints; i++)
        {
            float t = (float)i / (stringPoints - 1);
            Vector3 position;

            if (t <= 0.5f)
            {
                // First half: from start to pull point
                float localT = t * 2f;
                position = Vector3.Lerp(startPos, pullPos, localT);
            }
            else
            {
                // Second half: from pull point to end
                float localT = (t - 0.5f) * 2f;
                position = Vector3.Lerp(pullPos, endPos, localT);
            }

            stringRenderer.SetPosition(i, position);
        }

        // Adjust string thickness based on tension
        float tension = currentPull * 0.5f + 0.5f; // 0.5 to 1.0 range
        stringRenderer.startWidth = stringWidth * tension;
        stringRenderer.endWidth = stringWidth * tension;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Create a separate interactable for the bowstring
    private void CreateStringInteractable()
    {
        // Create string grab point GameObject
        GameObject stringGrabPoint = new GameObject("StringGrabPoint");
        stringGrabPoint.transform.SetParent(transform);
        stringGrabPoint.transform.position = stringPullPoint.position;

        // Put on UI layer to avoid physics interference
        stringGrabPoint.layer = LayerMask.NameToLayer("Default");

        // Add non-physical collider for string grabbing
        SphereCollider stringCollider = stringGrabPoint.AddComponent<SphereCollider>();
        stringCollider.radius = 0.03f; // Smaller radius to avoid pushing
        stringCollider.isTrigger = true;

        // Ensure no rigidbody on string grab point
        var stringRb = stringGrabPoint.GetComponent<Rigidbody>();
        if (stringRb != null)
        {
            DestroyImmediate(stringRb);
        }

        // Add XR grab interactable for string
        XRGrabInteractable stringInteractable = stringGrabPoint.AddComponent<XRGrabInteractable>();
        stringInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        stringInteractable.trackPosition = false; // Don't move the string point itself
        stringInteractable.trackRotation = false;

        // Connect string events
        stringInteractable.selectEntered.AddListener(OnStringSelectEntered);
        stringInteractable.selectExited.AddListener(OnStringSelectExited);
    }

    // Public methods for string interactable component
    public void OnStringSelectEntered(SelectEnterEventArgs args)
    {
        OnStringGrabbed(args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor);
    }

    public void OnStringSelectExited(SelectExitEventArgs args)
    {
        OnStringReleased();
    }
    private void CheckForNearbyArrows()
    {
        if (isNocked) return; // Already have an arrow nocked

        Transform searchPoint = nockPoint != null ? nockPoint : arrowSpawnPoint;
        float nockDistance = 0.15f; // Close distance for nocking

        Collider[] nearbyColliders = Physics.OverlapSphere(searchPoint.position, nockDistance);

        VRArrow foundArrow = null;
        foreach (var collider in nearbyColliders)
        {
            VRArrow arrow = collider.GetComponent<VRArrow>();
            if (arrow != null && !arrow.IsNocked && !arrow.IsFired)
            {
                // Check if arrow was just released (not being held anymore)
                XRGrabInteractable arrowGrab = arrow.GetComponent<XRGrabInteractable>();
                if (arrowGrab != null && !arrowGrab.isSelected)
                {
                    // Arrow is close to nock point and not being held - auto-nock it
                    foundArrow = arrow;
                    break;
                }
            }
        }

        // Arrow detected near bow and released - nock it automatically
        if (foundArrow != null)
        {
            NockArrowToString(foundArrow.gameObject);
            return;
        }

        // Check if there's an arrow being held near the bow (show instruction)
        Collider[] heldArrowColliders = Physics.OverlapSphere(searchPoint.position, 0.3f);
        bool heldArrowNearby = false;

        foreach (var collider in heldArrowColliders)
        {
            VRArrow arrow = collider.GetComponent<VRArrow>();
            if (arrow != null && !arrow.IsNocked && !arrow.IsFired)
            {
                XRGrabInteractable arrowGrab = arrow.GetComponent<XRGrabInteractable>();
                if (arrowGrab != null && arrowGrab.isSelected)
                {
                    heldArrowNearby = true;
                    nearbyArrow = arrow.gameObject;
                    break;
                }
            }
        }

        // Show/hide nocking instruction based on held arrow proximity
        if (heldArrowNearby && !isArrowNearBow)
        {
            isArrowNearBow = true;
            ShowNockingInstruction();
        }
        else if (!heldArrowNearby && isArrowNearBow)
        {
            nearbyArrow = null;
            isArrowNearBow = false;
            HideInstruction();
        }
    }

    /// <summary>
    /// Nocks an arrow to the bow when player brings it close and releases
    /// </summary>
    private void NockArrowToString(GameObject arrow)
    {
        if (isNocked || arrow == null) return;

        var vrArrow = arrow.GetComponent<VRArrow>();
        if (vrArrow == null) return;

        // Position arrow at nock point
        Transform nockPos = nockPoint != null ? nockPoint : stringPullPoint;
        arrow.transform.position = nockPos.position;
        arrow.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);

        // Disable arrow physics while nocked
        var arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.isKinematic = true;
        }

        // Disable arrow grabbing while nocked
        var arrowGrab = arrow.GetComponent<XRGrabInteractable>();
        if (arrowGrab != null)
        {
            arrowGrab.enabled = false;
        }

        // Set arrow as nocked
        vrArrow.NockToBow(this, nockPos);
        currentArrow = arrow;
        isNocked = true;
        isArrowNearBow = false;

        // Play nock sound
        PlaySound(nockArrowSound);

        Debug.Log("Arrow nocked to bow!");
        HideInstruction();
    }

    private bool IsArrowBeingHeld(VRArrow arrow)
    {
        // Check if the arrow is currently being grabbed by a player
        var grabInteractable = arrow.GetComponent<XRGrabInteractable>();
        return grabInteractable != null && grabInteractable.isSelected;
    }
    private void HandleNockingInput()
    {
        // Check for trigger press on either controller
        var leftController = GetControllerInput("Left");
        var rightController = GetControllerInput("Right");

        if (leftController || rightController)
        {
            NockArrow();
        }

        // Also check if the arrow was released (no longer being held)
        if (nearbyArrow != null)
        {
            var vrArrow = nearbyArrow.GetComponent<VRArrow>();
            if (vrArrow != null && !IsArrowBeingHeld(vrArrow))
            {
                // Arrow was released near the bow, auto-nock it
                StartCoroutine(DelayedNockArrow(0.1f)); // Small delay to ensure physics settle
            }
        }
    }

    private IEnumerator DelayedNockArrow(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (nearbyArrow != null && !isNocked)
        {
            NockArrow();
        }
    }
    private bool GetControllerInput(string hand)
    {
        // Try to get input from XR controllers
        var inputDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
            hand == "Left" ? UnityEngine.XR.InputDeviceCharacteristics.Left : UnityEngine.XR.InputDeviceCharacteristics.Right,
            inputDevices);

        foreach (var device in inputDevices)
        {
            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed)
            {
                return true;
            }
        }
        return false;
    }

    private void NockArrow()
    {
        if (nearbyArrow == null) return;

        var vrArrow = nearbyArrow.GetComponent<VRArrow>();
        if (vrArrow != null)
        {
            currentArrow = nearbyArrow;
            vrArrow.NockToBow(this, arrowSpawnPoint);
            isNocked = true;
            isArrowNearBow = false;
            nearbyArrow = null;

            ShowDrawingInstruction();

            // Notify room controller
            if (!hasNotifiedInteraction && roomController != null)
            {
                roomController.OnBowInteraction();
                hasNotifiedInteraction = true;
            }

            Debug.Log("Arrow nocked to bow");
        }
    }

    private void ShowNockingInstruction()
    {
        if (instructionPanel != null && instructionText != null)
        {
            instructionText.text = "ARROW READY TO NOCK!\n\nRelease arrow grip\nOR\npress TRIGGER\nto attach arrow to bow";
            instructionPanel.SetActive(true);
        }
    }
    private void ShowDrawingInstruction()
    {
        if (instructionPanel != null && instructionText != null)
        {
            instructionText.text = "ARROW NOCKED!\n\n1. Hold bow with one hand\n\n2. Grab string with other hand\n\n3. Pull back to draw\n\n4. Release to fire!";

            // Hide instruction after delay
            StartCoroutine(HideInstructionAfterDelay());
        }
    }
    private void HideInstruction()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }

    private void PositionInstructionPanel()
    {
        // Auto-positioning removed - use fixed panel positioning set in Unity Editor
        // This prevents the panel from covering the entire scene
    }
    private IEnumerator HideInstructionAfterDelay()
    {
        yield return new WaitForSeconds(instructionDisplayTime);
        HideInstruction();
    }

    /// <summary>
    /// Smoothly orients the bow to a natural archery position
    /// </summary>
    private IEnumerator SmoothOrientBow(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration && grabInteractable.isSelected)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // Smooth curve

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        if (grabInteractable.isSelected)
        {
            transform.rotation = targetRotation;
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

        if (nockPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(nockPoint.position, 0.03f);
        }

        // Show string points for setup
        if (stringStartPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(stringStartPoint.position, 0.02f);
        }

        if (stringEndPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(stringEndPoint.position, 0.02f);
        }

        if (stringPullPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(stringPullPoint.position, 0.025f);
        }
    }
}