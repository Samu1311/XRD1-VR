using UnityEngine;

/// <summary>
/// Simple terrain collider fix - attach directly to terrain GameObjects
/// </summary>
public class SimpleTerrainFix : MonoBehaviour
{
    private void Start()
    {
        // Wait a moment then refresh
        Invoke("FixTerrain", 1f);
    }

    private void FixTerrain()
    {
        TerrainCollider collider = GetComponent<TerrainCollider>();
        if (collider != null)
        {
            // Simple toggle fix
            collider.enabled = false;
            collider.enabled = true;
            Debug.Log($"Fixed terrain collider on: {gameObject.name}");
        }
    }

    // Public method to call manually
    [ContextMenu("Fix This Terrain")]
    public void FixThisTerrain()
    {
        FixTerrain();
    }
}