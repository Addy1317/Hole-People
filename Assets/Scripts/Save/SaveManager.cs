using UnityEngine;

namespace SlowpokeStudio.Save
{
    public class SaveManager : MonoBehaviour
    {
        private const string KEY_CURRENT_LEVEL = "CurrentLevelIndex";
        private const string KEY_HIGHEST_LEVEL = "HighestLevelIndex";
        private const string KEY_TOTAL_COINS = "TotalCoins";

        public int CurrentLevelIndex { get; private set; }
        public int HighestLevelIndex { get; private set; }
        public int TotalCoins { get; private set; }

        private void Awake()
        {
            LoadGame();
        }

        #region Save / Load
        public void SaveLevel(int currentLevel, int highestLevel)
        {
            CurrentLevelIndex = currentLevel;
            HighestLevelIndex = Mathf.Max(HighestLevelIndex, highestLevel);

            PlayerPrefs.SetInt(KEY_CURRENT_LEVEL, CurrentLevelIndex);
            PlayerPrefs.SetInt(KEY_HIGHEST_LEVEL, HighestLevelIndex);
            PlayerPrefs.Save();

            Debug.Log($"[SaveManager] Level saved → Current: {CurrentLevelIndex}, Highest: {HighestLevelIndex}");
        }

        public void SaveCoins(int coins)
        {
            TotalCoins = coins;
            PlayerPrefs.SetInt(KEY_TOTAL_COINS, TotalCoins);
            PlayerPrefs.Save();

            Debug.Log($"[SaveManager] Coins saved → Total: {TotalCoins}");
        }

        public void LoadGame()
        {
            CurrentLevelIndex = PlayerPrefs.GetInt(KEY_CURRENT_LEVEL, 0);  
            HighestLevelIndex = PlayerPrefs.GetInt(KEY_HIGHEST_LEVEL, 0);
            TotalCoins = PlayerPrefs.GetInt(KEY_TOTAL_COINS, 0);

            Debug.Log($"[SaveManager] Loaded → CurrentLevel: {CurrentLevelIndex}, HighestLevel: {HighestLevelIndex}, Coins: {TotalCoins}");
        }

        public void ResetSave()
        {
            PlayerPrefs.DeleteAll();
            CurrentLevelIndex = 0;
            HighestLevelIndex = 0;
            TotalCoins = 0;
            Debug.Log("[SaveManager] Save data reset.");
        }
        public void ClearAllData()
        {
            Debug.Log("[SaveManager] Clearing all saved data...");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        #endregion
    }
}

