using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class BowStringGrabbable : MonoBehaviour
{
    [SerializeField] private VRBow bowController;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Configure the grab interactable for string pulling
        grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Kinematic;
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
