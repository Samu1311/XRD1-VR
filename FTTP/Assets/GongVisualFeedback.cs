using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GongVisualFeedback : MonoBehaviour
{
    public Material normalMaterial;
    public Material highlightMaterial;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        rend.material = normalMaterial;
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        rend.material = highlightMaterial;
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        rend.material = normalMaterial;
    }
}

