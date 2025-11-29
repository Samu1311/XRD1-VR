using UnityEngine;
using UnityEngine.Events;

/// Watches multiple pressure plates and fires OnSolved when conditions are met.
public class PressurePlatePuzzleController : MonoBehaviour
{
    public PressurePlate[] plates;

    [Header("Events")]
    public UnityEvent OnSolved;

    [Header("Instructions")]
    [Tooltip("Assign the InstructionTextCanvas in the scene.")]
    public InstructionTextCanvas instructionCanvas;

    private bool _solved;

    private void Awake()
    {
        foreach (var plate in plates)
        {
            if (plate != null)
                plate.OnPlateStateChanged.AddListener(OnPlateStateChanged);
        }
    }

    private void Start()
    {
        ShowRoomInstructions();
    }

    private void OnDestroy()
    {
        foreach (var plate in plates)
        {
            if (plate != null)
                plate.OnPlateStateChanged.RemoveListener(OnPlateStateChanged);
        }
    }

    private void OnPlateStateChanged(bool _)
    {
        if (_solved) return;

        if (CheckSolved())
        {
            _solved = true;
            Debug.Log("Pressure Plate Puzzle Solved!");
            OnSolved?.Invoke();
        }
    }

    private bool CheckSolved()
    {
        foreach (var plate in plates)
        {
            if (plate.requiredToSolve && !plate.IsPressed)
                return false;

            if (plate.mustStayUnpressed && plate.IsPressed)
                return false;
        }
        return true;
    }

    // Show instructions when the player enters the room
    private void ShowRoomInstructions()
    {
        if (instructionCanvas != null)
        {
            instructionCanvas.ShowInstructions("Find all the boxes and place them on pressure plates \nto reveal the next portal!");
        }
    }
}
