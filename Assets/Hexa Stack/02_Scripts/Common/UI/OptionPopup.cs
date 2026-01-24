using UnityEngine;
using UnityEngine.UI;
using HexaStack.Core;

namespace HexaStack.Views
{
    public class OptionPopup : BaseUI
    {
        [Header("Audio Sliders")]
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private Slider _sfxSlider;

        [Header("Buttons")]
        [SerializeField] private Button _closeButton;

        private void Start()
        {
            _bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
            _sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

            _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }
        public override void ShowUI()
        {
            if (!object.ReferenceEquals(AudioManager.Instance, null))
            {
                _bgmSlider.value = AudioManager.Instance.BGMVolume;
                _sfxSlider.value = AudioManager.Instance.SFXVolume;
            }

            base.ShowUI();
        }

        private void OnBGMSliderChanged(float value)
        {
            if (!object.ReferenceEquals(AudioManager.Instance, null))
            {
                AudioManager.Instance.BGMVolume = value;
            }
        }

        private void OnSFXSliderChanged(float value)
        {
            if (!object.ReferenceEquals(AudioManager.Instance, null))
            {
                AudioManager.Instance.SFXVolume = value;
            }
        }
    }
}