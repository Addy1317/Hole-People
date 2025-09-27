using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlowpokeStudio.UI
{
    public class LevelCompleteUI : UIAbstractClass
    {
        [Header("Level Complete Reference")]
        [SerializeField] internal GameObject levelCompletePanel;
        [SerializeField] internal Button nextLevelButton;

        [Header("Coins Earned Text")]
        [SerializeField] internal TextMeshProUGUI coinsEarnedText;

        private void OnEnable()
        {
            nextLevelButton.onClick.AddListener(OnNextLevelButton);
        }

        private void OnDisable()
        {
            nextLevelButton.onClick.RemoveListener(OnNextLevelButton);
        }

        private void OnNextLevelButton()
        {
            Debug.Log("On Next Level Button Clicked");
        }
    }
}
