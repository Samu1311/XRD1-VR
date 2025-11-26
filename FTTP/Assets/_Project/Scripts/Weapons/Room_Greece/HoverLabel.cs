using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class HoverLabel : MonoBehaviour
{
    [Header("Label Settings")]
    [SerializeField] private string labelText = "Archery Training";
    [SerializeField] private GameObject labelCanvas;
    [SerializeField] private TextMeshProUGUI labelTextComponent;

    [Header("Display Options")]
    [SerializeField] private bool hideWhenGrabbed = true;
    [SerializeField] private float hoverDistance = 2f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;
    private Transform playerCamera;
    private bool isGrabbed = false;

    private void Start()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        playerCamera = Camera.main.transform;

        if (labelTextComponent != null)
            labelTextComponent.text = labelText;

        if (labelCanvas != null)
            labelCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);

            if (hideWhenGrabbed)
            {
                interactable.selectEntered.AddListener(OnGrabbed);
                interactable.selectExited.AddListener(OnReleased);
            }
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEnter);
            interactable.hoverExited.RemoveListener(OnHoverExit);

            if (hideWhenGrabbed)
            {
                interactable.selectEntered.RemoveListener(OnGrabbed);
                interactable.selectExited.RemoveListener(OnReleased);
            }
        }
    }

    private void Update()
    {
        if (labelCanvas != null && labelCanvas.activeSelf && playerCamera != null)
        {
            // Make label face the player
            labelCanvas.transform.LookAt(playerCamera);
            labelCanvas.transform.Rotate(0, 180, 0);

            // Check distance
            float distance = Vector3.Distance(transform.position, playerCamera.position);
            if (distance > hoverDistance)
            {
                labelCanvas.SetActive(false);
            }
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (!isGrabbed && labelCanvas != null)
        {
            labelCanvas.SetActive(true);
        }
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (labelCanvas != null)
        {
            labelCanvas.SetActive(false);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        if (labelCanvas != null)
            labelCanvas.SetActive(false);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }
}
