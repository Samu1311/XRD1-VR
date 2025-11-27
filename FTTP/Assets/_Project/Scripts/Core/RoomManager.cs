using System.Diagnostics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    private string _currentRoom;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadRoomByName(string roomName)
    {
        StartCoroutine(LoadRoomRoutine(roomName));
    }

    private System.Collections.IEnumerator LoadRoomRoutine(string roomName)
    {
        // --- REMOVE MAIN PORTAL BEFORE LOADING ---
        GameObject mainPortal = GameObject.FindWithTag("MainPortal");
        if (mainPortal != null)
        {
            Destroy(mainPortal);
        }

        // Load new room additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Unload previous room (but not Main)
        if (!string.IsNullOrEmpty(_currentRoom) && _currentRoom != "Main")
        {
            SceneManager.UnloadSceneAsync(_currentRoom);
        }

        _currentRoom = roomName;

        // Wait one frame so scene objects spawn
        yield return null;

        // Find player spawn point
        GameObject spawn = GameObject.FindWithTag("PlayerSpawn");
        if (spawn == null)
        {
            yield break;
        }

        // Find XR rig
        XROrigin rig = FindObjectOfType<XROrigin>();
        if (rig == null)
        {
            yield break;
        }

        // Teleport XR rig (Fixed to prevent scaling issues)
        // Store original scale to preserve it
        Vector3 originalScale = rig.transform.localScale;

        // Calculate offset between camera and rig
        Vector3 cameraOffset = rig.Camera.transform.position - rig.transform.position;

        // Set rig position accounting for camera offset
        rig.transform.position = spawn.transform.position - cameraOffset;
        rig.transform.rotation = spawn.transform.rotation;

        // Ensure scale is preserved
        rig.transform.localScale = originalScale;

    }
}
