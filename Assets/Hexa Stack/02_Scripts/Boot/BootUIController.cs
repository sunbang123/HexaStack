using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Views
{
    public class BootUIController : MonoBehaviour
    {
        [Header("Loading Bar")]
        [SerializeField] private Slider loadingBar;
        [SerializeField] private Image loadingBarFillImage; // Slider 대신 Image 사용 시
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private TextMeshProUGUI loadingPercentText;

        [Header("Settings")]
        [SerializeField] private string[] loadingMessages = {
            "게임을 로딩하는 중...",
            "자원을 불러오는 중...",
            "씬을 준비하는 중..."
        };
        [SerializeField] private float messageChangeInterval = 1f; // 메시지 변경 간격 (초)

        private float currentProgress = 0f;
        private float messageTimer = 0f;
        private int currentMessageIndex = 0;

        public void Init()
        {
            // 초기화
            currentProgress = 0f;
            messageTimer = 0f;
            currentMessageIndex = 0;

            // UI 초기 상태 설정
            if (loadingBar != null)
            {
                loadingBar.value = 0f;
            }

            if (loadingBarFillImage != null)
            {
                loadingBarFillImage.fillAmount = 0f;
            }

            if (loadingText != null && loadingMessages.Length > 0)
            {
                loadingText.text = loadingMessages[0];
            }

            if (loadingPercentText != null)
            {
                loadingPercentText.text = "0%";
            }

            Logger.Log("BootUIController initialized.");
        }

        private void Update()
        {
            // 로딩 메시지 자동 변경
            UpdateLoadingMessage();
        }

        /// <summary>
        /// 로딩 진행률을 업데이트합니다.
        /// </summary>
        /// <param name="progress">0.0 ~ 1.0 사이의 진행률</param>
        public void UpdateLoadingProgress(float progress)
        {
            currentProgress = Mathf.Clamp01(progress);

            // Slider 업데이트
            if (loadingBar != null)
            {
                loadingBar.value = currentProgress;
            }

            // Image FillAmount 업데이트 (Slider 대신 Image를 사용하는 경우)
            if (loadingBarFillImage != null)
            {
                loadingBarFillImage.fillAmount = currentProgress;
            }

            // 퍼센트 텍스트 업데이트
            if (loadingPercentText != null)
            {
                loadingPercentText.text = $"{Mathf.RoundToInt(currentProgress * 100f)}%";
            }
        }

        /// <summary>
        /// 로딩 메시지를 자동으로 변경합니다.
        /// </summary>
        private void UpdateLoadingMessage()
        {
            if (loadingMessages.Length == 0 || loadingText == null)
                return;

            messageTimer += Time.deltaTime;

            if (messageTimer >= messageChangeInterval)
            {
                messageTimer = 0f;
                currentMessageIndex = (currentMessageIndex + 1) % loadingMessages.Length;
                loadingText.text = loadingMessages[currentMessageIndex];
            }
        }

        /// <summary>
        /// 로딩 메시지를 수동으로 설정합니다.
        /// </summary>
        public void SetLoadingMessage(string message)
        {
            if (loadingText != null)
            {
                loadingText.text = message;
            }
        }
    }
}
