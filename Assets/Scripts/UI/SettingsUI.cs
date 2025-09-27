using UnityEngine;
using UnityEngine.UI;

namespace SlowpokeStudio.UI
{
    public class SettingsUI : UIAbstractClass
    {
        [Header("SettingsPanelReference")]
        [SerializeField] internal GameObject settingsPanel;
        [SerializeField] internal Button homeButton;
        [SerializeField] internal Button settingsQuitButton;
        [SerializeField] internal Button settingsPanelCloseButton;

        private void OnEnable()
        {
            homeButton.onClick.AddListener(OnHomeButton);
            settingsQuitButton.onClick.AddListener(OnQuitButton);
            settingsPanelCloseButton.onClick.AddListener(OnSettingsCloseButton);
        }

        private void OnDisable()
        {
            homeButton.onClick.RemoveListener(OnHomeButton);
            settingsQuitButton.onClick.RemoveListener(OnQuitButton);
            settingsPanelCloseButton.onClick.RemoveListener(OnSettingsCloseButton);
        }

        private void OnHomeButton()
        {
            Debug.Log("On Home Button Active");
        }

        private void OnQuitButton()
        {
            Application.Quit();
        }

        private void OnSettingsCloseButton()
        {
            settingsPanel.SetActive(false);
        }
    }

}
