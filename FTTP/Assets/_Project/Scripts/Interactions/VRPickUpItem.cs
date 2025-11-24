using UnityEngine;

public class VRPickUpItem : MonoBehaviour
{
    public string itemName;
    public GameObject collectedVersion;
    public SphinxClueManager clueManager;

    private bool handNear = false;

    void Update()
    {
        if (handNear && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            CollectItem();
        }
    }

    void OnTriggerEnter(Collider other)
    {
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
        gameObject.SetActive(false);
        collectedVersion.SetActive(true);
        clueManager.ItemCollected(itemName);
    }
}
