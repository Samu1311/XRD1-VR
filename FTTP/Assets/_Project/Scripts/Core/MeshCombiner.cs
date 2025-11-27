using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine Meshes (Preserve Transform)")]
    public void CombineMeshes()
    {
        // Save parent transform
        Vector3 originalPosition = transform.position;
        Quaternion originalRotation = transform.rotation;
        Vector3 originalScale = transform.localScale;

        // Reset to identity to combine correctly
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int index = 0;
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            combine[index].mesh = mf.sharedMesh;
            combine[index].transform = mf.transform.localToWorldMatrix;
            index++;
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        // Create combined object
        GameObject combinedObj = new GameObject(name + "_Combined");

        var mfCombined = combinedObj.AddComponent<MeshFilter>();
        var mrCombined = combinedObj.AddComponent<MeshRenderer>();
        var mcCombined = combinedObj.AddComponent<MeshCollider>();

        mfCombined.sharedMesh = combinedMesh;
        mrCombined.sharedMaterials = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterials;
        mcCombined.sharedMesh = combinedMesh;

        // Restore original transform
        combinedObj.transform.position = originalPosition;
        combinedObj.transform.rotation = originalRotation;
        combinedObj.transform.localScale = originalScale;

        // Restore original parent transform
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;

        Debug.Log("Mesh combined successfully!");
    }
}
