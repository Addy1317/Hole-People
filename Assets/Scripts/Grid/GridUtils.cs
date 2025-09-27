using SlowpokeStudio.character;
using System.Collections.Generic;
using UnityEngine;

namespace SlowpokeStudio.Grid
{
    public class GridUtils : MonoBehaviour
    {
        public List<GridObjectData> GetAdjacentSameColorCharacters(
            Vector2Int currentPos,
            ObjectColor myColor,
            Dictionary<Vector2Int, GridObjectData> characterMap)
        {
            List<GridObjectData> neighbors = new List<GridObjectData>();

            // Define 4-directional neighbors (no diagonals)
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1),   // Up
                new Vector2Int(1, 0),   // Right
                new Vector2Int(0, -1),  // Down
                new Vector2Int(-1, 0),  // Left
            };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighborPos = currentPos + dir;

                if (characterMap.TryGetValue(neighborPos, out GridObjectData neighbor))
                {
                    if (neighbor.color == myColor)
                    {
                        neighbors.Add(neighbor);

                        Debug.Log($"[GridUtils] Match found at {neighborPos} with color {neighbor.color}");
                    }
                }
            }

            return neighbors;
        }
    }
}
