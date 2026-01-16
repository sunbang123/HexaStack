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
            // 1. 초기 볼륨 세팅 (AudioManager에서 현재 값 가져오기)
            // 브로의 AudioManager에 GetVolume 기능이 있다면 여기서 연동!

            _bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
            _sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

            _closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        }

        private void OnBGMSliderChanged(float value)
        {
            // 전역 AudioManager에 즉시 반영 (마샬링 최소화 호출)
            if (!object.ReferenceEquals(AudioManager.Instance, null))
            {
                AudioManager.Instance.SetVolume(value);
            }
        }

        private void OnSFXSliderChanged(float value)
        {
            // 효과음 볼륨 조절 로직 (필요시 추가)
        }

        // 팝업이 켜질 때마다 호출될 함수
        public void Open()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling(); // UI 최상단으로
        }
    }
}