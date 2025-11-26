using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Watches several CalendarRing components and fires OnSolved when
/// all rings are in their correct positions.
/// </summary>
public class CalendarPuzzleController : MonoBehaviour
{
    [Tooltip("Rings belonging to this puzzle (outer, middle, inner).")]
    public CalendarRing[] rings;

    [Header("Events")]
    public UnityEvent OnSolved;

    private bool _isSolved;

    private void Awake()
    {
        if (rings == null || rings.Length == 0)
        {
            Debug.LogWarning($"{name}: No rings assigned to CalendarPuzzleController.");
            return;
        }

        foreach (var ring in rings)
        {
            if (ring == null) continue;
            ring.OnCorrectStateChanged.AddListener(OnRingCorrectStateChanged);
        }
    }

    private void OnDestroy()
    {
        if (rings == null) return;

        foreach (var ring in rings)
        {
            if (ring == null) continue;
            ring.OnCorrectStateChanged.RemoveListener(OnRingCorrectStateChanged);
        }
    }

    private void OnRingCorrectStateChanged(bool _)
    {
        if (_isSolved) return;

        foreach (var ring in rings)
        {
            if (ring == null || !ring.IsCorrect)
                return; // at least one incorrect
        }

        _isSolved = true;
        Debug.Log("Calendar puzzle solved!");
        OnSolved?.Invoke();
    }
}
