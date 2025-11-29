using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class LeftHandOnlyGrab : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Subscribe to the selecting event
        grabInteractable.firstSelectEntered.AddListener(CheckInteractor);
    }

    private void CheckInteractor(SelectEnterEventArgs args)
    {
        // Only allow interactors whose name contains "LeftHand"
        if (!args.interactorObject.transform.name.Contains("LeftHand"))
        {
            // Force release immediately
            if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor &&
                grabInteractable is UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
            {
                grabInteractable.interactionManager?.SelectExit(interactor, interactable);
            }
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.firstSelectEntered.RemoveListener(CheckInteractor);
    }
}
