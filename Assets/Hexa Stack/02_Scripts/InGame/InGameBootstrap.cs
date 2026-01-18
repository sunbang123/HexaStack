using UnityEngine;
using HexaStack.Core;
using HexaStack.Controllers.Managers;

namespace HexaStack.Scenes.InGame
{
    public class InGameBootstrap : MonoBehaviour
    {
        [Header("Managers & Controllers")]
        [SerializeField] private InGameManager _inGameManager;

        [SerializeField] private Controllers.GridGeneratorController _gridGenerator;
        [SerializeField] private Controllers.LevelController _levelController;

        private void Awake()
        {
            InitializeScene();
        }

        private void InitializeScene()
        {
            var sceneData = SceneLoader.Instance.GetSceneData<InGameSceneData>();

            if (sceneData != null)
            {
                _levelController.Setup(sceneData.LevelIndex);
            }

            if (!object.ReferenceEquals(_inGameManager, null))
            {
                Core.Logger.Log("[Bootstrap] InGame Scene Initialized Successfully.");
            }
        }
    }
}