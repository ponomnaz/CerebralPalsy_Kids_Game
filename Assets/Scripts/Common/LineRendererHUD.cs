using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A custom UI component that renders lines on the HUD using a grid-based system.
/// </summary>
public class LineRendererHUD : Graphic
{
    // Reference to the UIGridRenderer to determine grid size.
    public UIGridRenderer GridRenderer;

    // List of Points that define the line to be drawn.
    public List<Vector2> Points;

    // Stores the size of the grid for rendering lines.
    private Vector2Int _gridSize;

    // _thickness of the lines to be drawn.
    private const float _thickness = 15f;

    // Width and _height of the parent UI element.
    private float _width;
    private float _height;

    // Width and _height of each unit in the grid.
    private float _unitWidth;
    private float _unitHeight;

    // Update is called once per frame
    void Update()
    {
        // Check if the grid size has changed and update if necessary.
        if (GridRenderer != null && _gridSize != GridRenderer.GetGridSize())
        {
            _gridSize = GridRenderer.GetGridSize();
            SetVerticesDirty(); // Mark the vertices as dirty to trigger a redraw.
        }
    }

    /// <summary>
    /// Populates the mesh for the line renderer.
    /// </summary>
    /// <param Name="vh">The VertexHelper used to construct the UI mesh.</param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {

        vh.Clear(); // Clear any previous vertices from the mesh.

        // Get the current _width and _height of the rect transform.
        _width = rectTransform.rect.width;
        _height = rectTransform.rect.height;

        // Calculate the _width and _height of each unit in the grid.
        _unitWidth = _width / _gridSize.x;
        _unitHeight = _height / _gridSize.y;

        // Exit if there are not enough Points to draw a line.
        if (Points.Count < 2) return;


        for (int i = 0; i < Points.Count - 1; i++)
        {
            // Get the current point and the next point.
            Vector2 point = Points[i];
            Vector2 point2 = Points[i + 1];

            // Calculate the angle for the current line segment.
            float angle = GetAngle(point, point2) + 90f;

            // Draw vertices for the current line segment.
            DrawVerticesForPoint(point, point2, angle, vh);
        }

        // Create triangles for rendering the line segments.
        for (int i = 0; i < Points.Count - 1; i++)
        {
            int index = i * 4; // Calculate the starting index for the current segment.
            vh.AddTriangle(index + 0, index + 1, index + 2); // Triangle 1
            vh.AddTriangle(index + 1, index + 2, index + 3); // Triangle 2
        }
    }

    /// <summary>
    /// Calculates the angle between two Points.
    /// </summary>
    /// <param Name="current">The starting point.</param>
    /// <param Name="target">The target point.</param>
    /// <returns>The angle in degrees between the two Points.</returns>
    private float GetAngle(Vector2 current, Vector2 target)
    {
        return (float)(Mathf.Atan2(9f * (target.y - current.y), 16f * (target.x - current.x)) * (180 / Mathf.PI));
    }

    /// <summary>
    /// Draws the vertices for a line segment between two Points.
    /// </summary>
    /// <param Name="point">The starting point of the segment.</param>
    /// <param Name="point2">The ending point of the segment.</param>
    /// <param Name="angle">The angle of the segment.</param>
    /// <param Name="vh">The VertexHelper used to construct the UI mesh.</param>
    private void DrawVerticesForPoint(Vector2 point, Vector2 point2, float angle, VertexHelper vh)
    {
        UIVertex vertex = UIVertex.simpleVert; // Create a simple vertex.

        // Set the vertex color.
        vertex.color = color;

        // Calculate and add vertices for the starting point.
        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-_thickness / 2, 0) + new Vector3(_unitWidth * point.x, _unitHeight * point.y);
        vh.AddVert(vertex); // Add First vertex

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(_thickness / 2, 0) + new Vector3(_unitWidth * point.x, _unitHeight * point.y);
        vh.AddVert(vertex); // Add Second vertex

        // Calculate and add vertices for the ending point.
        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-_thickness / 2, 0) + new Vector3(_unitWidth * point2.x, _unitHeight * point2.y);
        vh.AddVert(vertex); // Add Third vertex

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(_thickness / 2, 0) + new Vector3(_unitWidth * point2.x, _unitHeight * point2.y);
        vh.AddVert(vertex); // Add fourth vertex
    }
}
