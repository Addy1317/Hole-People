using UnityEngine;

namespace SlowpokeStudio.Grid
{
    public enum CellType { Empty, Character, Hole }

    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] internal int rows = 6;
        [SerializeField] internal int columns = 6;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 gridOrigin = Vector3.zero;

        [Header("Debug Options")]
        [SerializeField] private bool showGridGizmos = true;
        [SerializeField] private Color gridColor = Color.yellow;

        [SerializeField] internal GridObjectDetection gridObjectDetection;
        [SerializeField] internal GridPathHandler pathCheckSystem;

        private CellType[,] gridArray;
        public static GridManager Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Debug.LogWarning("[GridManager] Duplicate instance found. Destroying this one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        
        InitializeGrid();
        }

        private void InitializeGrid()
        {
            gridArray = new CellType[columns, rows];

            // Fill with Empty
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    gridArray[x, y] = CellType.Empty;
                    Debug.Log($"[GridManager] Cell ({x},{y}) initialized as {gridArray[x, y]}");
                }
            }
        }

        #region Helper Methods
        public Vector3 GetWorldPosition(int x, int y)
        {
            return gridOrigin + new Vector3(x + 0.5f, 0, y + 0.5f) * cellSize; //new Vector3(x, 0, y) * cellSize;
        }

        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition - gridOrigin).x / cellSize);
            int y = Mathf.FloorToInt((worldPosition - gridOrigin).z / cellSize);
            return new Vector2Int(x, y);
        }

        public bool IsInsideGrid(int x, int y)
        {
            return x >= 0 && y >= 0 && x < columns && y < rows;
        }

        public void SetCell(int x, int y, CellType type)
        {
            if (IsInsideGrid(x, y))
            {
                gridArray[x, y] = type;
                Debug.Log($"[GridManager] Cell ({x},{y}) set to {type}");
            }
        }

        public CellType GetCell(int x, int y)
        {
            if (IsInsideGrid(x, y))
                return gridArray[x, y];

            return CellType.Empty;
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmos()
        {
            if (!showGridGizmos) return;

            Gizmos.color = gridColor;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Vector3 pos = GetWorldPosition(x, y);
                    Gizmos.DrawWireCube(pos, Vector3.one * (cellSize - 0.05f));
                }
            }
        }
        #endregion
    }
}


