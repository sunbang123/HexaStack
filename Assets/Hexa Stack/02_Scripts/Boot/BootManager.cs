using System.Collections;
using UnityEngine;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Scenes.Boot
{
    public class BootManager : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("로고를 보여줄 최소 시간")]
        [SerializeField] private float _logoDisplayTime = 2.0f;

        [Header("Global Managers (Prefabs)")]
        [SerializeField] private SceneLoader _sceneLoaderPrefab;
        [SerializeField] private AudioManager _audioManagerPrefab;

        [Header("Global Systems (Prefabs)")]
        [SerializeField] private GameObject _eventSystemPrefab;

        private void Start()
        {
            InitializeGlobalSystems();
            InitializeGlobalManagers();
            StartCoroutine(SplashProcess());
        }

        private void InitializeGlobalSystems()
        {
            // SingletonBehaviour 덕분에 .Instance 호출만으로도 체크 가능!
            if (object.ReferenceEquals(GlobalEventSystem.Instance, null))
            {
                if (_eventSystemPrefab != null)
                {
                    // 여기서 Instantiate 하면 GlobalEventSystem의 Init()이 돌면서 
                    // DontDestroyOnLoad까지 한 방에 해결됨!
                    Instantiate(_eventSystemPrefab);
                    Logger.Log("[Boot] GlobalEventSystem Created.");
                }
            }
        }
        private void InitializeGlobalManagers()
        {
            // ✨ [Pro Level Optimization]
            // "유니티 엔진아, SceneLoader 죽었니?" (X - 마샬링 발생)
            // "C# 변수야, 너 지금 비어있니?" (O - 마샬링 Zero)

            // 1. SceneLoader 생성 체크
            // .Instance 프로퍼티가 static 변수를 반환한다고 가정할 때,
            // 초기화 여부만 따지므로 ReferenceEquals가 가장 빠름.
            if (object.ReferenceEquals(SceneLoader.Instance, null))
            {
                // 프리팹은 유니티 오브젝트니까 Unity Null Check (안정성)
                if (_sceneLoaderPrefab != null)
                {
                    Instantiate(_sceneLoaderPrefab);
                }
                else
                {
                    Logger.LogError("[BootManager] SceneLoader Prefab is missing!");
                }
            }

            // 2. SoundManager 생성 체크
            if (object.ReferenceEquals(AudioManager.Instance, null))
            {
                if (_audioManagerPrefab != null)
                {
                    Instantiate(_audioManagerPrefab);
                }
            }
        }

        private IEnumerator SplashProcess()
        {
            Logger.Log("[BootManager] System Initialized. Waiting for logo display...");

            // WaitForSeconds도 'new' 할 때 가비지가 생김.
            // 극한의 최적화를 원한다면 캐싱해서 쓸 수 있지만,
            // Boot는 딱 1번 실행되므로 이 정도는 쿨하게 넘어가도 됨. (애자일!)
            yield return new WaitForSeconds(_logoDisplayTime);

            // 안전장치: 여기도 ReferenceEquals 가능
            if (object.ReferenceEquals(SceneLoader.Instance, null)) yield break;

            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }
    }
}