using SlowpokeStudio.character;
using System.Collections.Generic;
using UnityEngine;
using SlowpokeStudio.ManHole;

namespace SlowpokeStudio.Grid
{
    [System.Serializable]
    public class GridObjectData
    {
        public Vector2Int gridPosition;
        public ObjectColor color;
        public CharacterManager characterRef;

        public GridObjectData(Vector2Int pos, ObjectColor c, CharacterManager cm = null)
        {
            gridPosition = pos;
            color = c;
            characterRef = cm;
        }
    }

    public class GridObjectDetection : MonoBehaviour
    {
        #region old code
        /*        [Header("Detected Objects")]
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
                        GridManager.Instance.SetCell(gridPos.x, gridPos.y, CellType.Character);
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

                        GridManager.Instance.SetCell(gridPos.x, gridPos.y, CellType.Hole);
                        Debug.Log($"[GridObjectDetection] Hole detected at {gridPos} with color {hole.holeColor}");
                    }

                    Debug.Log($"[GridObjectDetection] Total Characters: {characterDataList.Count} | Total Holes: {holeDataList.Count}");
                }

                public void RemoveCharacterAt(Vector2Int gridPos)
                {
                    characterDataList.RemoveAll(data => data.gridPosition == gridPos);
                    Debug.Log($"[GridObjectDetection] Removed character at {gridPos}. Remaining: {characterDataList.Count}");
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

                public void UpdateCharacterPosition(CharacterManager character, Vector2Int oldPos, Vector2Int newPos)
                {
                    for (int i = 0; i < characterDataList.Count; i++)
                    {
                        if (characterDataList[i].gridPosition == oldPos && characterDataList[i].characterRef == character)
                        {
                            characterDataList[i].gridPosition = newPos;
                            Debug.Log($"[GridObjectDetection] Moved character to {newPos}");
                            return;
                        }
                    }

                    Debug.LogWarning($"[GridObjectDetection] Couldn't find character at {oldPos} to update!");
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
                            result.Add(data);
                    }
                    return result;
                }

                void OnDrawGizmos()
                {
                    foreach (var data in characterDataList)
                    {
                        Gizmos.color = Color.red;
                        Vector3 pos = GridManager.Instance.GetWorldPosition(data.gridPosition.x, data.gridPosition.y);
                        Gizmos.DrawSphere(pos + Vector3.up * 0.5f, 0.2f);
                    }
                }*/
        #endregion

        [Header("Detected Objects")]
        public List<GridObjectData> characterDataList = new List<GridObjectData>();
        public List<GridObjectData> holeDataList = new List<GridObjectData>();

        // ✅ New: Dictionary for BFS fast lookup
        public Dictionary<Vector2Int, GridObjectData> characterMap = new Dictionary<Vector2Int, GridObjectData>();

        private void Start()
        {
            DetectSceneObjects();
        }

        private void DetectSceneObjects()
        {
            characterDataList.Clear();
            holeDataList.Clear();
            characterMap.Clear();

            // Detect Characters
            GameObject[] characterObjects = GameObject.FindGameObjectsWithTag("Character");
            foreach (GameObject obj in characterObjects)
            {
                Character character = obj.GetComponent<Character>();
                CharacterManager cm = obj.GetComponent<CharacterManager>();
                if (character == null || cm == null) continue;

                Vector2Int gridPos = GridManager.Instance.GetGridPosition(obj.transform.position);

                var data = new GridObjectData(gridPos, character.characterColor, cm);
                characterDataList.Add(data);
                characterMap[gridPos] = data;

                GridManager.Instance.SetCell(gridPos.x, gridPos.y, CellType.Character);

                Debug.Log($"[GridObjectDetection] Character detected at {gridPos} with color {character.characterColor}");
            }

            // Detect Holes
            GameObject[] holeObjects = GameObject.FindGameObjectsWithTag("Hole");
            foreach (GameObject obj in holeObjects)
            {
                Hole hole = obj.GetComponent<Hole>();
                if (hole == null) continue;

                Vector2Int gridPos = GridManager.Instance.GetGridPosition(obj.transform.position);
                var data = new GridObjectData(gridPos, hole.holeColor);
                holeDataList.Add(data);

                GridManager.Instance.SetCell(gridPos.x, gridPos.y, CellType.Hole);

                Debug.Log($"[GridObjectDetection] Hole detected at {gridPos} with color {hole.holeColor}");
            }

            Debug.Log($"[GridObjectDetection] Total Characters: {characterDataList.Count} | Total Holes: {holeDataList.Count}");
        }

        // ✅ Remove character from list + dictionary
        public void RemoveCharacterAt(Vector2Int gridPos)
        {
            characterDataList.RemoveAll(data => data.gridPosition == gridPos);
            characterMap.Remove(gridPos);

            Debug.Log($"[GridObjectDetection] Removed character at {gridPos}. Remaining: {characterDataList.Count}");
        }

        // ✅ Update character’s grid position
        public void UpdateCharacterPosition(CharacterManager character, Vector2Int oldPos, Vector2Int newPos)
        {
            // Update in list
            for (int i = 0; i < characterDataList.Count; i++)
            {
                if (characterDataList[i].gridPosition == oldPos && characterDataList[i].characterRef == character)
                {
                    characterDataList[i].gridPosition = newPos;
                    break;
                }
            }

            // Update dictionary
            if (characterMap.ContainsKey(oldPos))
            {
                var data = characterMap[oldPos];
                characterMap.Remove(oldPos);
                data.gridPosition = newPos;
                characterMap[newPos] = data;
            }

            Debug.Log($"[GridObjectDetection] Character moved from {oldPos} → {newPos}");
        }

        // ✅ Cleanup inactive characters (destroyed or deactivated)
        public void CleanupInactiveCharacters()
        {
            List<Vector2Int> toRemove = new List<Vector2Int>();

            foreach (var kvp in characterMap)
            {
                var cm = kvp.Value.characterRef;
                if (cm == null || !cm.gameObject.activeInHierarchy)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var pos in toRemove)
            {
                characterMap.Remove(pos);
                characterDataList.RemoveAll(d => d.gridPosition == pos);
                Debug.Log($"[GridObjectDetection] Cleaned inactive character at {pos}");
            }
        }

        // ✅ Utility: Get all characters of a specific color
        public List<GridObjectData> GetAllCharactersOfColor(ObjectColor color)
        {
            List<GridObjectData> result = new List<GridObjectData>();
            foreach (var data in characterDataList)
            {
                if (data.color == color)
                    result.Add(data);
            }
            return result;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var data in characterDataList)
            {
                Vector3 pos = GridManager.Instance.GetWorldPosition(data.gridPosition.x, data.gridPosition.y);
                Gizmos.DrawSphere(pos + Vector3.up * 0.5f, 0.2f);
            }
        }
    }
}

