using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    public int spawnPointIndex; // Used to identify this spawn point
    public Color gizmoColor = Color.cyan; // Color for the editor gizmo
    public float gizmoSize = 0.5f; // Size of the editor gizmo
    
    [Header("Spawn Probability")]
    [Range(0, 1)]
    public float spawnProbability = 1.0f; // Chance this spawn point will be used when selected
    
    // Draw a visual representation in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoSize);
        
        // Draw a line downward to show the fall path
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 10f, transform.position.z));
    }
}
