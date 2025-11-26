using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class BowStringGrabbable : MonoBehaviour
{
    [SerializeField] private VRBow bowController;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Auto-configure XR interaction setup
        if (grabInteractable.colliders.Count == 0)
        {
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                grabInteractable.colliders.Add(collider);
            }
        }

        // Auto-assign interaction manager if not set
        if (grabInteractable.interactionManager == null)
        {
            grabInteractable.interactionManager = FindObjectOfType<XRInteractionManager>();
        }

        // Configure the grab interactable for string pulling
        grabInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = false;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnStringGrabbed);
        grabInteractable.selectExited.AddListener(OnStringReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnStringGrabbed);
        grabInteractable.selectExited.RemoveListener(OnStringReleased);
    }

    private void OnStringGrabbed(SelectEnterEventArgs args)
    {
        if (bowController != null)
        {
            bowController.OnStringGrabbed(args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor);
        }
    }

    private void OnStringReleased(SelectExitEventArgs args)
    {
        if (bowController != null)
        {
            bowController.OnStringReleased();
        }
    }
}
