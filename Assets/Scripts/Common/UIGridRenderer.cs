using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Renders a grid of cells with a specified _thickness within a UI element.
/// </summary>
public class UIGridRenderer : Graphic
{
    /// <summary>
    /// Defines the grid size (number of cells along x and y axes).
    /// </summary>
    private Vector2Int _gridSize = new(1, 12);

    /// <summary>
    /// Specifies the _thickness of the grid lines.
    /// </summary>
    private const float _thickness = 5f;

    // Width and _height of the entire grid
    private float _width;
    private float _height;

    // Width and _height of each cell in the grid
    private float _cellWidth;
    private float _cellHeight;

    public Vector2Int GetGridSize() => _gridSize;

    public void SetGridSize(Vector2Int gridSize) => _gridSize.x = gridSize.x;

    /// <summary>
    /// Called when the UI element needs to regenerate its mesh.
    /// Populates the mesh with vertices and triangles for each cell in the grid.
    /// </summary>
    /// <param Name="vh">Helper object to populate vertex data.</param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        // Calculate the _width and _height of the grid based on RectTransform dimensions
        _width = rectTransform.rect.width;
        _height = rectTransform.rect.height;

        // Calculate each cell's _width and _height based on grid size
        _cellWidth = _width / _gridSize.x;
        _cellHeight = _height / _gridSize.y;

        // Create cells by iterating over the grid's rows and columns
        int count = 0;
        for (int y = 0; y < _gridSize.y; y++)
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                // Draw each cell and increment index counter
                DrawCell(x, y, count, vh);
                count++;
            }
        } 
    }

    /// <summary>
    /// Draws a single cell within the grid at the specified (x, y) position.
    /// </summary>
    /// <param Name="x">The cell's x-position in the grid.</param>
    /// <param Name="y">The cell's y-position in the grid.</param>
    /// <param Name="index">The cell index, used to calculate vertex offsets.</param>
    /// <param Name="vh">Helper object for populating vertex data.</param>
    private void DrawCell(int x, int y, int index, VertexHelper vh)
    {
        // Calculate the cell's bottom-left corner position
        float xPos = _cellWidth * x;
        float yPos = _cellHeight * y;

        // Initialize a base vertex with the assigned color
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        // Define the outer vertices (corners of the cell)
        vertex.position = new Vector3(xPos, yPos);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos, yPos + _cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + _cellWidth, yPos + _cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + _cellWidth, yPos);
        vh.AddVert(vertex);

        // Calculate _thickness-based offset for inner vertices
        float widthSqr = _thickness * _thickness;
        float distanceSqr = widthSqr / 2f;
        float distance = Mathf.Sqrt(distanceSqr);

        // Define the inner vertices (offset to create line _thickness)
        vertex.position = new Vector3(xPos + distance, yPos + distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + distance, yPos + _cellHeight - distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + _cellWidth - distance, yPos + _cellHeight - distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + _cellWidth - distance, yPos + distance);
        vh.AddVert(vertex);

        // Calculate the starting index offset for this cell's vertices
        int offset = index * 8;

        // Add triangles for each side of the cell's border to form the grid lines
        vh.AddTriangle(offset + 0, offset + 1, offset + 5);
        vh.AddTriangle(offset + 5, offset + 4, offset + 0);

        vh.AddTriangle(offset + 1, offset + 2, offset + 6);
        vh.AddTriangle(offset + 6, offset + 5, offset + 1);

        vh.AddTriangle(offset + 2, offset + 3, offset + 7);
        vh.AddTriangle(offset + 7, offset + 6, offset + 2);

        vh.AddTriangle(offset + 3, offset + 0, offset + 4);
        vh.AddTriangle(offset + 4, offset + 7, offset + 3);
    }
}