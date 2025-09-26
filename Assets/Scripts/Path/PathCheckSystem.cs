using SlowpokeStudio.character;
using SlowpokeStudio.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace SlowpokeStudio.Grid
{
    public class PathCheckSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private GridObjectDetection objectDetection;

        public List<GridObjectData> GetMovableCharacters(Vector2Int holePos, ObjectColor targetColor)
        {
            List<GridObjectData> validCharacters = new List<GridObjectData>();

            foreach (GridObjectData charData in objectDetection.characterDataList)
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

        private bool IsPathClearVertical(Vector2Int hole, Vector2Int character)
        {
            int minY = Mathf.Min(hole.y, character.y) + 1;
            int maxY = Mathf.Max(hole.y, character.y);

            for (int y = minY; y < maxY; y++)
            {
                if (gridManager.GetCell(hole.x, y) != CellType.Empty)
                    return false;
            }
            return true;
        }

        private bool IsPathClearHorizontal(Vector2Int hole, Vector2Int character)
        {
            int minX = Mathf.Min(hole.x, character.x) + 1;
            int maxX = Mathf.Max(hole.x, character.x);

            for (int x = minX; x < maxX; x++)
            {
                if (gridManager.GetCell(x, hole.y) != CellType.Empty)
                    return false;
            }
            return true;
        }
    }
}

