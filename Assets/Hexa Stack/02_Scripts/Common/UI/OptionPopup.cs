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
        private string PRIVACY_POLICY_URL = "https://south-comic-1a2.notion.site/Cosmic-Hexa-Puzzle-2f6068d6f71e80b88f37d5ce122a9358?pvs=143";

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

        public void OnClickPrivacyPolicyURL()
        {
            Core.Logger.Log($"{GetType()}::OnClickPrivacyPolicyURL");
            Application.OpenURL(PRIVACY_POLICY_URL);
        }
    }
}