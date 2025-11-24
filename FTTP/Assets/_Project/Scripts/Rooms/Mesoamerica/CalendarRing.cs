using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles snapping and correctness for a single rotating calendar ring.
/// Attach this to the ring GameObject that has XRGrabInteractable + Rigidbody.
/// </summary>
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class CalendarRing : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Rotation Settings")]
    [Tooltip("How many discrete positions this ring can snap to (e.g. 12 -> every 30 degrees).")]
    public int steps = 12;

    [Tooltip("Index of the correct step (0..steps-1).")]
    public int correctStepIndex = 0;

    [Tooltip("Which local axis this ring rotates around.")]
    public RotationAxis rotationAxis = RotationAxis.Z;

    [Header("Events")]
    [Tooltip("Called whenever this ring becomes correct or incorrect.")]
    public UnityEvent<bool> OnCorrectStateChanged;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private bool _isCorrect;
    private Vector3 _baseEuler;

    public bool IsCorrect => _isCorrect;

    private void Awake()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        _baseEuler = transform.localEulerAngles;

        _grab.selectExited.AddListener(OnGrabReleased);
    }

    private void OnDestroy()
    {
        if (_grab != null)
            _grab.selectExited.RemoveListener(OnGrabReleased);
    }

    private void OnGrabReleased(SelectExitEventArgs args)
    {
        SnapToNearestStep();
    }

    private void SnapToNearestStep()
    {
        if (steps <= 0)
        {
            Debug.LogWarning($"{name}: steps must be > 0.");
            return;
        }

        float angle = GetAxisAngle() - GetBaseAxisAngle();
        angle = Mathf.Repeat(angle, 360f);

        float stepAngle = 360f / steps;
        int nearestStepIndex = Mathf.RoundToInt(angle / stepAngle) % steps;
        float snappedAngle = nearestStepIndex * stepAngle;

        bool newCorrect = (nearestStepIndex == correctStepIndex);
        if (newCorrect != _isCorrect)
        {
            _isCorrect = newCorrect;
            OnCorrectStateChanged?.Invoke(_isCorrect);
        }

        SetAxisAngle(GetBaseAxisAngle() + snappedAngle);
    }

    private float GetAxisAngle()
    {
        var e = transform.localEulerAngles;
        return rotationAxis switch
        {
            RotationAxis.X => e.x,
            RotationAxis.Y => e.y,
            _               => e.z
        };
    }

    private float GetBaseAxisAngle()
    {
        return rotationAxis switch
        {
            RotationAxis.X => _baseEuler.x,
            RotationAxis.Y => _baseEuler.y,
            _               => _baseEuler.z
        };
    }

    private void SetAxisAngle(float a)
    {
        var e = transform.localEulerAngles;
        switch (rotationAxis)
        {
            case RotationAxis.X: e.x = a; break;
            case RotationAxis.Y: e.y = a; break;
            case RotationAxis.Z: e.z = a; break;
        }
        transform.localRotation = Quaternion.Euler(e);
    }
}
