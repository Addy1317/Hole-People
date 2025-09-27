using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlowpokeStudio.UI
{
    public class GameUI : UIAbstractClass
    {
        [Header("Game UI Refenence")]
        [SerializeField] internal GameObject gameUI;
        [SerializeField] internal Button replayButton;
        [SerializeField] internal Button settingButton;

        [Header("Level Text")]
        [SerializeField] internal TextMeshProUGUI currentLevelText;

        private void OnEnable()
        {
            replayButton.onClick.AddListener(OnReplayButton);
            settingButton.onClick.AddListener(OnsettingsButton);
        }

        private void OnDisable()
        {
            replayButton.onClick.RemoveListener(OnReplayButton);
            settingButton.onClick.RemoveListener(OnsettingsButton);
        }

        private void OnReplayButton()
        {
            Debug.Log("Replaying Level");
        }

        private void OnsettingsButton()
        {
            Debug.Log("Settings Panel Active");
        }
    }
}
