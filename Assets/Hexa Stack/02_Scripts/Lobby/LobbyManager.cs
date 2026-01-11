using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexaStack.Views;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Controllers.Managers
{
    public class LobbyManager : SingletonBehaviour<LobbyManager>
    {
        public LobbyUIController LobbyUIController { get; private set; }

        protected override void Init()
        {
            m_IsDestroyOnLoad = true;
            
            base.Init();
        }

        private void Start()
        {
            LobbyUIController = FindObjectOfType<LobbyUIController>();
            if(!LobbyUIController)
            {
                Logger.Log("LobbyUIController does not exist.");
                return;
            }

            LobbyUIController.Init();
        }

        public void StartInGame()
        {
            if (SceneLoader.Instance == null)
            {
                Logger.LogError("SceneLoader.Instance is null. Creating new SceneLoader...");
                // SceneLoader가 없으면 생성
                GameObject sceneLoaderObj = new GameObject("SceneLoader");
                sceneLoaderObj.AddComponent<SceneLoader>();
            }

            SceneLoader.Instance.LoadScene(SceneType.InGame);
        }
    }
}
