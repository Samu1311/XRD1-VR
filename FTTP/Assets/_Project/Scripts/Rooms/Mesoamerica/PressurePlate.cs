using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A pressure plate that reacts when objects with the correct tag sit on it.
/// - Uses a trigger collider on the root.
/// - Moves the plate down visually.
/// - Fires an event when pressed/unpressed.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [Tooltip("If true, this plate must be pressed to solve the puzzle.")]
    public bool requiredToSolve = true;

    [Tooltip("If true, this plate must NOT be pressed to solve the puzzle.")]
    public bool mustStayUnpressed = false;

    [Header("Detection Settings")]
    [Tooltip("Objects with these tags can activate the plate.")]
    public string[] activatingTags = { "Weight" };

    [Header("Visual Settings")]
    [Tooltip("The mesh that moves visually when pressed.")]
    public Transform plateVisual;

    [Tooltip("How far down the plate moves when pressed.")]
    public float pressedYOffset = -0.05f;

    [Tooltip("Speed of plate movement.")]
    public float moveSpeed = 8f;

    [Header("Events")]
    public UnityEvent<bool> OnPlateStateChanged;

    private int _objectsOnPlate = 0;
    private bool _isPressed;
    private Vector3 _initialLocalPos;

    public bool IsPressed => _isPressed;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (plateVisual == null && transform.childCount > 0)
            plateVisual = transform.GetChild(0);

        if (plateVisual != null)
            _initialLocalPos = plateVisual.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActivator(other)) return;

        _objectsOnPlate++;
        UpdatePlateState();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsActivator(other)) return;

        _objectsOnPlate = Mathf.Max(0, _objectsOnPlate - 1);
        UpdatePlateState();
    }

    private bool IsActivator(Collider other)
    {
        foreach (var tag in activatingTags)
        {
            if (other.CompareTag(tag))
                return true;
        }
        return false;
    }

    private void UpdatePlateState()
    {
        bool newState = _objectsOnPlate > 0;

        if (newState != _isPressed)
        {
            _isPressed = newState;
            OnPlateStateChanged?.Invoke(_isPressed);
        }
    }

    private void Update()
    {
        if (plateVisual == null) return;

        Vector3 target = _initialLocalPos;
        if (_isPressed)
            target += new Vector3(0f, pressedYOffset, 0f);

        plateVisual.localPosition = Vector3.Lerp(
            plateVisual.localPosition,
            target,
            Time.deltaTime * moveSpeed
        );
    }
}
