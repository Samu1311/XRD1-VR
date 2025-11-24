using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // <-- add this

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class CalendarRing : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Rotation Settings")]
    public int steps = 8;
    public int correctStepIndex = 0;
    public RotationAxis rotationAxis = RotationAxis.Z;

    [Header("Events")]
    public UnityEvent<bool> OnCorrectStateChanged;

    private XRGrabInteractable _grab;
    private bool _isCorrect;
    private bool _isGrabbed;

    private Vector3 _baseEuler;
    private Vector3 _baseLocalPosition;

    public bool IsCorrect => _isCorrect;

    private void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        _baseEuler = transform.localEulerAngles;
        _baseLocalPosition = transform.localPosition;

        _grab.selectEntered.AddListener(OnGrabStarted);
        _grab.selectExited.AddListener(OnGrabReleased);
    }

    private void OnDestroy()
    {
        if (_grab != null)
        {
            _grab.selectEntered.RemoveListener(OnGrabStarted);
            _grab.selectExited.RemoveListener(OnGrabReleased);
        }
    }

    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        _isGrabbed = true;
    }

    private void OnGrabReleased(SelectExitEventArgs args)
    {
        _isGrabbed = false;
        SnapToNearestStep();
    }

    private void LateUpdate()
    {
        if (!_isGrabbed) return;

        // Keep ring fixed on the wall
        transform.localPosition = _baseLocalPosition;

        // Lock other axes so it behaves like a wheel
        var e = transform.localEulerAngles;

        switch (rotationAxis)
        {
            case RotationAxis.X:
                e.y = _baseEuler.y;
                e.z = _baseEuler.z;
                break;
            case RotationAxis.Y:
                e.x = _baseEuler.x;
                e.z = _baseEuler.z;
                break;
            case RotationAxis.Z:
                e.x = _baseEuler.x;
                e.y = _baseEuler.y;
                break;
        }

        transform.localRotation = Quaternion.Euler(e);
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
