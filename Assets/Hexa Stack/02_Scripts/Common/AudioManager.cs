using UnityEngine;
using HexaStack.Core; // SingletonBehaviour�� �ִ� ���ӽ����̽�
using System.Collections;

namespace HexaStack.Core
{
    // [�߿�] �ν����� �迭 ������ �� Enum ������ 1:1�� ��ġ�ؾ� ��!
    public enum BGM
    {
        Lobby = 0,
        InGame = 1,
        // �ʿ��ϸ� ���⿡ �߰� (��: Boss, Ending...)
        COUNT
    }

    public enum SFX
    {
        ChapterClear = 0,
        StageClear = 1,
        UIButtonClick = 2,
        // �ʿ��ϸ� ���⿡ �߰�
        COUNT
    }

    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        [Header("Audio Sources (Drag & Drop)")]
        [Tooltip("������ǿ� ����� �ҽ� (Loop �ѱ�)")]
        [SerializeField] private AudioSource m_BGMSource;

        [Tooltip("ȿ������ ����� �ҽ� (Loop ����)")]
        [SerializeField] private AudioSource m_SFXSource;

        [Header("Audio Clips Lookup Table")]
        [Tooltip("Enum ������� Ŭ���� ��������.")]
        [NamedArray(typeof(BGM))]
        [SerializeField] private AudioClip[] m_BGMClips;

        [Tooltip("Enum ������� Ŭ���� ��������.")]
        [NamedArray(typeof(SFX))]
        [SerializeField] private AudioClip[] m_SFXClips;

        protected override void Init()
        {
            base.Init();

            // [������ üũ] �����ڰ� �ν����� ���� ��Ծ��� �� ���
            if (m_BGMSource == null || m_SFXSource == null)
            {
                Logger.LogError("[SoundManager] AudioSources are missing! Please assign them in Inspector.");
            }

            // ���� �Ŵ����̹Ƿ� �� ��ȯ �� �ı����� ����
            m_IsDestroyOnLoad = false;
        }

        /// <summary>
        /// BGM ���� (0.0 ~ 1.0) �б�/����
        /// </summary>
        public float BGMVolume
        {
            get => m_BGMSource.volume;
            set => m_BGMSource.volume = Mathf.Clamp01(value);
        }

        /// <summary>
        /// SFX ���� (0.0 ~ 1.0) �б�/����
        /// </summary>
        public float SFXVolume
        {
            get => m_SFXSource.volume;
            set => m_SFXSource.volume = Mathf.Clamp01(value);
        }

        #region BGM Logic
        /// <summary>
        /// ������� ��� (�ε��� ���, O(1))
        /// </summary>
        public void PlayBGM(BGM bgm)
        {
            int index = (int)bgm;

            // 1. �迭 ���� ��� �ڵ�
            if (index < 0 || index >= m_BGMClips.Length)
            {
                Logger.LogError($"[SoundManager] Missing BGM Clip for index: {index} ({bgm})");
                return;
            }

            AudioClip targetClip = m_BGMClips[index];

            // 2. �̹� ���� �뷡�� ������ �ִٸ� ���� (���ʿ��� ���� ����)
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
        /// ȿ���� ��� (PlayOneShot ��� - �ҽ� 1���� ��ø ��� ����)
        /// </summary>
        public void PlaySFX(SFX sfx)
        {
            int index = (int)sfx;

            // 1. �迭 ���� ��� �ڵ�
            if (index < 0 || index >= m_SFXClips.Length)
            {
                Logger.LogError($"[SoundManager] Missing SFX Clip for index: {index} ({sfx})");
                return;
            }

            // 2. PlayOneShot: ȿ������ ���ĵ� ������ �ʰ� �ڿ������� ���� ����
            // ������ AudioSource ���� ���(New GameObject)�� ����.
            m_SFXSource.PlayOneShot(m_SFXClips[index]);
        }
        #endregion

        #region Volume Control
        /// <summary>
        /// ��ü ���Ұ� (��� ���)
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

        // (���� ����) ���� ���� ����� �ʿ��ϴٸ� AudioMixer�� ��õ������,
        // �����ϰԴ� �̷��� ���� ����.
        public void SetVolume(float volume) // 0.0f ~ 1.0f
        {
            m_BGMSource.volume = volume;
            m_SFXSource.volume = volume;
        }
        #endregion
    }
}