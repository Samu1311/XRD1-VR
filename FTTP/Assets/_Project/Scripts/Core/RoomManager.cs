using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles loading/unloading of additive "time room" scenes.
/// Only one room scene is loaded at a time.
/// </summary>
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Time Room Scenes (additive)")]
    [Tooltip("Scene names for each time room, in chronological order.")]
    [SerializeField] private string[] roomSceneNames;

    private int _currentRoomIndex = -1;
    private string _loadedRoomScene;

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

    private void Start()
    {
        // Optional: automatically load the first room.
        if (roomSceneNames != null && roomSceneNames.Length > 0)
        {
            LoadRoom(0);
        }
        else
        {
            Debug.LogWarning("RoomManager: No roomSceneNames configured.");
        }
    }

    public void LoadRoom(int index)
    {
        if (index < 0 || index >= roomSceneNames.Length)
        {
            Debug.LogWarning($"RoomManager: Invalid room index {index}");
            return;
        }

        // Unload previously loaded room, if any.
        if (!string.IsNullOrEmpty(_loadedRoomScene))
        {
            SceneManager.UnloadSceneAsync(_loadedRoomScene);
        }

        string sceneName = roomSceneNames[index];

        // Load room additively so Main stays active.
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        _currentRoomIndex = index;
        _loadedRoomScene = sceneName;

        Debug.Log($"RoomManager: Loaded room '{sceneName}'");
    }

    public void LoadNextRoom()
    {
        int nextIndex = _currentRoomIndex + 1;
        if (nextIndex < roomSceneNames.Length)
        {
            LoadRoom(nextIndex);
        }
        else
        {
            Debug.Log("RoomManager: No more rooms. End of timeline.");
            // TODO: later show end screen or credits.
        }
    }
}