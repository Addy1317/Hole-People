using SlowpokeStudio.character;
using System.Collections.Generic;
using UnityEngine;

namespace SlowpokeStudio.Grid
{
    public class GridPathHandler : MonoBehaviour
    {
        [Header("Grid Bounds (override if needed)")]
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 10;
        [SerializeField] private float characterLookupRadius = 0.2f;

        public Dictionary<CharacterManager, List<Vector2Int>> CollectReachableFromHole(Vector2Int holePos, ObjectColor targetColor)
        {
            var result = new Dictionary<CharacterManager, List<Vector2Int>>();

            Queue<Vector2Int> q = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            q.Enqueue(holePos);
            visited.Add(holePos);

            while (q.Count > 0)
            {
                var current = q.Dequeue();

                foreach (var nb in GetNeighbors4(current))
                {
                    if (visited.Contains(nb)) continue;
                    if (!GridManager.Instance.IsWithinBounds(nb.x, nb.y)) continue;

                    var cell = GridManager.Instance.GetCell(nb.x, nb.y);

                    if (cell == CellType.Empty)
                    {
                        visited.Add(nb);
                        cameFrom[nb] = current;
                        q.Enqueue(nb);
                    }

                    else if (cell == CellType.Character)
                    {
                        if (GridManager.Instance.gridObjectDetection.characterMap.TryGetValue(nb, out var charData))
                        {
                            if (charData.color == targetColor)
                            {
                                if (!cameFrom.ContainsKey(nb))
                                    cameFrom[nb] = current;

                                CharacterManager cm = charData.characterRef;
                                if (cm != null && cm.gameObject.activeInHierarchy)
                                {
                                    List<Vector2Int> path = ReconstructPath(cameFrom, holePos, nb);
                                    result[cm] = path;
                                }

                                visited.Add(nb);
                                q.Enqueue(nb);
                            }
                        }
                    }
                }
            }

            Debug.Log($"[GridPathHandler/BFS] Reachable {targetColor} characters: {result.Count} (Hole at {holePos})");
            return result;
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int cur = end;
            path.Add(cur);

            while (cur != start)
            {
                cur = cameFrom[cur];
                path.Add(cur);
            }

            path.Reverse();
            return path;
        }

        private IEnumerable<Vector2Int> GetNeighbors4(Vector2Int pos)
        {
            yield return new Vector2Int(pos.x + 1, pos.y);
            yield return new Vector2Int(pos.x - 1, pos.y);
            yield return new Vector2Int(pos.x, pos.y + 1);
            yield return new Vector2Int(pos.x, pos.y - 1);
        }

        private CharacterManager FindCharacterAtGridPos(Vector2Int gridPos)
        {
            Vector3 world = GridManager.Instance.GetWorldPosition(gridPos.x, gridPos.y);
            var hits = Physics.OverlapSphere(world, characterLookupRadius);
            foreach (var h in hits)
            {
                var cm = h.GetComponent<CharacterManager>();
                if (cm != null) return cm;
            }
            return null;
        }

        internal List<GridObjectData> GetMovableCharacters(Vector2Int holePos, ObjectColor targetColor)
        {
            List<GridObjectData> validCharacters = new List<GridObjectData>();

            foreach (GridObjectData charData in GridManager.Instance.gridObjectDetection.characterDataList)
            {
                // Color Match
                if (charData.color != targetColor)
                    continue;

                // Row or Column Match
                if (charData.gridPosition.x == holePos.x)
                {
                    // Same column → check vertical path
                    if (IsPathClearVertical(holePos, charData.gridPosition))
                        validCharacters.Add(charData);
                }
                else if (charData.gridPosition.y == holePos.y)
                {
                    // Same row → check horizontal path
                    if (IsPathClearHorizontal(holePos, charData.gridPosition))
                        validCharacters.Add(charData);
                }
            }

            Debug.Log($"[PathCheckSystem] Found {validCharacters.Count} valid characters for hole at {holePos} with color {targetColor}");
            return validCharacters;
        }

        internal bool IsPathClearVertical(Vector2Int hole, Vector2Int character)
        {
            int minY = Mathf.Min(hole.y, character.y) + 1;
            int maxY = Mathf.Max(hole.y, character.y);

            for (int y = minY; y < maxY; y++)
            {
                if (GridManager.Instance.GetCell(hole.x, y) != CellType.Empty)
                    return false;
            }
            return true;
        }

        internal bool IsPathClearHorizontal(Vector2Int hole, Vector2Int character)
        {
            int minX = Mathf.Min(hole.x, character.x) + 1;
            int maxX = Mathf.Max(hole.x, character.x);

            for (int x = minX; x < maxX; x++)
            {
                if (GridManager.Instance.GetCell(x, hole.y) != CellType.Empty)
                    return false;
            }
            return true;
        }

        public bool HasClearPath(Vector2Int holePos, Vector2Int charPos)
        {
            if (charPos.x == holePos.x)
                return IsPathClearVertical(holePos, charPos);
            else if (charPos.y == holePos.y)
                return IsPathClearHorizontal(holePos, charPos);

            return false;
        }

    }
}

