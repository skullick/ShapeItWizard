using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3[] points;


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.startColor = Color.magenta;
        lineRenderer.endColor = Color.magenta;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
        Debug.Log("LineRenderer initialized");
    }

    public void UpdatePoints(Vector3[] newPoints, bool changeColor = false)
    {
        if (changeColor)
        {
            SetColor(Color.green);
        }
        else
        {
            SetColor(Color.magenta);
        }

        points = newPoints;

        if (points != null && points.Length > 0)
        {
            lineRenderer.positionCount = points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
                // Debug.Log($"Point {i}: World ({points[i].x}, {points[i].y}, {points[i].z})");
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
            Debug.Log("No points to render");
        }
    }

    public void SetColor(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            
        }
        else
        {
            Debug.LogWarning("LineRenderer not found, unable to set color");
        }
    }
}

