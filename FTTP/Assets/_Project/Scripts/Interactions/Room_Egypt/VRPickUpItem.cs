using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRPickUpItem : MonoBehaviour
{
    public string itemName;
    public GameObject collectedVersion;
    public SphinxClueManager clueManager;

    private bool handNear = false;
    private List<InputDevice> devices = new List<InputDevice>();

    void Update()
    {
        if (!handNear) return;

        // Get all connected XR devices
        InputDevices.GetDevices(devices);

        foreach (var device in devices)
        {
            // Check trigger press
            bool triggerPressed;
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
            {
                CollectItem();
                break; // prevent multiple pickups at once
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Make sure your XR controller has this tag
        if (other.CompareTag("PlayerHand"))
            handNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
            handNear = false;
    }

    void CollectItem()
    {
        if (collectedVersion != null)
            collectedVersion.SetActive(true);

        gameObject.SetActive(false);

        if (clueManager != null)
            clueManager.ItemCollected(itemName);
    }
}
