using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HexaStack.Core;


namespace HexaStack.Views
{
    public class NoticePopup : BaseUI
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        public void Setup(string message, System.Action onConfirm = null, System.Action onCancel = null)
        {
            _messageText.text = message;

            _confirmButton.onClick.RemoveAllListeners();
            _confirmButton.onClick.AddListener(() =>
            {
                onConfirm?.Invoke();
                gameObject.SetActive(false);
            });

            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.AddListener(() =>
            {
                onCancel?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }
}