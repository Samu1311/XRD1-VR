using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Simple Oracle interaction fix - attach to Oracle GameObject
/// This will force proper setup and bypass common issues
/// </summary>
public class OracleQuickFix : MonoBehaviour
{
    private void Awake()
    {
        // Force Oracle to Default layer
        gameObject.layer = 0;

        // Remove any conflicting XRGrabInteractable
        var grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            Debug.Log("Removing conflicting XRGrabInteractable from Oracle");
            DestroyImmediate(grabInteractable);
        }

        // Ensure XRSimpleInteractable exists and is configured
        var simpleInteractable = GetComponent<XRSimpleInteractable>();
        if (simpleInteractable == null)
        {
            simpleInteractable = gameObject.AddComponent<XRSimpleInteractable>();
            Debug.Log("Added XRSimpleInteractable to Oracle");
        }

        // Force interaction layers to default
        simpleInteractable.interactionLayers = InteractionLayerMask.GetMask("Default");

        // Ensure proper collider
        var collider = GetComponent<Collider>();
        if (collider == null)
        {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(3f, 4f, 3f); // Large interaction area
            boxCollider.center = new Vector3(0f, 2f, 0f); // Center at body height
            Debug.Log("Added large Box Collider to Oracle");
        }
        else
        {
            collider.isTrigger = true;
            if (collider is BoxCollider box)
            {
                box.size = new Vector3(3f, 4f, 3f); // Make it larger
                box.center = new Vector3(0f, 2f, 0f);
                Debug.Log("Resized Oracle Box Collider");
            }
        }
    }
}