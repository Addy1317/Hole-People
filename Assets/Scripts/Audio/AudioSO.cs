using UnityEngine;

namespace SlowpokeStudio
{
    public enum SFXType
    {
        OnButtonClickSFX,
        OnHoleClickSFX,
        OnCharaceterReachedToHoleSFX,
        OnLevelCompleteSFX
         
    }

    [CreateAssetMenu(fileName = "AudioSO", menuName = "Audio/AudioSO")]
    public class AudioSO : ScriptableObject
    {
        [System.Serializable]
        public struct SFXAudio
        {
            public SFXType sfxType;
            public AudioClip audioClip;
        }

        [Header("Background Music")]
        [SerializeField] internal AudioClip backgroundMusicClip;
        [SerializeField][Range(0f, 1f)] internal float backgroundMusicVolume = 0.5f;

        [Header("SFX")]
        [SerializeField] internal SFXAudio[] sfxClips;
        [SerializeField][Range(0f, 1f)] internal float sfxVolume = 0.5f;

        internal AudioClip GetSFXClip(SFXType sfxType)
        {
            foreach (var sfx in sfxClips)
            {
                if (sfx.sfxType == sfxType)
                {
                    return sfx.audioClip;
                }
            }

            Debug.LogWarning($"SFX clip for {sfxType} not found. Returning null.");
            return null;
        }
    }
}

