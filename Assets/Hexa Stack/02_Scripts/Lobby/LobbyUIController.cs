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

            // Local UI 등록: Lobby 씬에서만 사용하는 UI
            if (UIManager.Instance != null && _archivePrefab != null)
            {
                UIManager.Instance.RegisterPrefabLocal<ArchivePopup>(_archivePrefab);
            }

            if (UIManager.Instance != null && _profilePrefab != null)
            {
                UIManager.Instance.RegisterPrefabLocal<ProfilePopup>(_profilePrefab);
            }

            if (UIManager.Instance != null && _rankPrefab != null)
            {
                UIManager.Instance.RegisterPrefabLocal<RankPopup>(_rankPrefab);
                // RankPopup을 Prewarm하여 미리 생성
                UIManager.Instance.Prewarm<RankPopup>();
            }
        }

        private void OnDestroy()
        {
            if (_jellyController != null)
            {
                _jellyController.OnProgressUpdated -= UpdateProgressText;
            }

            // Local UI 해제는 SceneLoader에서 자동으로 처리되지만,
            // 안전을 위해 여기서도 호출 가능 (중복 호출은 안전함)
            // 주석 처리: SceneLoader에서 이미 처리하므로 불필요
            // if (!object.ReferenceEquals(UIManager.Instance, null))
            // {
            //     UIManager.Instance.UnregisterLocalUIs();
            // }
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

            var uiData = new BaseUIData();

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                UIManager.Instance.OpenUI<OptionPopup>(uiData);
            }
            else
            {
                Logger.LogError("UIManager Instance를 찾을 수 없습니다!");
            }
        }

        public void OnClickArchiveBtn()
        {
            Logger.Log($"{GetType()}::OnClickAchieveBtn");

            var uiData = new BaseUIData();

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                UIManager.Instance.OpenUI<ArchivePopup>(uiData);
            }
        }

        public void OnClickProfileBtn()
        {
            Logger.Log($"{GetType()}::OnClickAchieveBtn");

            var uiData = new BaseUIData();

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                UIManager.Instance.OpenUI<ProfilePopup>(uiData);
            }
        }
        public void OnClickRankBtn()
        {
            Logger.Log($"{GetType()}::OnClickAchieveBtn");

            var uiData = new BaseUIData();

            if (!object.ReferenceEquals(UIManager.Instance, null))
            {
                UIManager.Instance.OpenUI<RankPopup>(uiData);
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
