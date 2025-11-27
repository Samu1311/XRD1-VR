using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class GreekVase : MonoBehaviour
{
    [Header("Myth Selection")]
    [SerializeField] private GreekMythDatabase mythDatabase;
    [SerializeField] private int mythIndex = 0;

    [Header("Display Settings")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI mythTextUI;
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 2.5f, 0);

    private XRSimpleInteractable interactable;
    private GameObject activeTextPanel;
    private float hideTimer;
    private GreeceRoomController roomController;

    private void Awake()
    {
        // Find the room controller
        roomController = FindObjectOfType<GreeceRoomController>();

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnVaseClicked);
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnVaseClicked);
        }
    }

    private void Update()
    {
        if (activeTextPanel != null && hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
            {
                HideText();
            }
        }
    }

    private void OnVaseClicked(SelectEnterEventArgs args)
    {
        // Notify room controller of vase interaction
        if (roomController != null)
        {
            roomController.OnVaseInteraction();
        }

        ShowText();
    }

    private void ShowText()
    {
        string mythText = "No myth database assigned.";

        if (mythDatabase != null)
        {
            mythText = mythDatabase.GetMythText(mythIndex);
            Debug.Log($"Vase {gameObject.name}: Got myth text: {mythText}");
        }
        else
        {
            Debug.LogWarning($"Vase {gameObject.name}: No myth database assigned!");
        }

        if (textPanel != null)
        {
            if (activeTextPanel == null)
            {
                // Calculate position above the vase
                Vector3 spawnPosition = CalculateTextPosition();

                // Calculate rotation to face camera
                Quaternion faceRotation = CalculateCameraFacingRotation();

                activeTextPanel = Instantiate(textPanel, spawnPosition, faceRotation);
                activeTextPanel.transform.SetParent(transform);
            }
            else
            {
                // Update position and rotation if panel already exists
                activeTextPanel.transform.position = CalculateTextPosition();
                activeTextPanel.transform.rotation = CalculateCameraFacingRotation();
            }

            // Force set the text on ALL possible text components to override any defaults

            // First try MysticTextPanel
            var mysticPanel = activeTextPanel.GetComponent<MysticTextPanel>();
            if (mysticPanel != null)
            {
                Debug.Log($"Setting text via MysticTextPanel: {mythText}");
                mysticPanel.SetText(mythText);
            }

            // Also set on direct TextMeshPro if assigned
            if (mythTextUI != null)
            {
                Debug.Log($"Setting text via mythTextUI: {mythText}");
                mythTextUI.text = mythText;
            }

            // Find and set ALL TextMeshPro components in children
            var allTextComponents = activeTextPanel.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var textComponent in allTextComponents)
            {
                Debug.Log($"Setting text on component {textComponent.name}: {mythText}");
                textComponent.text = mythText;
            }

            // Text panels now keep their manually set size

            activeTextPanel.SetActive(true);
            hideTimer = displayDuration;
        }
        else
        {
            Debug.Log($"Greek Vase {gameObject.name}: {mythText}");
        }
    }

    private void HideText()
    {
        if (activeTextPanel != null)
        {
            activeTextPanel.SetActive(false);
        }
    }

    private Vector3 CalculateTextPosition()
    {
        // Get the vase's renderer bounds to calculate its height
        Renderer vaseRenderer = GetComponent<Renderer>();
        float vaseHeight = 1f; // Default fallback height

        if (vaseRenderer != null)
        {
            vaseHeight = vaseRenderer.bounds.size.y;
        }

        // Position the panel above the vase with some extra margin
        Vector3 position = transform.position;
        position.y += vaseHeight + 0.5f; // Add vase height plus extra margin

        return position;
    }

    private Quaternion CalculateCameraFacingRotation()
    {
        // Find the player camera
        Camera playerCamera = Camera.main;

        // Try to find XR camera if main camera is not found
        if (playerCamera == null)
        {
            var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin != null)
            {
                playerCamera = xrOrigin.Camera;
            }
        }

        if (playerCamera != null)
        {
            // Calculate direction from camera to panel (FIXED: was backwards)
            Vector3 directionFromCamera = transform.position - playerCamera.transform.position;
            directionFromCamera.y = 0; // Keep panel upright, only rotate horizontally

            // Return rotation that faces the camera
            return Quaternion.LookRotation(directionFromCamera);
        }

        // Fallback: face forward
        return Quaternion.identity;
    }

}
