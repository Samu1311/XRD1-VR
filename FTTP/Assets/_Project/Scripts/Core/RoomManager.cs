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
            Debug.Log("Main portal removed before loading new room.");
        }

        // --- LOAD NEW ROOM ADDITIVELY ---
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // --- UNLOAD PREVIOUS ROOM (BUT NOT MAIN) ---
        if (!string.IsNullOrEmpty(_currentRoom) && _currentRoom != "Main")
        {
            SceneManager.UnloadSceneAsync(_currentRoom);
        }

        _currentRoom = roomName;

        // Wait one frame so scene objects spawn
        yield return null;

        // --- FIND PLAYER SPAWN POINT ---
        GameObject spawn = GameObject.FindWithTag("PlayerSpawn");
        if (spawn == null)
        {
            Debug.LogWarning($"No PlayerSpawn found in scene '{roomName}'!");
            yield break;
        }

        // --- FIND XR RIG ---
        XROrigin rig = FindObjectOfType<XROrigin>();
        if (rig == null)
        {
            Debug.LogError("No XROrigin found in the persistent scene!");
            yield break;
        }

        // --- TELEPORT XR RIG ---
        rig.MoveCameraToWorldLocation(spawn.transform.position);
        rig.transform.rotation = spawn.transform.rotation;

        Debug.Log($"Player moved to spawn point in: {roomName}");
    }
}
