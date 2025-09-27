using SlowpokeStudio.character;
using SlowpokeStudio.ManHole;
using SlowpokeStudio.Services;
using System.Collections.Generic;
using UnityEngine;

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
        [Header("Detected Objects")]
        public List<GridObjectData> characterDataList = new List<GridObjectData>();
        public List<GridObjectData> holeDataList = new List<GridObjectData>();

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

        internal void RemoveCharacterAt(Vector2Int gridPos)
        {
            characterDataList.RemoveAll(data => data.gridPosition == gridPos);
            characterMap.Remove(gridPos);

            Debug.Log($"[GridObjectDetection] Removed character at {gridPos}. Remaining: {characterDataList.Count}");
            CheckForLevelCompletion();
        }

        internal void UpdateCharacterPosition(CharacterManager character, Vector2Int oldPos, Vector2Int newPos)
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

            if (characterMap.ContainsKey(oldPos))
            {
                var data = characterMap[oldPos];
                characterMap.Remove(oldPos);
                data.gridPosition = newPos;
                characterMap[newPos] = data;
            }

            Debug.Log($"[GridObjectDetection] Character moved from {oldPos} → {newPos}");
        }

        private void CheckForLevelCompletion()
        {
            if (characterDataList.Count == 0)
            {
                Debug.Log("[GridObjectDetection] All characters cleared → Level Complete!");

                // 🔹 Broadcast via EventManager
                if (GameService.Instance != null && GameService.Instance.eventManager != null)
                {
                    
                    GameService.Instance.eventManager.OnLevelCompleteEvent.InvokeEvent();
                }
                else
                {
                    Debug.LogWarning("[GridObjectDetection] Could not invoke OnLevelCompleteEvent → EventManager missing!");
                }
            }
        }

        internal void CleanupInactiveCharacters()
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

        internal List<GridObjectData> GetAllCharactersOfColor(ObjectColor color)
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

