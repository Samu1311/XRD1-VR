using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class XRInitializer : MonoBehaviour
{
    [SerializeField] private bool autoInitialize = true;

    private void Start()
    {
        if (autoInitialize)
        {
            StartCoroutine(InitializeXR());
        }
    }

    public IEnumerator InitializeXR()
    {
        Debug.Log("Initializing XR...");

        // Check if XR is already initialized
        if (XRGeneralSettings.Instance?.Manager?.activeLoader != null)
        {
            Debug.Log("XR is already initialized!");
            yield break;
        }

        // Initialize XR
        if (XRGeneralSettings.Instance?.Manager != null)
        {
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Failed to initialize XR!");
            }
            else
            {
                Debug.Log("XR initialized successfully!");

                // Start XR
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                Debug.Log("XR subsystems started!");
            }
        }
        else
        {
            Debug.LogError("XRGeneralSettings.Instance.Manager is null!");
        }

        // Wait a moment for XR to fully initialize
        yield return new WaitForSeconds(1f);

        // Check XR status
        CheckXRStatus();
    }

    private void CheckXRStatus()
    {
        Debug.Log("=== XR STATUS CHECK ===");
        Debug.Log($"XR Device Active: {UnityEngine.XR.XRSettings.enabled}");
        Debug.Log($"XR Device Name: {UnityEngine.XR.XRSettings.loadedDeviceName}");

        var activeLoader = XRGeneralSettings.Instance?.Manager?.activeLoader;
        Debug.Log($"Active XR Loader: {(activeLoader != null ? activeLoader.name : "None")}");

        // Check for XR Interaction Manager
        var interactionManager = FindObjectOfType<XRInteractionManager>();
        Debug.Log($"XR Interaction Manager: {(interactionManager != null ? "Found" : "Not Found")}");

        if (interactionManager != null)
        {
            Debug.Log($"XR Interaction Manager is active: {interactionManager.enabled}");
        }
    }

    public void ManualInitialize()
    {
        StartCoroutine(InitializeXR());
    }

    private void OnDestroy()
    {
        // Clean up XR on destroy
        if (XRGeneralSettings.Instance?.Manager?.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }
}