using UnityEngine;

/// <summary>
/// Toggles a glow object when a pressure plate changes state.
/// </summary>
public class PlateGlow : MonoBehaviour
{
    public GameObject glowObject;

    public void SetGlow(bool active)
    {
        if (glowObject != null)
            glowObject.SetActive(active);
    }
}
