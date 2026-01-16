using UnityEngine;
using HexaStack.Core;
using HexaStack.Views;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Scenes.Loading
{
    public class LoadingBootstrap : MonoBehaviour
    {
        [Header("Local Components")]
        [Tooltip("로딩 씬에 배치된 UI 컨트롤러")]
        [SerializeField] private LoadingUIController _loadingUI;

        private void Start()
        {
            if (_loadingUI != null)
            {
                _loadingUI.Init();
                _loadingUI.SetLoadingMessage("Loading...");
            }
            if (!object.ReferenceEquals(SceneLoader.Instance, null))
            {
                SceneLoader.Instance.RegisterLoadingScreen(this);
            }
            else
            {
                Logger.LogWarning("[LoadingBootstrap] SceneLoader not found. Running in test mode.");
            }
        }
        public void UpdateProgress(float progress)
        {
            if (_loadingUI != null) _loadingUI.UpdateLoadingProgress(progress);
        }

        public void SetMessage(string msg)
        {
            if (_loadingUI != null) _loadingUI.SetLoadingMessage(msg);
        }
    }
}