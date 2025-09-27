#region Summary
// UIManager is responsible for managing and updating the game's UI elements, including health, wave count, kills, currency, and tower selection.
// It updates the UI when certain events happen, such as enemy deaths or health changes.
#endregion
using SlowpokeStudio.Services;
using SlowpokeStudio.Audio;
using System.Collections;
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

        [Header("All level Complete Reference")]
        [SerializeField] internal GameObject allLevelsCompletedPanel;
        //[SerializeField] internal Button allLevelCompletedReplayButton;
        [SerializeField] internal Button allLevelCompletedHomeButton;
        [SerializeField] internal Button allLevelCompletedQuitButton;

        private void Awake()
        {
            mainMenuUI.SetActive(true);
        }

        private void Start()
        {
            GameService.Instance.eventManager.OnLevelCompleteEvent.AddListener(OnLevelCompleteEvent);
            GameService.Instance.eventManager.OnCoinsChanged.AddListeners(UpdateCoinsUI);
        }

        private void OnDestroy()
        {
            GameService.Instance.eventManager.OnLevelCompleteEvent.RemoveListener(OnLevelCompleteEvent);
            GameService.Instance.eventManager.OnCoinsChanged.RemoveListeners(UpdateCoinsUI);
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

            nextLevelButton.onClick.AddListener(OnNextLevelButton);

            //allLevelCompletedReplayButton.onClick.AddListener(OnAlLevelCompletedReplayButton);
            allLevelCompletedHomeButton.onClick.AddListener(OnAllLevelCompletedHomeButton);
            allLevelCompletedQuitButton.onClick.AddListener(OnAllLevelCompletedQuitButton);
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

            nextLevelButton.onClick.RemoveListener(OnNextLevelButton);

            //allLevelCompletedReplayButton.onClick.RemoveListener(OnAlLevelCompletedReplayButton);
            allLevelCompletedHomeButton.onClick.RemoveListener(OnAllLevelCompletedHomeButton);
            allLevelCompletedQuitButton.onClick.RemoveListener(OnAllLevelCompletedQuitButton);

        }

        #region MainMenu UI Buttons Methods

        private void OnplayButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            mainMenuUI.SetActive(false);
            Debug.Log("Game Started");
        }

        private void OnQuitButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            Application.Quit();
        }

        #endregion

        #region Main Game UI Buttons Methoods
        private void OnReplayButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            Debug.Log("Replaying Level");
            GameService.Instance.levelManager.RestartLevel();
        }

        private void OnsettingsButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            settingsPanel.SetActive(true);
            Debug.Log("Settings Panel Active");
        }
        public void UpdateLevelText(int index, string name)
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            if (currentLevelText == null)
            {
                Debug.LogWarning("[UIManager] Level Text reference missing!");
                return;
            }

            // Example formatting: "Level 1 - Tutorial"
            currentLevelText.text = $"Level : {name}";
        }
        private void UpdateCoinsUI(int newTotal)
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);

            coinsEarnedText.text = $"Coins: {newTotal}";
        }
        #endregion

        #region Settings UI Buttons Methods
        private void OnHomeButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            mainMenuUI.SetActive(true);
            Debug.Log("On Home Button Active");
        }

        private void OnSettingsQuitButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            Application.Quit();
        }

        private void OnSettingsCloseButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            settingsPanel.SetActive(false);
        }
        #endregion

        #region Level Completion UI Button Method
        private void OnLevelCompleteEvent()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnLevelCompleteSFX);
            nextLevelButton.gameObject.SetActive(false);
            levelCompletePanel.SetActive(true);
            GameService.Instance.levelManager.LoadNextLevel();

            StartCoroutine(NextButtonVisible());
            Debug.Log("On Next Level Button Clicked");
        }
        private IEnumerator NextButtonVisible()
        {
            yield return new WaitForSeconds(1f);
            nextLevelButton.gameObject.SetActive(true);
        }
        private void OnNextLevelButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            Debug.Log("Trigger Next Level");
            levelCompletePanel.SetActive(false);
        }

        #endregion

        #region All Level Completed UI Buttons Methods
        public void OnAllLevelCompleted()
        {
            Debug.Log("[UIManager] Showing All Levels Completed Panel");
            allLevelsCompletedPanel.SetActive(true);
        }

        private void OnAlLevelCompletedReplayButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            allLevelsCompletedPanel.SetActive(false);
        }

        private void OnAllLevelCompletedHomeButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            mainMenuUI.SetActive(true);
            allLevelsCompletedPanel.SetActive(false);
            Debug.Log("[UIManager] Switched to Main Menu UI");
            if (GameService.Instance.saveManager != null)
            {
                GameService.Instance.saveManager.ClearAllData();
            }
            else
            {
                // Fallback if you're using PlayerPrefs directly
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("[LevelManager] All PlayerPrefs cleared!");
            }
        }

        private void OnAllLevelCompletedQuitButton()
        {
            GameService.Instance.audioManager.PlaySFX(SFXType.OnButtonClickSFX);
            Application.Quit();
        }
        #endregion
    }
}
