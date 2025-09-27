using UnityEngine;

namespace SlowpokeStudio.Level
{
    [System.Serializable]
    public class LevelDataSO 
    {
        [Header("Level Information")]
        public string levelName;
        public int levelIndex;

        [Header("Prefab Reference")]
        public GameObject levelPrefab;

        [Header("Optional Settings")]
        public int coins = 0;   
    }

    [CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelSO")]
    public class LevelSO : ScriptableObject
    {
        [Header("List of Levels")]
        public LevelDataSO[] levels;
    }
}
