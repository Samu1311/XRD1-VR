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

    [Header("Effects")]
    [SerializeField] private AudioClip magicSoundClip;

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

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    private void OnVaseClicked(SelectEnterEventArgs args)
    {
        // Notify room controller of vase interaction
        if (roomController != null)
        {
            roomController.OnVaseInteraction();
        }
        PlayAudioClip(magicSoundClip);
        ShowText();
    }

    private void ShowText()
    {
        string mythText = "No myth database assigned.";

        if (mythDatabase != null)
        {
            mythText = mythDatabase.GetMythText(mythIndex);
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
                mysticPanel.SetText(mythText);
            }

            if (mythTextUI != null)
            {
                mythTextUI.text = mythText;
            }

            var allTextComponents = activeTextPanel.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var textComponent in allTextComponents)
            {
                textComponent.text = mythText;
            }

            // Text panels keep their manually set size

            activeTextPanel.SetActive(true);
            hideTimer = displayDuration;
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
        float vaseHeight = 2f; // Default fallback height

        if (vaseRenderer != null)
        {
            vaseHeight = vaseRenderer.bounds.size.y;
        }

        // Position the panel above the vase with some extra margin
        Vector3 position = transform.position;
        position.y += vaseHeight + 2f; // Vase height plus extra margin

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
            Vector3 directionFromCamera = transform.position - playerCamera.transform.position;
            directionFromCamera.y = 0; // Keep panel upright, only rotate horizontally

            // Return rotation that faces the camera
            return Quaternion.LookRotation(directionFromCamera);
        }

        // Fallback: face forward
        return Quaternion.identity;
    }

}
