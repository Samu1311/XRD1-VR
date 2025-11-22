using System.Diagnostics;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string targetRoomName;

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("Something entered portal: " + other.name);

        if (other.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("Player detected, loading room: " + targetRoomName);
            RoomManager.Instance.LoadRoomByName(targetRoomName);
        }
    }
}