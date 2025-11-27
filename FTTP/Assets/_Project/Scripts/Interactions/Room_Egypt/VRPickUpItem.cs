using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPickUpItem : MonoBehaviour
{
    public string itemName;
    public GameObject collectedVersion;
    public SphinxClueManager clueManager;
    public Collider tableTrigger; // Assign the table's trigger collider in inspector

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool hasBeenCollected = false;
    private bool isOnTable = false;

    private void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }
        grabInteractable.selectExited.AddListener(OnItemReleased);

        // Ensure collider exists
        var collider = GetComponent<Collider>();
        if (collider == null)
        {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = false;
            boxCollider.size = Vector3.one * 0.3f;
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnItemReleased);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tableTrigger != null && other == tableTrigger)
        {
            isOnTable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (tableTrigger != null && other == tableTrigger)
        {
            isOnTable = false;
        }
    }

    private void OnItemReleased(SelectExitEventArgs args)
    {
        if (hasBeenCollected) return;
        if (isOnTable)
        {
            Debug.Log($"VRPickUpItem: {itemName} released on table");
            CollectItem();
        }
    }

    private void CollectItem()
    {
        if (hasBeenCollected) return;
        hasBeenCollected = true;

        if (collectedVersion != null)
            collectedVersion.SetActive(true);

        gameObject.SetActive(false);

        if (clueManager != null)
            clueManager.ItemCollected(itemName);

        Debug.Log($"VRPickUpItem: Collected {itemName}");
    }

    [ContextMenu("Reset Item")]
    public void ResetItem()
    {
        hasBeenCollected = false;
        gameObject.SetActive(true);
        if (collectedVersion != null)
            collectedVersion.SetActive(false);
        isOnTable = false;
    }
}