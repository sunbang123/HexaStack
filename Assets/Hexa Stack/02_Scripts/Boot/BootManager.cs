using System.Collections;
using UnityEngine;
using HexaStack.Views;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Controllers.Managers
{
    public class BootManager : SingletonBehaviour<BootManager>
    {
        [Header("Settings")]
        [SerializeField] private float minLoadingTime = 2f; // 최소 로딩 시간 (초)
        [SerializeField] private bool waitForMinimumTime = true; // 최소 시간 대기 여부

        public BootUIController BootUIController { get; private set; }

        protected override void Init()
        {
            m_IsDestroyOnLoad = true;
            base.Init();
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            BootUIController = FindObjectOfType<BootUIController>();
            if (!BootUIController)
            {
                Logger.LogWarning("BootUIController does not exist. Creating default loading...");
                // BootUIController가 없어도 기본 로딩 진행
                StartCoroutine(LoadLobbySceneCoroutine());
                return;
            }

            BootUIController.Init();
            StartCoroutine(LoadLobbySceneCoroutine());
        }

        private IEnumerator LoadLobbySceneCoroutine()
        {
            // SceneLoader가 없으면 생성
            if (SceneLoader.Instance == null)
            {
                Logger.LogWarning("SceneLoader.Instance is null. Creating new SceneLoader...");
                GameObject sceneLoaderObj = new GameObject("SceneLoader");
                sceneLoaderObj.AddComponent<SceneLoader>();
                yield return null; // 한 프레임 대기하여 초기화 완료 대기
            }

            float startTime = Time.time;
            AsyncOperation asyncOperation = SceneLoader.Instance.LoadSceneAsync(SceneType.Lobby);

            if (asyncOperation == null)
            {
                Logger.LogError("Failed to load Lobby scene asynchronously.");
                yield break;
            }

            // 로딩 진행률 업데이트
            while (!asyncOperation.isDone)
            {
                float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f); // 0.9까지가 실제 로딩, 0.9~1.0은 활성화 대기

                // 최소 시간 대기 체크
                if (waitForMinimumTime)
                {
                    float elapsedTime = Time.time - startTime;
                    float timeBasedProgress = elapsedTime / minLoadingTime;
                    
                    // 시간 기반과 실제 로딩 진행률 중 더 높은 값을 사용
                    progress = Mathf.Max(progress, timeBasedProgress);
                }

                // UI 업데이트
                if (BootUIController != null)
                {
                    BootUIController.UpdateLoadingProgress(progress);
                }

                // 로딩이 완료되고 최소 시간도 지났으면 활성화
                if (asyncOperation.progress >= 0.9f)
                {
                    if (!waitForMinimumTime || (Time.time - startTime) >= minLoadingTime)
                    {
                        // 약간의 지연 후 활성화 (부드러운 전환)
                        yield return new WaitForSeconds(0.5f);
                        SceneLoader.Instance.ActivateScene(asyncOperation);
                        break;
                    }
                }

                yield return null;
            }

            Logger.Log("Lobby scene loaded successfully.");
        }
    }
}
