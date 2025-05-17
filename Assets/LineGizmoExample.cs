using UnityEngine;

public class LineGizmoExample : MonoBehaviour
{
    public Transform endPoint;
    [Range(0f, 1f)]
    public float alpha = 0.75f; 

    private void OnDrawGizmos()
    {
        if (endPoint == null) return;

        // Set the color with custom alpha
        Gizmos.color = new Color(1f, 1f, 0f, alpha); // Yellow with custom alpha

        // Draw the line
        Gizmos.DrawLine(transform.position, endPoint.position);

        // Draw spheres at start and end points
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(endPoint.position, 0.1f);

        // Calculate and display the midpoint
        Vector3 midpoint = (transform.position + endPoint.position) / 2f;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(midpoint, 0.15f);

        // Display the distance
        float distance = Vector3.Distance(transform.position, endPoint.position);
        UnityEditor.Handles.Label(midpoint, $"Distance: {distance:F2}");
    }
}