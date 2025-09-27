using SlowpokeStudio.Services;
using UnityEngine;

namespace SlowpokeStudio.Currecny
{
    public class CurrencyManager : MonoBehaviour
    {
        [Header("Currency Settings")]
        [SerializeField] private int startingCoins = 0;

        private int totalCoins;

        private void Awake()
        {
            totalCoins = startingCoins;
            Debug.Log($"[CurrencyManager] Initialized with {totalCoins} coins");

            GameService.Instance.eventManager.OnCoinsChanged.InvokeEvents(totalCoins);
        }

        internal void AddCoins(int amount)
        {
            if (amount <= 0) return;

            totalCoins += amount;
            Debug.Log($"[CurrencyManager] Added {amount} coins → Total: {totalCoins}");

            GameService.Instance.eventManager.OnCoinsChanged.InvokeEvents(totalCoins);
            GameService.Instance.saveManager.SaveCoins(totalCoins);

        }
    }
}
