using SlowpokeStudio.Grid;
using UnityEngine;

namespace SlowpokeStudio
{
    public class CharacterSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private GameObject characterPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private int numberOfCharacters = 10;

        private void Start()
        {
            if (gridManager == null)
            {
                Debug.LogError("[CharacterSpawner] GridManager not assigned!");
                return;
            }

            SpawnCharactersRandomly();
        }

        private void SpawnCharactersRandomly()
        {
            int rows = gridManager.rows;
            int cols = gridManager.columns;

            for (int i = 0; i < numberOfCharacters; i++)
            {
                int randX = Random.Range(0, cols);
                int randY = Random.Range(0, rows);

                // Skip if already occupied
                if (gridManager.GetCell(randX, randY) != CellType.Empty)
                {
                    i--; // retry
                    continue;
                }

                Vector3 spawnPos = gridManager.GetWorldPosition(randX, randY);

                GameObject characterObj = Instantiate(characterPrefab, spawnPos, Quaternion.identity);
                characterObj.name = $"Character_{i}";

                // Update grid
                gridManager.SetCell(randX, randY, CellType.Character);
            }

            Debug.Log($"[CharacterSpawner] Spawned {numberOfCharacters} characters");
        }
    }
}
