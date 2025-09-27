using SlowpokeStudio.Services;
using System.Collections;
using UnityEngine;

namespace SlowpokeStudio.Level
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private LevelSO levelDatabase;   
        [SerializeField] private Transform levelParent;   

        private int currentLevelIndex = 0;
        private GameObject activeLevelInstance;

        private void Awake()
        {
            if (levelDatabase == null || levelDatabase.levels.Length == 0)
            {
                Debug.LogError("[LevelManager] No LevelSO or levels assigned!");
                return;
            }
        }
        /*
                private void Start()
                {
                    LoadLevel(currentLevelIndex);
                }

                private void LoadLevel(int index)
                {
                    if (index < 0 || index >= levelDatabase.levels.Length)
                    {
                        Debug.LogError($"[LevelManager] Invalid level index {index}");
                        return;
                    }

                    // Remove previous level
                    if (activeLevelInstance != null)
                    {
                        Destroy(activeLevelInstance);
                        activeLevelInstance = null;
                    }

                    LevelDataSO levelData = levelDatabase.levels[index];
                    if (levelData.levelPrefab == null)
                    {
                        Debug.LogError($"[LevelManager] No prefab assigned for {levelData.levelName}");
                        return;
                    }

                    // Instantiate new level
                    activeLevelInstance = Instantiate(levelData.levelPrefab, levelParent);
                    currentLevelIndex = index;
                    // StartCoroutine(InitializeLevelNextFrame());

                    GameService.Instance.playerController.InitLevelReferences();
                    Debug.Log($"[LevelManager] Loaded Level {levelData.levelIndex}: {levelData.levelName}, Coins Reward: {levelData.coins}");
                }




                private IEnumerator InitializeLevelNextFrame()
                {
                    yield return null; // wait 1 frame
                    GameService.Instance.playerController.InitLevelReferences();
                }

                internal void LoadNextLevel()
                {
                    int nextIndex = currentLevelIndex + 1;

                    if (nextIndex >= levelDatabase.levels.Length)
                    {
                        Debug.Log("[LevelManager] All levels completed!");
                        // TODO: Trigger Game Over / Loop back to first level if needed
                        return;
                    }

                    LoadLevel(nextIndex);
                }*/

        private void Start()
        {
            StartCoroutine(LoadLevelRoutine(currentLevelIndex));
        }

        // Call this instead of LoadLevel(index)
        private System.Collections.IEnumerator LoadLevelRoutine(int index)
        {
            if (index < 0 || index >= levelDatabase.levels.Length)
            {
                Debug.LogError($"[LevelManager] Invalid level index {index}");
                yield break;
            }

            // 1) Tear down current level (deferred)
            if (activeLevelInstance != null)
            {
                Destroy(activeLevelInstance);
                activeLevelInstance = null;
            }

            // 2) Wait until the previous GridManager instance is actually gone
            //    (Destroy() completes end-of-frame)
            yield return null; // one frame is usually enough
            // If you want to be extra safe:
            // yield return new WaitUntil(() => GridManager.Instance == null);

            // 3) Spawn the new level
            LevelDataSO levelData = levelDatabase.levels[index];
            if (levelData.levelPrefab == null)
            {
                Debug.LogError($"[LevelManager] No prefab assigned for {levelData.levelName}");
                yield break;
            }

            activeLevelInstance = Instantiate(levelData.levelPrefab, levelParent);
            currentLevelIndex = index;

            // 4) Give the new GridManager a frame to run Awake/Start, then init PlayerController
            yield return null;
            GameService.Instance.playerController.InitLevelReferences();

            Debug.Log($"[LevelManager] Loaded Level {levelData.levelIndex}: {levelData.levelName}, Coins: {levelData.coins}");
        }

        internal void LoadNextLevel()
        {
            int nextIndex = currentLevelIndex + 1;
            if (nextIndex >= levelDatabase.levels.Length)
            {
                Debug.Log("[LevelManager] All levels completed!");
                return;
            }
            StartCoroutine(LoadLevelRoutine(nextIndex));
        }
    }
}
