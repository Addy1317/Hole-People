using UnityEngine;
using UnityEngine.UI;

namespace SlowpokeStudio.UI
{
    public class LevelFailUI : UIAbstractClass
    {
        [Header("Level Fail Reference")]
        [SerializeField] internal GameObject levelFailPanel;
        [SerializeField] internal Button levelFailReplayButton;
        [SerializeField] internal Button levelFailHomeButton;
        [SerializeField] internal Button LevelFailQuitButton;

        private void OnEnable()
        {
            levelFailReplayButton.onClick.AddListener(OnLevelFailReplayButton);
            levelFailHomeButton.onClick.AddListener(OnLevelFailHomeButton);
            LevelFailQuitButton.onClick.AddListener(OnLevelFailQuitButton);
        }

        private void OnDisable()
        {
            levelFailReplayButton.onClick.RemoveListener(OnLevelFailReplayButton);
            levelFailHomeButton.onClick.RemoveListener(OnLevelFailHomeButton);
            LevelFailQuitButton.onClick.RemoveListener(OnLevelFailQuitButton);
        }

        private void OnLevelFailReplayButton()
        {

        }

        private void OnLevelFailHomeButton()
        {

        }

        private void OnLevelFailQuitButton()
        {
            Application.Quit();
        }
    }
}
