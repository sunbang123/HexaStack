using UnityEngine;
using UnityEngine.SceneManagement;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;
using HexaStack.Scenes.Loading;
using System.Collections;

namespace HexaStack.Core
{
    public enum SceneType
    {
        Boot,
        Loading,
        Lobby,
        InGame,
    }
    public class SceneLoader : SingletonBehaviour<SceneLoader>
    {
        [Header("Settings")]
        [Tooltip("로딩 씬의 Enum 값 (빌드 인덱스와 일치해야 함)")]
        [SerializeField] private SceneType _loadingSceneType = SceneType.Loading;

        [Tooltip("로딩 화면을 유지할 최소 시간 (초) - 너무 빠르면 0.5초 정도로 줄여도 됨")]
        [SerializeField] private float _minLoadingTime = 1.0f;

        private SceneData _pendingData;

        private LoadingBootstrap _currentLoadingScreen;

        public void LoadScene(SceneType targetScene, SceneData data = null)
        {
            _pendingData = data;
            StartCoroutine(LoadSceneProcess(targetScene));
        }

        public void RegisterLoadingScreen(LoadingBootstrap screen)
        {
            _currentLoadingScreen = screen;
        }

        private IEnumerator LoadSceneProcess(SceneType targetScene)
        {
            yield return SceneManager.LoadSceneAsync((int)_loadingSceneType, LoadSceneMode.Additive);
            yield return null;

            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.IsValid())
            {
                yield return SceneManager.UnloadSceneAsync(currentScene);
            }

            float cleanupRatio = 0.50f;

            // 청소 전 메모리 확인
            long beforeMem = System.GC.GetTotalMemory(false);
            Logger.Log($"[Memory] 청소 전: {beforeMem / 1024 / 1024} MB");

            AsyncOperation unloadOp = Resources.UnloadUnusedAssets();

            while (!unloadOp.isDone)
            {
                float currentProgress = Mathf.Clamp01(unloadOp.progress) * cleanupRatio;

                if (_currentLoadingScreen != null)
                {
                    _currentLoadingScreen.UpdateProgress(currentProgress);
                }
                yield return null;
            }

            System.GC.Collect();

            // 청소 후 메모리 확인
            long afterMem = System.GC.GetTotalMemory(false);
            Logger.Log($"[Memory] 청소 후: {afterMem / 1024 / 1024} MB");

            Logger.Log($"[Memory] 확보된 메모리: {(beforeMem - afterMem) / 1024} KB");

            float startTime = Time.time;
            AsyncOperation op = SceneManager.LoadSceneAsync((int)targetScene, LoadSceneMode.Additive);
            op.allowSceneActivation = false;

            while (!op.isDone)
            {
                float realProgress = Mathf.Clamp01(op.progress / 0.9f);
                float timeProgress = Mathf.Clamp01((Time.time - startTime) / _minLoadingTime);

                float baseProgress = Mathf.Min(realProgress, timeProgress);

                float finalProgress = cleanupRatio + ((1.0f - cleanupRatio) * baseProgress);

                if (_currentLoadingScreen != null)
                {
                    _currentLoadingScreen.UpdateProgress(finalProgress);
                }

                if (op.progress >= 0.9f && timeProgress >= 1.0f)
                {
                    if (_currentLoadingScreen != null) _currentLoadingScreen.UpdateProgress(1.0f);

                    yield return new WaitForSeconds(0.2f);

                    op.allowSceneActivation = true;
                    yield return null;

                    Scene newScene = SceneManager.GetSceneByBuildIndex((int)targetScene);
                    if (newScene.IsValid()) SceneManager.SetActiveScene(newScene);

                    yield return SceneManager.UnloadSceneAsync((int)_loadingSceneType);
                    _currentLoadingScreen = null;

                    yield break;
                }

                yield return null;
            }
        }

        public T GetSceneData<T>() where T : SceneData
        {
            var data = _pendingData as T;
            _pendingData = null;
            return data;
        }
    }
}