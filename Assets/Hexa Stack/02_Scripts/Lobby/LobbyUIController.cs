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

        [Header("Local UI Prefabs")]
        [SerializeField] private ProfilePopup _profilePrefab;

        [Header("Local UI Prefabs")]
        [SerializeField] private RankPopup _rankPrefab;

        public void Init()
        {
            if (_jellyController != null)
            {
                _jellyController.OnProgressUpdated += UpdateProgressText;
            }

            if (UIManager.Instance != null && _archivePrefab != null)
            {
                UIManager.Instance.RegisterPrefab<ArchivePopup>(_archivePrefab);
            }

            if (UIManager.Instance != null && _profilePrefab != null)
            {
                UIManager.Instance.RegisterPrefab<ProfilePopup>(_profilePrefab);
            }

            if (UIManager.Instance != null && _rankPrefab != null)
            {
                UIManager.Instance.RegisterPrefab<RankPopup>(_rankPrefab);
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
                UIManager.Instance.OpenUI<ArchivePopup>(null);
            }
        }

        public void OnClickProfileBtn()
        {
            Logger.Log($"{GetType()}::OnClickAchieveBtn");

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                UIManager.Instance.OpenUI<ProfilePopup>(null);
            }
        }
        public void OnClickRankBtn()
        {
            Logger.Log($"{GetType()}::OnClickAchieveBtn");

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                UIManager.Instance.OpenUI<RankPopup>(null);
            }
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
