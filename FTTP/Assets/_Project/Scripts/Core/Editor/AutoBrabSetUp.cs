using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AutoGrabSetup : EditorWindow
{
    [MenuItem("Tools/Auto Setup VR Grab Objects")]
    public static void ShowWindow()
    {
        GetWindow<AutoGrabSetup>("Auto Grab Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto Setup VR Grab Objects", EditorStyles.boldLabel);

        if (GUILayout.Button("Convert Selected Objects"))
        {
            ConvertSelectedObjects();
        }
    }

    private void ConvertSelectedObjects()
    {
        foreach (GameObject selected in Selection.gameObjects)
        {
            SetupObject(selected);
        }
    }

    private void SetupObject(GameObject model)
    {
        if (model == null) return;

        // Create root object
        GameObject root = new GameObject(model.name + "_GrabRoot");
        Undo.RegisterCreatedObjectUndo(root, "Create GrabRoot");

        root.transform.position = model.transform.position;
        root.transform.rotation = model.transform.rotation;
        root.transform.localScale = model.transform.localScale;

        // Parent model under root
        model.transform.SetParent(root.transform, true);

        // Recentering model under root
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        // Add Rigidbody
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Add MeshCollider (Convex)
        MeshCollider meshCol = root.AddComponent<MeshCollider>();
        MeshFilter mf = model.GetComponentInChildren<MeshFilter>();
        if (mf != null)
        {
            meshCol.sharedMesh = mf.sharedMesh;
            meshCol.convex = true;
        }

        // Add XRGrabInteractable
        XRGrabInteractable grab = root.AddComponent<XRGrabInteractable>();
        grab.trackPosition = true;
        grab.trackRotation = true;
        grab.throwOnDetach = true;

        // Add your pickup script
        if (root.GetComponent<VRPickUpItem>() == null)
            root.AddComponent<VRPickUpItem>();

        // Optional: Trigger collider for table detection
        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.25f;

        EditorUtility.SetDirty(root);

    }
}
