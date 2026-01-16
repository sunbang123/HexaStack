using UnityEngine;
using HexaStack.Views;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Controllers.Managers
{
    // 로비에서만 살면 되니까 가볍게 가자!
    public class LobbyManager : MonoBehaviour
    {
        [Header("Scene Components")]
        // [변경] Find 대신 직접 연결! (마샬링 0)
        [SerializeField] private LobbyUIController _lobbyUIController;

        private void Start()
        {
            // 1. UI 초기화 (ReferenceEquals로 안전하게 체크)
            if (!object.ReferenceEquals(_lobbyUIController, null))
            {
                _lobbyUIController.Init();
            }
            else
            {
                Logger.LogWarning("LobbyUIController is not assigned in Inspector.");
            }

            // 전역 매니저들이 잘 있는지 체크 (개발용 로그)
            if (object.ReferenceEquals(SceneLoader.Instance, null))
            {
                Logger.LogError("SceneLoader가 씬에 없습니다! BootScene부터 시작했나요?");
            }
        }
    }
}