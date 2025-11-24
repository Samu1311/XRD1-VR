using UnityEngine;

public class Portal : MonoBehaviour
{
    public string targetRoomName;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Start room transition
        RoomManager.Instance.LoadRoomByName(targetRoomName);
    }
}
