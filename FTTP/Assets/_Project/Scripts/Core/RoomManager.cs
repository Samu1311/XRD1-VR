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
        // --- REMOVE MAIN PORTAL BEFORE TRANSITION ---
        GameObject mainPortal = GameObject.FindWithTag("MainPortal");
        if (mainPortal != null)
        {
            Destroy(mainPortal);
            Debug.Log("Main portal removed before loading new room.");
        }

        // --- LOAD TARGET ROOM ADDITIVELY ---
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // --- UNLOAD PREVIOUS ROOM (EXCEPT MAIN) ---
        if (!string.IsNullOrEmpty(_currentRoom) &&
            _currentRoom != "Main")
        {
            SceneManager.UnloadSceneAsync(_currentRoom);
        }

        _currentRoom = roomName;

        yield return null; // wait for objects to spawn


        // --- FIND THE SPAWN POINT ---
        GameObject spawn = GameObject.FindWithTag("PlayerSpawn");

        if (spawn == null)
        {
            Debug.LogWarning($"No PlayerSpawn found in scene '{roomName}'!");
            LoadingScreenManager.Instance.Hide();
            yield break;
        }

        // --- FIND THE XR ORIGIN ---
        XROrigin rig = FindObjectOfType<XROrigin>();
        if (rig == null)
        {
            Debug.LogError("No XROrigin found in persistent scene!");
            LoadingScreenManager.Instance.Hide();
            yield break;
        }

        // --- TELEPORT THE PLAYER ---
        rig.MoveCameraToWorldLocation(spawn.transform.position);
        rig.transform.rotation = spawn.transform.rotation;

        Debug.Log($"Player moved to spawn point in: {roomName}");

        // --- HIDE LOADING SCREEN ---
        LoadingScreenManager.Instance.Hide();
    }
}
