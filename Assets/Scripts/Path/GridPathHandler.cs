using SlowpokeStudio.character;
using System.Collections.Generic;
using UnityEngine;

namespace SlowpokeStudio.Grid
{
    public class GridPathHandler : MonoBehaviour
    {
/*        public List<GridObjectData> GetMovableCharacters(Vector2Int holePos, ObjectColor targetColor)
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
*/
        //==========================================================================================================

        [Header("Grid Bounds (override to match GridManager if needed)")]
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 10;

        [Header("Lookup Settings")]
        [SerializeField] private float characterLookupRadius = 0.2f;

        /// <summary>
        /// NEW (BFS): For the tapped hole, find all characters of the given color
        /// that have a valid path to the hole. Returns CharacterManager -> path.
        /// Path is an ordered list of grid cells from the character's current cell to the hole cell.
        /// </summary>
        public Dictionary<CharacterManager, List<Vector2Int>> GetReachableCharactersAndPaths(Vector2Int holePos, ObjectColor targetColor)
        {
            var result = new Dictionary<CharacterManager, List<Vector2Int>>();

            // Iterate over every character of this color tracked by detection
            foreach (GridObjectData charData in GridManager.Instance.gridObjectDetection.characterDataList)
            {
                if (charData.color != targetColor)
                    continue;

                var start = charData.gridPosition;

                // Skip if this grid cell no longer has an active character in the scene
                var cm = FindCharacterAtGridPos(start);
                if (cm == null || !cm.gameObject.activeInHierarchy)
                    continue;

                if (TryFindPathBFS(start, holePos, out var path))
                {
                    result[cm] = path; // store full path for movement
                }
            }

            Debug.Log($"[GridPathHandler/BFS] Reachable {targetColor} characters: {result.Count} (Hole at {holePos})");
            return result;
        }


        /* public Dictionary<CharacterManager, List<Vector2Int>> GetReachableCharactersAndPaths(Vector2Int holePos, ObjectColor targetColor)
         {
             var result = new Dictionary<CharacterManager, List<Vector2Int>>();

             // BFS setup
             Queue<Vector2Int> queue = new Queue<Vector2Int>();
             HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
             Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

             queue.Enqueue(holePos);
             visited.Add(holePos);

             while (queue.Count > 0)
             {
                 Vector2Int current = queue.Dequeue();

                 foreach (var neighbor in GetNeighbors4(current))
                 {
                     if (visited.Contains(neighbor))
                         continue;

                     if (!GridManager.Instance.IsWithinBounds(neighbor.x, neighbor.y))
                         continue;

                     var cellType = GridManager.Instance.GetCell(neighbor.x, neighbor.y);

                     // ✅ Case 1: Empty → keep exploring
                     if (cellType == CellType.Empty)
                     {
                         visited.Add(neighbor);
                         queue.Enqueue(neighbor);
                         cameFrom[neighbor] = current;
                     }
                     // ✅ Case 2: Character → check color
                     else if (cellType == CellType.Character)
                     {
                         // Is there a character of the right color here?
                         if (GridManager.Instance.gridObjectDetection.characterMap.TryGetValue(neighbor, out GridObjectData data))
                         {
                             if (data.color == targetColor)
                             {
                                 CharacterManager cm = data.characterRef;
                                 if (cm != null && cm.gameObject.activeInHierarchy && !result.ContainsKey(cm))
                                 {
                                     // Make sure character cell is connected in cameFrom
                                     if (!cameFrom.ContainsKey(neighbor))
                                         cameFrom[neighbor] = current;

                                     // Build path from hole to this character
                                     var path = ReconstructPath(cameFrom, holePos, neighbor);
                                     result[cm] = path;

                                     Debug.Log($"[GridPathHandler/BFS] Found reachable {targetColor} character at {neighbor}");
                                 }
                             }
                         }

                         // ⚠️ Important: Do NOT enqueue Character cells, they block movement further
                         visited.Add(neighbor);
                     }
                     // ❌ Case 3: Hole → skip (not traversable)
                     else if (cellType == CellType.Hole)
                     {
                         visited.Add(neighbor);
                     }
                 }
             }

             Debug.Log($"[GridPathHandler/BFS] Reachable {targetColor} characters: {result.Count} (Hole at {holePos})");
             return result;
         }*/
        /// <summary>
        /// NEW (BFS): Shortest path from start to target through Empty cells (target may be Hole).
        /// Returns true if a path exists; 'path' is filled with the sequence of grid cells (inclusive).
        /// </summary>
        public bool TryFindPathBFS(Vector2Int start, Vector2Int target, out List<Vector2Int> path)
        {
            path = null;

            // Early exit: if start == target, trivially done
            if (start == target)
            {
                path = new List<Vector2Int> { start };
                return true;
            }

            var q = new Queue<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var visited = new HashSet<Vector2Int>();

            q.Enqueue(start);
            visited.Add(start);

            while (q.Count > 0)
            {
                var current = q.Dequeue();

                foreach (var nb in GetNeighbors4(current))
                {
                    if (visited.Contains(nb))
                        continue;

                    if (!IsWalkable(nb, target))
                        continue;

                    visited.Add(nb);
                    cameFrom[nb] = current;

                    if (nb == target)
                    {
                        // reconstruct path
                        path = ReconstructPath(cameFrom, start, target);
                        return true;
                    }

                    q.Enqueue(nb);
                }
            }

            return false;
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int target)
        {
            var rev = new List<Vector2Int>();
            var cur = target;
            rev.Add(cur);

            while (cur != start)
            {
                cur = cameFrom[cur];
                rev.Add(cur);
            }

            rev.Reverse();
            return rev;
        }

        /// <summary>
        /// A cell is walkable for BFS if:
        /// - It is Empty, OR
        /// - It is exactly the target (the hole cell)
        /// Any Character or non-target Hole cell blocks the path.
        /// </summary>
        private bool IsWalkable(Vector2Int pos, Vector2Int target)
        {
            // Bounds check
            if (pos.x < 0 || pos.x >= gridWidth || pos.y < 0 || pos.y >= gridHeight)
                return false;

            var cell = GridManager.Instance.GetCell(pos.x, pos.y);

            if (pos == target)
                return true;

            return cell == CellType.Empty;
        }

        private IEnumerable<Vector2Int> GetNeighbors4(Vector2Int pos)
        {
            // Up, Down, Left, Right
            yield return new Vector2Int(pos.x, pos.y + 1);
            yield return new Vector2Int(pos.x, pos.y - 1);
            yield return new Vector2Int(pos.x - 1, pos.y);
            yield return new Vector2Int(pos.x + 1, pos.y);
        }

        /// <summary>
        /// Resolve the CharacterManager at a given grid position via physics overlap.
        /// Keeps this class independent from whether characterRef was filled in detection.
        /// </summary>
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

        // -------------------------------
        // YOUR EXISTING STRAIGHT-LINE API
        // (kept for compatibility)
        // -------------------------------

        public List<GridObjectData> GetMovableCharacters(Vector2Int holePos, ObjectColor targetColor)
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

