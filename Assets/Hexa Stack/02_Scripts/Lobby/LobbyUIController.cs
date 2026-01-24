using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HexaStack.Controllers.Managers;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Views
{
    public class LobbyUIController : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _fillPercentText;

        [Header("References")]
        [SerializeField] private JellyFillController _jellyController;

        [Header("Local UI Prefabs")]
        [SerializeField] private ArchivePopup _archivePrefab;
        
        public void Init()
        {
            if (_jellyController != null)
            {
                _jellyController.OnProgressUpdated += UpdateProgressText;
            }
            // [수정] 여기가 핵심! 로비 씬에 들어왔을 때 "이거 등록해줘"라고 UIManager에게 요청
            if (UIManager.Instance != null && _archivePrefab != null)
            {
                UIManager.Instance.RegisterPrefab<ArchivePopup>(_archivePrefab);
            }
        }

        private void OnDestroy()
        {
            if (_jellyController != null)
            {
                _jellyController.OnProgressUpdated -= UpdateProgressText;
            }
        }

        private void UpdateProgressText(float progress)
        {
            if (_fillPercentText != null)
            {
                _fillPercentText.text = $"{progress * 100:F0}%";
            }
        }
        public void OnClickSettingsBtn()
        {
            Logger.Log($"{GetType()}::OnClickSettingsBtn");

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                UIManager.Instance.OpenUI<OptionPopup>(null);
            }
            else
            {
                Logger.LogError("UIManager Instance를 찾을 수 없습니다!");
            }
        }

        public void OnClickArchiveBtn()
        {
            Logger.Log($"{GetType()}::OnClickAchieveBtn");

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                // 등록은 Init에서 했으니, 여기선 열기만 하면 됨!
                UIManager.Instance.OpenUI<ArchivePopup>(null);
            }
        }

        public void OnClickProfileBtn()
        {
            Logger.Log($"{GetType()}::OnClickProfileBtn");
        }

        public void OnClickCurrChapter()
        {
            Logger.Log($"{GetType()}::OnClickCurrChapter");
        }

        public void OnClickStartBtn()
        {
            Logger.Log($"{GetType()}::OnClickStartBtn");

            if (!System.Object.ReferenceEquals(SceneLoader.Instance, null))
            {
                var startData = new InGameSceneData(1, false);
                SceneLoader.Instance.LoadScene(SceneType.InGame, startData);
            }
        }
    }
}
