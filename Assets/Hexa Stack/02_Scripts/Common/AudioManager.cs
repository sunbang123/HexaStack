using UnityEngine;
using HexaStack.Core;
using System.Collections;

namespace HexaStack.Core
{
    // [중요] 배열 인덱스와 Enum 값이 1:1로 매칭되어야 함!
    public enum BGM
    {
        Lobby = 0,
        InGame = 1,
        // 필요하면 여기에 추가 (예: Boss, Ending...)
        COUNT
    }

    public enum SFX
    {
        ChapterClear = 0,
        StageClear = 1,
        UIButtonClick = 2,
        // 필요하면 여기에 추가
        COUNT
    }

    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        [Header("Audio Sources (Drag & Drop)")]
        [Tooltip("배경음악용 오디오 소스 (Loop 가능)")]
        [SerializeField] private AudioSource m_BGMSource;

        [Tooltip("효과음용 오디오 소스 (Loop 불가)")]
        [SerializeField] private AudioSource m_SFXSource;

        [Header("Audio Clips Lookup Table")]
        [Tooltip("Enum 순서대로 클립을 배치하세요.")]
        [NamedArray(typeof(BGM))]
        [SerializeField] private AudioClip[] m_BGMClips;

        [Tooltip("Enum 순서대로 클립을 배치하세요.")]
        [NamedArray(typeof(SFX))]
        [SerializeField] private AudioClip[] m_SFXClips;

        protected override void Init()
        {
            base.Init();

            // [안정성 체크] 개발자가 인스펙터에 할당 안 했을 때 경고
            if (m_BGMSource == null || m_SFXSource == null)
            {
                Logger.LogError("[SoundManager] AudioSources are missing! Please assign them in Inspector.");
            }

            // 전역 싱글톤이므로 씬 전환 시에도 유지
            m_IsDestroyOnLoad = false;
        }

        /// <summary>
        /// BGM 볼륨 (0.0 ~ 1.0) 읽기/쓰기
        /// </summary>
        public float BGMVolume
        {
            get => m_BGMSource.volume;
            set => m_BGMSource.volume = Mathf.Clamp01(value);
        }

        /// <summary>
        /// SFX 볼륨 (0.0 ~ 1.0) 읽기/쓰기
        /// </summary>
        public float SFXVolume
        {
            get => m_SFXSource.volume;
            set => m_SFXSource.volume = Mathf.Clamp01(value);
        }

        #region BGM Logic
        /// <summary>
        /// 배경음악 재생 (인덱스 기반, O(1))
        /// </summary>
        public void PlayBGM(BGM bgm)
        {
            int index = (int)bgm;

            // 1. 배열 범위 체크 코드
            if (index < 0 || index >= m_BGMClips.Length)
            {
                Logger.LogError($"[SoundManager] Missing BGM Clip for index: {index} ({bgm})");
                return;
            }

            AudioClip targetClip = m_BGMClips[index];

            // 2. 이미 같은 음악이 재생 중이면 스킵 (불필요한 재시작 방지)
            if (m_BGMSource.isPlaying && m_BGMSource.clip == targetClip)
                return;

            m_BGMSource.clip = targetClip;
            m_BGMSource.loop = true;
            m_BGMSource.Play();
        }

        public void StopBGM()
        {
            m_BGMSource.Stop();
        }

        public void PauseBGM()
        {
            m_BGMSource.Pause();
        }

        public void ResumeBGM()
        {
            m_BGMSource.UnPause();
        }
        #endregion

        #region SFX Logic
        /// <summary>
        /// 효과음 재생 (PlayOneShot 방식 - 소스 1개로 여러 효과음 동시 재생 가능)
        /// </summary>
        public void PlaySFX(SFX sfx)
        {
            int index = (int)sfx;

            // 1. 배열 범위 체크 코드
            if (index < 0 || index >= m_SFXClips.Length)
            {
                Logger.LogError($"[SoundManager] Missing SFX Clip for index: {index} ({sfx})");
                return;
            }

            // 2. PlayOneShot: 효과음이 겹쳐도 자연스럽게 재생 가능
            // 필요하면 AudioSource 여러 개(New GameObject)로 확장.
            m_SFXSource.PlayOneShot(m_SFXClips[index]);
        }
        #endregion

        #region Volume Control
        /// <summary>
        /// 전체 음소거 (토글 방식)
        /// </summary>
        public void ToggleMute()
        {
            bool isMuted = !m_BGMSource.mute;
            SetMute(isMuted);
        }

        public void SetMute(bool isMute)
        {
            m_BGMSource.mute = isMute;
            m_SFXSource.mute = isMute;
        }

        // (선택 사항) 더 세밀한 제어가 필요하면 AudioMixer로 확장하거나,
        // 간단하게는 이런 방식으로 제어.
        public void SetVolume(float volume) // 0.0f ~ 1.0f
        {
            m_BGMSource.volume = volume;
            m_SFXSource.volume = volume;
        }
        #endregion
    }
}
