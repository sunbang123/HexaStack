using UnityEngine;
using TMPro; // 텍스트 제어용
using UnityEngine.UI;
using HexaStack.Core;


namespace HexaStack.Views
{
    public class NoticePopup : BaseUI
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _confirmButton;

        // 팝업을 띄우면서 메시지를 세팅하는 입구
        public void Setup(string message, System.Action onConfirm = null)
        {
            _messageText.text = message;

            // 확인 버튼 누르면 닫히게 + 추가 로직(Action) 실행
            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}