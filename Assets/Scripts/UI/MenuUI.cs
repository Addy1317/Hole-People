using UnityEngine;
using UnityEngine.UI;

namespace SlowpokeStudio.UI
{
    public class MenuUI : UIAbstractClass
    {
        [Header("Menu UI Reference")]
        [SerializeField] internal GameObject menuUI;
        [SerializeField] internal Button playButton;
        [SerializeField] internal Button quitButton;

        private void Awake()
        {
           // menuUI.SetActive(true);
        }

        private void OnEnable()
        {
            playButton.onClick.AddListener(OnplayButton);
            quitButton.onClick.AddListener(OnQuitButton);
        }

        private void OnDisable()
        {
            playButton.onClick.RemoveListener(OnplayButton);
            quitButton.onClick.RemoveListener(OnQuitButton);
        }

        #region MainMenu UI Buttons Methods

        private void OnplayButton()
        {
            menuUI.SetActive(false);
            Debug.Log("Game Started");
        }

        private void OnQuitButton()
        {
            Application.Quit();
        }

        #endregion
    }
}
