using UnityEngine;

namespace SlowpokeStudio.Grid
{
    [RequireComponent(typeof(LineRenderer))]
    public class GridLineRenderer : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int rows = 6;
        [SerializeField] private int columns = 6;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 origin = Vector3.zero; // now Vector3 instead of Vector2

        [Header("Line Settings")]
        [SerializeField] private Material lineMaterial;
        [SerializeField] private float lineWidth = 0.02f;
        [SerializeField] private Color lineColor = Color.green;

        private Transform gridParent;

        private void Start()
        {
            DrawGridLines();
        }

        private void DrawGridLines()
        {
            // Clear old grid if exists
            if (gridParent != null) Destroy(gridParent.gameObject);

            gridParent = new GameObject("GridLines").transform;
            gridParent.SetParent(transform);

            // Horizontal lines (along X axis, moving in Z)
            for (int z = 0; z <= rows; z++)
            {
                Vector3 start = origin + new Vector3(0, 0, z * cellSize);
                Vector3 end = origin + new Vector3(columns * cellSize, 0, z * cellSize);
                CreateLine(start, end, gridParent);
            }

            // Vertical lines (along Z axis, moving in X)
            for (int x = 0; x <= columns; x++)
            {
                Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
                Vector3 end = origin + new Vector3(x * cellSize, 0, rows * cellSize);
                CreateLine(start, end, gridParent);
            }
        }

        private void CreateLine(Vector3 start, Vector3 end, Transform parent)
        {
            GameObject lineObj = new GameObject("GridLine");
            lineObj.transform.SetParent(parent);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.sortingOrder = 10;

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
    }
}
    
