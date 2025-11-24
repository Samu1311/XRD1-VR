using Unity.XR.CoreUtils;  // Needed for XROrigin
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles loading/unloading room scenes additively.
/// Keeps one persistent XR rig and teleports it to spawn points in rooms.
/// Also removes the Main portal when entering any other room.
/// </summary>
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Add your room scene names here EXACTLY as in Build Settings")]
    [SerializeField] private string[] roomSceneNames;

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


    /// <summary>
    /// Loads a room by index from the array.
    /// </summary>
    public void LoadRoom(int index)
    {
        if (index < 0 || index >= roomSceneNames.Length)
        {
            Debug.LogError("Room index out of range!");
            return;
        }

        LoadRoomByName(roomSceneNames[index]);
    }


    /// <summary>
    /// Loads a room scene by name, unloads the previous room,
    /// and moves the XR Rig to the PlayerSpawnPoint.
    /// Also destroys the Main portal so it doesn't appear in the new room.
    /// </summary>
    public void LoadRoomByName(string roomName)
    {
        StartCoroutine(LoadRoomRoutine(roomName));
    }


    private System.Collections.IEnumerator LoadRoomRoutine(string roomName)
    {
        // --- LOAD NEW ROOM ADDITIVELY ---
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;


        // --- REMOVE ONLY THE MAIN PORTAL ---
        GameObject mainPortal = GameObject.FindWithTag("MainPortal");
        if (mainPortal != null)
        {
            Destroy(mainPortal);
            Debug.Log("Main portal removed when entering new room.");
        }


        // --- UNLOAD PREVIOUS ROOM (NOT Main) ---
        if (!string.IsNullOrEmpty(_currentRoom) && _currentRoom != "Main")
        {
            SceneManager.UnloadSceneAsync(_currentRoom);
        }

        _currentRoom = roomName;


        // --- MOVE PLAYER TO SPAWN POINT ---
        yield return null; // Wait one frame so objects spawn in the new scene

        GameObject spawn = GameObject.FindWithTag("PlayerSpawn");

        if (spawn == null)
        {
            Debug.LogWarning($"No PlayerSpawnPoint found in scene '{roomName}'. Add an object tagged PlayerSpawn.");
            yield break;
        }

        XROrigin rig = FindObjectOfType<XROrigin>();

        if (rig == null)
        {
            Debug.LogError("No XR Origin found in the persistent Main scene!");
            yield break;
        }

        // Teleport XR rig to spawn location
        rig.MoveCameraToWorldLocation(spawn.transform.position);
        rig.transform.rotation = spawn.transform.rotation;

        Debug.Log($"Player moved to spawn point in room: {roomName}");
    }
}
