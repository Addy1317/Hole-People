using SlowpokeStudio.character;
using System.Collections.Generic;
using UnityEngine;
using SlowpokeStudio.ManHole;

namespace SlowpokeStudio.Grid
{
    [System.Serializable]
    public struct GridObjectData
    {
        public Vector2Int gridPosition;
        public ObjectColor color;

        public GridObjectData(Vector2Int pos, ObjectColor col)
        {
            gridPosition = pos;
            color = col;
        }
    }

    public class GridObjectDetection : MonoBehaviour
    {
        [Header("Detected Objects")]
        public List<GridObjectData> characterDataList = new List<GridObjectData>();
        public List<GridObjectData> holeDataList = new List<GridObjectData>();

        private void Start()
        {
            DetectSceneObjects();
        }

        private void DetectSceneObjects()
        {
            characterDataList.Clear();
            holeDataList.Clear();

            // Detect Characters
            GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Character");
            foreach (GameObject obj in characterObjects)
            {
                Character character = obj.GetComponent<Character>();
                if (character == null) continue;

                Vector2Int gridPos = GridManager.Instance.GetGridPosition(obj.transform.position);
                characterDataList.Add(new GridObjectData(gridPos, character.characterColor));
                Debug.Log($"[GridObjectDetection] Character detected at {gridPos} with color {character.characterColor}");
            }

            // Detect Holes
            GameObject[] holeObjects = GameObject.FindGameObjectsWithTag("Hole");
            foreach (GameObject obj in holeObjects)
            {
                
                Hole hole = obj.GetComponent<Hole>();
                if (hole == null) continue;

                Vector2Int gridPos = GridManager.Instance.GetGridPosition(obj.transform.position);
                holeDataList.Add(new GridObjectData(gridPos, hole.holeColor));
                Debug.Log($"[GridObjectDetection] Hole detected at {gridPos} with color {hole.holeColor}");
            }

            Debug.Log($"[GridObjectDetection] Total Characters: {characterDataList.Count} | Total Holes: {holeDataList.Count}");
        }

        public void RemoveCharacterAt(Vector2Int gridPos)
        {
            characterDataList.RemoveAll(data => data.gridPosition == gridPos);
            Debug.Log($"[GridObjectDetection] Removed character at {gridPos}. Remaining: {characterDataList.Count}");
        }

        public void RefreshCharacterList()
        {
            characterDataList.Clear();

            GameObject[] allCharacters = GameObject.FindGameObjectsWithTag("Character");

            foreach (GameObject character in allCharacters)
            {
                var mover = character.GetComponent<CharacterManager>();
                var data = new GridObjectData
                {
                    gridPosition = GridManager.Instance.GetGridPosition(character.transform.position),
                    color = mover.GetColor()
                };

                characterDataList.Add(data);
            }
        }

        public void CleanupInactiveCharacters()
        {
            characterDataList.RemoveAll(data =>
            {
                Vector3 worldPos = GridManager.Instance.GetWorldPosition(data.gridPosition.x, data.gridPosition.y);
                Collider[] hits = Physics.OverlapSphere(worldPos, 0.1f);
                foreach (var hit in hits)
                {
                    CharacterManager cm = hit.GetComponent<CharacterManager>();
                    if (cm != null && cm.gameObject.activeInHierarchy)
                        return false;
                }
                return true; // remove if no active character found
            });
        }

        public List<GridObjectData> GetAdjacentSameColorCharacters(Vector2Int currentPos, ObjectColor color)
        {
            List<GridObjectData> result = new List<GridObjectData>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<Vector2Int> toVisit = new Queue<Vector2Int>();

            toVisit.Enqueue(currentPos);
            visited.Add(currentPos);

            while (toVisit.Count > 0)
            {
                Vector2Int current = toVisit.Dequeue();

                GridObjectData currentData = characterDataList.Find(data => data.gridPosition == current && data.color == color);
                if (!EqualityComparer<GridObjectData>.Default.Equals(currentData, default))
                {
                    result.Add(currentData);

                    // Check 4 directions
                    Vector2Int[] directions = new Vector2Int[]
                    {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
                    };

                    foreach (Vector2Int dir in directions)
                    {
                        Vector2Int neighbor = current + dir;
                        if (!visited.Contains(neighbor))
                        {
                            var neighborData = characterDataList.Find(data => data.gridPosition == neighbor && data.color == color);
                            if (!EqualityComparer<GridObjectData>.Default.Equals(neighborData, default))
                            {
                                toVisit.Enqueue(neighbor);
                                visited.Add(neighbor);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public List<CharacterManager> GetConnectedCharactersFrom(Vector2Int holePos, ObjectColor targetColor)
        {
            List<CharacterManager> result = new List<CharacterManager>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            // Directions: up, down, left, right
            Vector2Int[] directions = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            queue.Enqueue(holePos);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                foreach (Vector2Int dir in directions)
                {
                    Vector2Int neighborPos = current + dir;

                    if (visited.Contains(neighborPos))
                        continue;

                    visited.Add(neighborPos);

                    // Check if there's a character at this position with the correct color
                    GridObjectData match = characterDataList.Find(obj =>
                        obj.gridPosition == neighborPos && obj.color == targetColor);

                    if (!match.Equals(default(GridObjectData)))

                        {
                            // Find the actual CharacterMover in the scene at this world position
                            Vector3 worldPos = GridManager.Instance.GetWorldPosition(neighborPos.x, neighborPos.y);
                        Collider[] hits = Physics.OverlapSphere(worldPos, 0.1f);

                        foreach (Collider hit in hits)
                        {
                            CharacterManager mover = hit.GetComponent<CharacterManager>();
                            if (mover != null && !result.Contains(mover))
                            {
                                result.Add(mover);
                                queue.Enqueue(neighborPos); // Explore further from this point
                                break;
                            }
                        }
                    }
                }
            }

            Debug.Log($"[GridObjectDetection] Connected characters to Hole at {holePos}: {result.Count}");
            return result;
        }

        public List<GridObjectData> GetAllCharactersOfColor(ObjectColor color)
        {
            List<GridObjectData> result = new List<GridObjectData>();

            foreach (var data in characterDataList)
            {
                if (data.color == color)
                {
                    result.Add(data);
                }
            }

            return result;
        }

    }
}

