#region Summary
/// <summary>
/// Manages background music and sound effects (SFX) in the game.
/// Handles volume control and playback for background music and SFX.
/// </summary>
#endregion
using UnityEngine;
using UnityEngine.UI;

namespace SlowpokeStudio.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioSO _audioSettings;
        [SerializeField] private Slider _backgroundVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;

        private AudioSource backgroundMusicSource;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            backgroundMusicSource.clip = _audioSettings.backgroundMusicClip;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.volume = _audioSettings.backgroundMusicVolume;
            backgroundMusicSource.Play();

            if (_backgroundVolumeSlider != null)
            {
                _backgroundVolumeSlider.value = _audioSettings.backgroundMusicVolume;
                _backgroundVolumeSlider.onValueChanged.AddListener(UpdateBackgroundVolume);
            }

            if (_sfxVolumeSlider != null)
            {
                _sfxVolumeSlider.value = _audioSettings.sfxVolume;
                _sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
            }
        }

        #region Audio Methods
        private void UpdateBackgroundVolume(float volume)
        {
            _audioSettings.backgroundMusicVolume = volume;
            backgroundMusicSource.volume = volume;
            Debug.Log($"Background Music Volume set to: {volume}");
        }

        private void UpdateSFXVolume(float volume)
        {
            _audioSettings.sfxVolume = volume;
            Debug.Log($"SFX Volume set to: {volume}");
        }

        internal void PlaySFX(SFXType sfxType)
        {
            AudioClip clip = _audioSettings.GetSFXClip(sfxType);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, _audioSettings.sfxVolume);
            }
            else
            {
                Debug.LogError($"SFX Clip for {sfxType} not found!");
            }
        }

        #endregion
    }
}

