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
        [Header("References")]
        [SerializeField] private GridManager gridManager;

        [Header("Detected Objects")]
        public List<GridObjectData> characterDataList = new List<GridObjectData>();
        public List<GridObjectData> holeDataList = new List<GridObjectData>();

        private void Start()
        {
            if (gridManager == null)
            {
                Debug.LogError("[GridObjectDetection] GridManager not assigned!");
                return;
            }

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

                Vector2Int gridPos = gridManager.GetGridPosition(obj.transform.position);
                characterDataList.Add(new GridObjectData(gridPos, character.characterColor));
                Debug.Log($"[GridObjectDetection] Character detected at {gridPos} with color {character.characterColor}");
            }

            // Detect Holes
            GameObject[] holeObjects = GameObject.FindGameObjectsWithTag("Hole");
            foreach (GameObject obj in holeObjects)
            {
                
                Hole hole = obj.GetComponent<Hole>();
                if (hole == null) continue;

                Vector2Int gridPos = gridManager.GetGridPosition(obj.transform.position);
                holeDataList.Add(new GridObjectData(gridPos, hole.holeColor));
                Debug.Log($"[GridObjectDetection] Hole detected at {gridPos} with color {hole.holeColor}");
            }

            Debug.Log($"[GridObjectDetection] Total Characters: {characterDataList.Count} | Total Holes: {holeDataList.Count}");
        }

        public void RemoveCharacterAt(Vector2Int gridPos)
        {
            characterDataList.RemoveAll(c => c.gridPosition == gridPos);
        }

        public void RefreshCharacterList()
        {
            characterDataList.Clear();

            GameObject[] allCharacters = GameObject.FindGameObjectsWithTag("Character");

            foreach (GameObject character in allCharacters)
            {
                var mover = character.GetComponent<CharacterMover>();
                var data = new GridObjectData
                {
                    gridPosition = gridManager.GetGridPosition(character.transform.position),
                    color = mover.GetColor()
                };

                characterDataList.Add(data);
            }
        }

    }
}

