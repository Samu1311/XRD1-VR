using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Debug script to diagnose Oracle interaction issues
/// Attach this to the Oracle GameObject to get detailed interaction diagnostics
/// </summary>
public class OracleDebug : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== ORACLE DEBUG START ===");

        // Check GameObject basics
        Debug.Log($"Oracle GameObject: {gameObject.name}");
        Debug.Log($"Oracle Layer: {gameObject.layer} ({LayerMask.LayerToName(gameObject.layer)})");
        Debug.Log($"Oracle Position: {transform.position}");
        Debug.Log($"Oracle Active: {gameObject.activeInHierarchy}");

        // Check Colliders
        var colliders = GetComponents<Collider>();
        Debug.Log($"Collider Count: {colliders.Length}");
        foreach (var col in colliders)
        {
            Debug.Log($"- Collider: {col.GetType().Name}, IsTrigger: {col.isTrigger}, Enabled: {col.enabled}");
            if (col is BoxCollider box)
            {
                Debug.Log($"  Box Size: {box.size}, Center: {box.center}");
            }
            if (col is SphereCollider sphere)
            {
                Debug.Log($"  Sphere Radius: {sphere.radius}, Center: {sphere.center}");
            }
        }

        // Check XR Components
        var xrSimple = GetComponent<XRSimpleInteractable>();
        var xrGrab = GetComponent<XRGrabInteractable>();

        Debug.Log($"XRSimpleInteractable: {(xrSimple != null ? "FOUND" : "MISSING")}");
        if (xrSimple != null)
        {
            Debug.Log($"- Enabled: {xrSimple.enabled}");
            Debug.Log($"- Interaction Layer Mask: {xrSimple.interactionLayers.value}");
            Debug.Log($"- Select Entered Events: {xrSimple.selectEntered.GetPersistentEventCount()}");
        }

        Debug.Log($"XRGrabInteractable: {(xrGrab != null ? "FOUND" : "MISSING")}");

        // Check Oracle script
        var oracle = GetComponent<OracleOfDelphi>();
        Debug.Log($"OracleOfDelphi Script: {(oracle != null ? "FOUND" : "MISSING")}");
        if (oracle != null)
        {
            Debug.Log($"- Oracle Script Enabled: {oracle.enabled}");
        }

        Debug.Log("=== ORACLE DEBUG END ===");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ORACLE DEBUG] Trigger Enter: {other.gameObject.name} (Layer: {other.gameObject.layer})");

        // Check if it's an XR controller
        var xrController = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
        if (xrController != null)
        {
            Debug.Log($"[ORACLE DEBUG] XR Controller detected: {xrController.GetType().Name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[ORACLE DEBUG] Trigger Exit: {other.gameObject.name}");
    }

    private void Update()
    {
        // Check for nearby XR controllers every few seconds
        if (Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
        {
            var controllers = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>();
            Debug.Log($"[ORACLE DEBUG] Found {controllers.Length} XR controllers in scene");

            foreach (var controller in controllers)
            {
                float distance = Vector3.Distance(transform.position, controller.transform.position);
                Debug.Log($"- Controller {controller.name}: Distance = {distance:F2}m, Layer = {controller.gameObject.layer}");
            }
        }
    }
}