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
        [SerializeField] private UIManager _uiManagerPrefab;

        [Header("Global Systems (Prefabs)")]
        [SerializeField] private GameObject _eventSystemPrefab;

        [Header("Global UI Prefabs (For UIManager Registration)")]
        [SerializeField] private Views.OptionPopup _optionPopupPrefab;
        [SerializeField] private Views.NoticePopup _noticePopupPrefab;

        private void Start()
        {
            InitializeGlobalSystems();
            InitializeGlobalManagers();
            StartCoroutine(SplashProcess());
        }

        private void InitializeGlobalSystems()
        {
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

            // B. UIManager 생성 및 범용 UI 등록
            if (object.ReferenceEquals(UIManager.Instance, null))
            {
                if (_uiManagerPrefab != null)
                {
                    Instantiate(_uiManagerPrefab);
                    Logger.Log("[Boot] UIManager Created.");

                    // 제네릭 등록: 타입 안정성 확보 및 마샬링 우회
                    UIManager.Instance.RegisterPrefab<Views.OptionPopup>(_optionPopupPrefab);
                    UIManager.Instance.RegisterPrefab<Views.NoticePopup>(_noticePopupPrefab);
                }
            }
        }
        private void InitializeGlobalManagers()
        {
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

            yield return new WaitForSeconds(_logoDisplayTime);

            if (object.ReferenceEquals(SceneLoader.Instance, null)) yield break;

            SceneLoader.Instance.LoadScene(SceneType.Lobby);
        }
    }
}