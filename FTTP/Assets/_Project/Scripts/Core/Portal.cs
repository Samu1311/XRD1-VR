using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Name of scene in Build Settings")]
    public string targetRoomName;

    [Header("Message shown on the loading screen")]
    [TextArea]
    public string loadingMessage = "Traveling...";

    private void OnTriggerEnter(Collider other)
    {
        // Check only for the XR Rig
        if (!other.CompareTag("Player"))
            return;

        Debug.Log($"Player entered portal - Loading room {targetRoomName}");

        // Show loading screen with custom text
        LoadingScreenManager.Instance.Show(loadingMessage);

        // Trigger room transition
        RoomManager.Instance.LoadRoomByName(targetRoomName);
    }
}
