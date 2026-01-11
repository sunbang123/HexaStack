using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexaStack.Views;
using HexaStack.Controllers;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Controllers.Managers
{
    public class InGameManager : SingletonBehaviour<InGameManager>
    {
        public InGameUIController InGameUIController { get; private set; }
        public MergeController MergeController { get; private set; }
        public StackSpawnerController StackSpawnerController { get; private set; }

        protected override void Init()
        {
            m_IsDestroyOnLoad = true;

            base.Init();
        }

        private void Start()
        {
            InitializeControllers();
        }

        private void InitializeControllers()
        {
            InGameUIController = FindObjectOfType<InGameUIController>();
            if (!InGameUIController)
            {
                Logger.Log("InGameUIController does not exist.");
            }
            else
            {
                InGameUIController.Init();
            }

            MergeController = FindObjectOfType<MergeController>();
            StackSpawnerController = FindObjectOfType<StackSpawnerController>();
        }

        public void GoToLobby()
        {
            if (SceneLoader.Instance == null)
            {
                Logger.LogError("SceneLoader.Instance is null. Creating new SceneLoader...");
                // SceneLoader가 없으면 생성
                GameObject sceneLoaderObj = new GameObject("SceneLoader");
                sceneLoaderObj.AddComponent<SceneLoader>();
            }

            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }
    }
}
