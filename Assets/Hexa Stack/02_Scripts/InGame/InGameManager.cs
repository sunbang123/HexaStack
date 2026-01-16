using UnityEngine;
using HexaStack.Views;
using HexaStack.Controllers;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Controllers.Managers
{
    public class InGameManager : SingletonBehaviour<InGameManager>
    {
        [Header("Scene Controllers (Drag & Drop)")]
        [SerializeField] private InGameUIController _inGameUI;
        [SerializeField] private MergeController _mergeController;
        [SerializeField] private StackSpawnerController _stackSpawner;

        public InGameUIController InGameUIController => _inGameUI;
        public MergeController MergeController => _mergeController;
        public StackSpawnerController StackSpawnerController => _stackSpawner;

        protected override void Init()
        {
            m_IsDestroyOnLoad = false; 
            base.Init();
        }

        private void Start()
        {
            InitializeControllers();
        }

        private void InitializeControllers()
        {
            if (!object.ReferenceEquals(_inGameUI, null))
            {
                _inGameUI.Init();
            }
            else
            {
                Logger.LogWarning("InGameUIController is missing in Inspector.");
            }

            var audio = AudioManager.Instance;
            if (!object.ReferenceEquals(audio, null))
            {
                audio.PlayBGM(BGM.InGame);
            }
        }
    }
}