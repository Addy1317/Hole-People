#region Summary
// UIManager is responsible for managing and updating the game's UI elements, including health, wave count, kills, currency, and tower selection.
// It updates the UI when certain events happen, such as enemy deaths or health changes.
#endregion
using SlowpokeStudio.Manager;
using SlowpokeStudio.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlowpokeStudio.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Menu UI Reference")]
        [SerializeField] internal GameObject mainMenuUI;
        [SerializeField] internal Button playButton;
        [SerializeField] internal Button quitButton;

        [Header("Game UI Refenence")]
        [SerializeField] internal GameObject mainGameUI;
        [SerializeField] internal Button replayButton;
        [SerializeField] internal Button settingButton;

        [Header("Level Text")]
        [SerializeField] internal TextMeshProUGUI currentLevelText;

        [Header("SettingsPanelReference")]
        [SerializeField] internal GameObject settingsPanel;
        [SerializeField] internal Button homeButton;
        [SerializeField] internal Button settingsQuitButton;
        [SerializeField] internal Button settingsPanelCloseButton;

        [Header("Level Complete Reference")]
        [SerializeField] internal GameObject levelCompletePanel;
        [SerializeField] internal Button nextLevelButton;

        [Header("Coins Earned Text")]
        [SerializeField] internal TextMeshProUGUI coinsEarnedText;

        [Header("Level Fail Reference")]
        [SerializeField] internal GameObject levelFailPanel;
        [SerializeField] internal Button levelFailReplayButton;
        [SerializeField] internal Button levelFailHomeButton;
        [SerializeField] internal Button LevelFailQuitButton;

        private void Awake()
        {
            mainMenuUI.SetActive(true);
        }

        private void Start()
        {
            GameService.Instance.eventManager.OnLevelCompleteEvent.AddListener(OnLevelCompleteButton);
        }

        private void OnDestroy()
        {
            GameService.Instance.eventManager.OnLevelCompleteEvent.RemoveListener(OnLevelCompleteButton);
        }
        private void OnEnable()
        {
            playButton.onClick.AddListener(OnplayButton);
            quitButton.onClick.AddListener(OnQuitButton);

            replayButton.onClick.AddListener(OnReplayButton);
            settingButton.onClick.AddListener(OnsettingsButton);

            homeButton.onClick.AddListener(OnHomeButton);
            settingsQuitButton.onClick.AddListener(OnSettingsQuitButton);
            settingsPanelCloseButton.onClick.AddListener(OnSettingsCloseButton);

            nextLevelButton.onClick.AddListener(OnLevelCompleteButton);

            levelFailReplayButton.onClick.AddListener(OnLevelFailReplayButton);
            levelFailHomeButton.onClick.AddListener(OnLevelFailHomeButton);
            LevelFailQuitButton.onClick.AddListener(OnLevelFailQuitButton);
        }

        private void OnDisable()
        {
            playButton.onClick.RemoveListener(OnplayButton);
            quitButton.onClick.RemoveListener(OnQuitButton);

            replayButton.onClick.RemoveListener(OnReplayButton);
            settingButton.onClick.RemoveListener(OnsettingsButton);

            homeButton.onClick.RemoveListener(OnHomeButton);
            settingsQuitButton.onClick.RemoveListener(OnSettingsQuitButton);
            settingsPanelCloseButton.onClick.RemoveListener(OnSettingsCloseButton);

            nextLevelButton.onClick.RemoveListener(OnLevelCompleteButton);

            levelFailReplayButton.onClick.RemoveListener(OnLevelFailReplayButton);
            levelFailHomeButton.onClick.RemoveListener(OnLevelFailHomeButton);
            LevelFailQuitButton.onClick.RemoveListener(OnLevelFailQuitButton);

        }

        #region MainMenu UI Buttons Methods

        private void OnplayButton()
        {
            mainMenuUI.SetActive(false);
            Debug.Log("Game Started");
        }

        private void OnQuitButton()
        {
            Application.Quit();
        }

        #endregion

        #region Main Game UI Buttons Methoods
        private void OnReplayButton()
        {
            Debug.Log("Replaying Level");
        }

        private void OnsettingsButton()
        {
            settingsPanel.SetActive(true);
            Debug.Log("Settings Panel Active");
        }
        #endregion

        #region Settings UI Buttons Methods
        private void OnHomeButton()
        {
            mainMenuUI.SetActive(true);
            Debug.Log("On Home Button Active");
        }

        private void OnSettingsQuitButton()
        {
            Application.Quit();
        }

        private void OnSettingsCloseButton()
        {
            settingsPanel.SetActive(false);
        }
        #endregion

        #region Level COmpletion UI Button Method
        private void OnLevelCompleteButton()
        {
            levelCompletePanel.SetActive(true);
            Debug.Log("On Next Level Button Clicked");
        }
        #endregion

        #region Level Fail UI Buttons Methods
        private void OnLevelFailReplayButton()
        {
            levelFailPanel.SetActive(false);
        }

        private void OnLevelFailHomeButton()
        {
            mainMenuUI.SetActive(true);
        }

        private void OnLevelFailQuitButton()
        {
            Application.Quit();
        }
        #endregion
    }
}
