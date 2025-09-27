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

        private void Start()
        {
            currentLevelIndex = GameService.Instance.saveManager.CurrentLevelIndex;
            StartCoroutine(LoadLevelRoutine(currentLevelIndex));
        }

        private IEnumerator LoadLevelRoutine(int index)
        {
            if (index < 0 || index >= levelDatabase.levels.Length)
            {
                Debug.LogError($"[LevelManager] Invalid level index {index}");
                yield break;
            }

            if (activeLevelInstance != null)
            {
                Destroy(activeLevelInstance);
                activeLevelInstance = null;
            }

            yield return null; 

            LevelDataSO levelData = levelDatabase.levels[index];
            if (levelData.levelPrefab == null)
            {
                Debug.LogError($"[LevelManager] No prefab assigned for {levelData.levelName}");
                yield break;
            }

            activeLevelInstance = Instantiate(levelData.levelPrefab, levelParent);
            currentLevelIndex = index;

            yield return null;

            if (GameService.Instance.playerController != null)
            {
                GameService.Instance.playerController.InitLevelReferences();
            }
            else
            {
                Debug.LogError("[LevelManager] PlayerController not found in GameService!");
            }

            if (GameService.Instance.uiManager != null)
            {
                var levelinfo = levelDatabase.levels[currentLevelIndex];
                GameService.Instance.uiManager.UpdateLevelText(levelinfo.levelIndex, levelinfo.levelName);
            }

            Debug.Log($"[LevelManager] Loaded Level {levelData.levelIndex}: {levelData.levelName}, Coins: {levelData.coins}");
        }

        internal void LoadNextLevel()
        {
            var coins = levelDatabase.levels[currentLevelIndex].coins;
            GameService.Instance.currencyManager.AddCoins(coins);

            int nextIndex = currentLevelIndex + 1;
            if (nextIndex >= levelDatabase.levels.Length)
            {
                Debug.Log("[LevelManager] All levels completed!");
                OnAllLevelsCompleted();
                return;
            }
            GameService.Instance.saveManager.SaveLevel(currentLevelIndex, currentLevelIndex);
            StartCoroutine(LoadLevelRoutine(nextIndex));
        }

        internal void RestartLevel()
        {
            Debug.Log($"[LevelManager] Restarting Level {currentLevelIndex}");
            StartCoroutine(LoadLevelRoutine(currentLevelIndex));
        }

        private void OnAllLevelsCompleted()
        {
            if (GameService.Instance.uiManager != null)
            {
                GameService.Instance.uiManager.OnAllLevelCompleted();
            }
            else
            {
                Debug.LogWarning("[LevelManager] UIManager not found. Cannot show completion panel.");
            }
        }
    }
}
