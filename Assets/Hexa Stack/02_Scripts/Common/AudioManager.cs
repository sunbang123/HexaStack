using UnityEngine;
using HexaStack.Core; // SingletonBehaviour가 있는 네임스페이스
using System.Collections;

namespace HexaStack.Core
{
    // [중요] 인스펙터 배열 순서와 이 Enum 순서가 1:1로 일치해야 함!
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
        [Tooltip("배경음악용 오디오 소스 (Loop 켜기)")]
        [SerializeField] private AudioSource m_BGMSource;

        [Tooltip("효과음용 오디오 소스 (Loop 끄기)")]
        [SerializeField] private AudioSource m_SFXSource;

        [Header("Audio Clips Lookup Table")]
        [Tooltip("Enum 순서대로 클립을 넣으세요.")]
        [NamedArray(typeof(BGM))]
        [SerializeField] private AudioClip[] m_BGMClips;

        [Tooltip("Enum 순서대로 클립을 넣으세요.")]
        [NamedArray(typeof(SFX))]
        [SerializeField] private AudioClip[] m_SFXClips;

        protected override void Init()
        {
            base.Init();

            // [안정성 체크] 개발자가 인스펙터 연결 까먹었을 때 경고
            if (m_BGMSource == null || m_SFXSource == null)
            {
                Logger.LogError("[SoundManager] AudioSources are missing! Please assign them in Inspector.");
            }

            // 전역 매니저이므로 씬 전환 시 파괴되지 않음
            m_IsDestroyOnLoad = false;
        }

        #region BGM Logic
        /// <summary>
        /// 배경음악 재생 (인덱스 기반, O(1))
        /// </summary>
        public void PlayBGM(BGM bgm)
        {
            int index = (int)bgm;

            // 1. 배열 범위 방어 코드
            if (index < 0 || index >= m_BGMClips.Length)
            {
                Logger.LogError($"[SoundManager] Missing BGM Clip for index: {index} ({bgm})");
                return;
            }

            AudioClip targetClip = m_BGMClips[index];

            // 2. 이미 같은 노래가 나오고 있다면 무시 (불필요한 리셋 방지)
            if (m_BGMSource.isPlaying && m_BGMSource.clip == targetClip)
                return;

            m_BGMSource.clip = targetClip;
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
        /// 효과음 재생 (PlayOneShot 사용 - 소스 1개로 중첩 재생 가능)
        /// </summary>
        public void PlaySFX(SFX sfx)
        {
            int index = (int)sfx;

            // 1. 배열 범위 방어 코드
            if (index < 0 || index >= m_SFXClips.Length)
            {
                Logger.LogError($"[SoundManager] Missing SFX Clip for index: {index} ({sfx})");
                return;
            }

            // 2. PlayOneShot: 효과음이 겹쳐도 끊기지 않고 자연스럽게 섞여 나옴
            // 별도의 AudioSource 생성 비용(New GameObject)이 없음.
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

        // (선택 사항) 볼륨 조절 기능이 필요하다면 AudioMixer를 추천하지만,
        // 간단하게는 이렇게 구현 가능.
        public void SetVolume(float volume) // 0.0f ~ 1.0f
        {
            m_BGMSource.volume = volume;
            m_SFXSource.volume = volume;
        }
        #endregion
    }
}