using UnityEngine;
using UnityEngine.SceneManagement;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Core
{
    public enum SceneType
    {
        Boot,
        Lobby,
        InGame,
    }

    public class SceneLoader : SingletonBehaviour<SceneLoader>
    {
        protected override void Init()
        {
            m_IsDestroyOnLoad = false; // 씬 전환 시에도 유지
            base.Init();
        }

        public void LoadScene(SceneType sceneType)
        {
            Logger.Log($"{sceneType} scene loading...");

            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneType.ToString());
        }

        public void ReloadScene()
        {
            Logger.Log($"{SceneManager.GetActiveScene().name} scene loading...");

            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// 비동기로 씬을 로드합니다. 로딩 진행률을 반환합니다.
        /// </summary>
        public AsyncOperation LoadSceneAsync(SceneType sceneType)
        {
            Logger.Log($"{sceneType} scene loading asynchronously...");

            Time.timeScale = 1f;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneType.ToString());
            asyncOperation.allowSceneActivation = false; // 자동 활성화 방지
            
            return asyncOperation;
        }

        /// <summary>
        /// 비동기 씬 로딩을 활성화합니다.
        /// </summary>
        public void ActivateScene(AsyncOperation asyncOperation)
        {
            if (asyncOperation != null && !asyncOperation.isDone)
            {
                asyncOperation.allowSceneActivation = true;
            }
        }
    }
}
