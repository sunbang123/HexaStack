using UnityEngine;
using HexaStack.Views;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Controllers.Managers
{
    public class LobbyManager : SingletonBehaviour<LobbyManager>
    {
        [Header("Scene Components")]
        [SerializeField] private LobbyUIController _lobbyUIController;

        protected override void Init()
        {
            m_IsDestroyOnLoad = true; 
            base.Init();
        }

        private void Start()
        {
            if (!object.ReferenceEquals(_lobbyUIController, null))
            {
                _lobbyUIController.Init();
            }
            else
            {
                Logger.LogWarning("LobbyUIController is not assigned in Inspector.");
            }

            if (object.ReferenceEquals(SceneLoader.Instance, null))
            {
                Logger.LogError("SceneLoader가 없습니다.");
            }

            var audio = AudioManager.Instance;
            if (!object.ReferenceEquals(audio, null))
            {
                audio.PlayBGM(BGM.Lobby);
            }
        }
    }
}