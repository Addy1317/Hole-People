using UnityEngine;

namespace SlowpokeStudio.Grid
{
    public enum CellType { Empty, Character, Hole }

    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 10;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Vector3 originPosition = Vector3.zero;

        private CellType[,] gridArray;

        [Header("System References")]
        [SerializeField] internal GridPathHandler pathCheckSystem;
        [SerializeField] internal GridObjectDetection gridObjectDetection;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
               Destroy(gameObject);
                return;
            }
            Instance = this;

            gridArray = new CellType[gridWidth, gridHeight];
            InitializeGrid();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;   
            }
        }

        private void InitializeGrid()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    gridArray[x, y] = CellType.Empty;
                }
            }

            Debug.Log("[GridManager] Grid initialized.");
        }

        internal CellType GetCell(int x, int y)
        {
            if (IsWithinBounds(x, y))
                return gridArray[x, y];
            return CellType.Empty;
        }

        internal void SetCell(int x, int y, CellType type)
        {
            if (IsWithinBounds(x, y))
            {
                gridArray[x, y] = type;
                Debug.Log($"[GridManager] Cell ({x},{y}) set to {type}");
            }
        }

        internal bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < gridWidth && y < gridHeight;
        }

        internal Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt((worldPosition.x - originPosition.x) / cellSize);
            int y = Mathf.RoundToInt((worldPosition.z - originPosition.z) / cellSize); 
            return new Vector2Int(x, y);
        }

        internal Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(
                x * cellSize + originPosition.x,
                originPosition.y,
                y * cellSize + originPosition.z
            );
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 pos = GetWorldPosition(x, y);
                    Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.1f, cellSize));
                }
            }
        }
    }
}


