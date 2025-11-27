using System.Diagnostics;
using UnityEngine;

public class ActivateAndMovePortal : MonoBehaviour
{
    [Header("Portal Object")]
    public GameObject portal;

    [Header("New Position For Portal")]
    public Transform newLocation;

    public void ActivatePortal()
    {
        if (portal == null || newLocation == null)
        {
            return;
        }

        // Activate the portal
        portal.SetActive(true);

        // Move the portal to the new location
        portal.transform.position = newLocation.position;

        // If you also want the rotation to match:
        portal.transform.rotation = newLocation.rotation;

    }
}
