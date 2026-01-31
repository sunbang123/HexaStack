using System;
using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;

namespace HexaStack.Core
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [Header("UI Canvas")]
        [Tooltip("모든 UI가 생성되는 Canvas (Global/Local UI 모두 이 Canvas에 생성됨)")]
        [SerializeField] private Canvas _uiCanvas;

        // 1. 프리팹 레지스트리: 타입별 프리팹 저장
        private Dictionary<Type, Component> _prefabRegistry = new Dictionary<Type, Component>();

        // 2. 생성된 UI 인스턴스 캐시
        private Dictionary<Type, Component> _uiCache = new Dictionary<Type, Component>();

        // 3. Global UI 타입 추적 (씬 전환 시에도 유지)
        private HashSet<Type> _globalUITypes = new HashSet<Type>();

        protected override void Init()
        {
            m_IsDestroyOnLoad = false;
            base.Init();
        }

        /// <summary>
        /// Global UI 등록 (BootManager에서 호출)
        /// </summary>
        public void RegisterPrefabGlobal<T>(T prefab) where T : Component
        {
            Type type = typeof(T);
            _prefabRegistry[type] = prefab;
            _globalUITypes.Add(type);
            Logger.Log($"[UIManager] Global UI 등록: {type.Name}");
        }

        /// <summary>
        /// Local UI 등록 (씬별 Controller에서 호출)
        /// </summary>
        public void RegisterPrefabLocal<T>(T prefab) where T : Component
        {
            Type type = typeof(T);
            _prefabRegistry[type] = prefab;
            // Global이 아니면 Local로 간주
            if (!_globalUITypes.Contains(type))
            {
                Logger.Log($"[UIManager] Local UI 등록: {type.Name}");
            }
        }

        /// <summary>
        /// [Deprecated] RegisterPrefabLocal 사용 권장
        /// </summary>
        public void RegisterPrefab<T>(T prefab) where T : Component
        {
            RegisterPrefabLocal(prefab);
        }

        /// <summary>
        /// UI 열기 (제네릭 타입 기반 - NO String, NO Marshaling)
        /// [성능 최적화] Dictionary Lookup O(1), 캐시 히트 시 즉시 반환
        /// </summary>
        public T OpenUI<T>(BaseUIData data = null) where T : BaseUI
        {
            Type type = typeof(T);

            // 1. 캐시 히트: 이미 생성된 인스턴스 재사용 (NO Instantiate)
            if (_uiCache.TryGetValue(type, out Component cachedUI))
            {
                T ui = (T)cachedUI;
                ui.gameObject.SetActive(true);
                SetupAndShow(ui, data);
                return ui;
            }

            // 2. 프리팹 레지스트리 조회 (NO String, Type 기반)
            if (!_prefabRegistry.TryGetValue(type, out Component prefab))
            {
                Logger.LogError($"[UIManager] {type.Name} 프리팹이 등록되지 않았습니다!");
                return null;
            }

            // 3. 인스턴스 생성 및 캐시 저장
            T instance = Instantiate((T)prefab, _uiCanvas.transform);
            _uiCache[type] = instance;
            instance.Init(_uiCanvas.transform);

            SetupAndShow(instance, data);

            return instance;
        }

        /// <summary>
        /// UI를 미리 생성하여 캐시에 저장 (Prewarm)
        /// [성능 최적화] 첫 OpenUI 호출 시 Instantiate 지연 제거
        /// </summary>
        public void Prewarm<T>() where T : BaseUI
        {
            Type type = typeof(T);

            // 이미 캐시에 있으면 스킵 (중복 생성 방지)
            if (_uiCache.ContainsKey(type))
            {
                Logger.Log($"[UIManager] {type.Name} is already prewarmed.");
                return;
            }

            // 프리팹 레지스트리 조회 (NO String)
            if (!_prefabRegistry.TryGetValue(type, out Component prefab))
            {
                Logger.LogError($"[UIManager] {type.Name} 프리팹이 등록되지 않았습니다!");
                return;
            }

            // 미리 생성하여 캐시에 저장 (비활성화 상태로)
            T instance = Instantiate((T)prefab, _uiCanvas.transform);
            instance.gameObject.SetActive(false);
            _uiCache[type] = instance;
            instance.Init(_uiCanvas.transform);

            Logger.Log($"[UIManager] {type.Name} prewarmed.");
        }

        /// <summary>
        /// Local UI 해제 (씬 전환 시 호출)
        /// Global UI는 유지하고 Local UI만 제거
        /// [최적화] 단일 순회로 처리하여 성능 향상
        /// </summary>
        public void UnregisterLocalUIs()
        {
            // [성능 최적화] Dictionary 순회 중 수정을 위해 키를 먼저 수집
            // 하지만 더 효율적으로: 직접 제거하면서 순회 (역순 처리)
            var prefabKeys = new List<Type>(_prefabRegistry.Keys);
            var cacheKeys = new List<Type>(_uiCache.Keys);

            // Local UI 프리팹 레지스트리에서 제거
            for (int i = prefabKeys.Count - 1; i >= 0; i--)
            {
                Type type = prefabKeys[i];
                if (!_globalUITypes.Contains(type))
                {
                    _prefabRegistry.Remove(type);
                    Logger.Log($"[UIManager] Local UI 프리팹 해제: {type.Name}");
                }
            }

            // Local UI 인스턴스 제거 (캐시에서)
            for (int i = cacheKeys.Count - 1; i >= 0; i--)
            {
                Type type = cacheKeys[i];
                if (!_globalUITypes.Contains(type))
                {
                    if (_uiCache.TryGetValue(type, out Component ui) && !object.ReferenceEquals(ui, null))
                    {
                        if (!object.ReferenceEquals(ui.gameObject, null))
                        {
                            Destroy(ui.gameObject);
                        }
                    }
                    _uiCache.Remove(type);
                    Logger.Log($"[UIManager] Local UI 인스턴스 제거: {type.Name}");
                }
            }
        }

        /// <summary>
        /// [Helper] UI 열기 과정의 공통 로직 (오디오, 데이터 설정, Show 호출)
        /// [최적화] NO Marshaling: object.ReferenceEquals 사용
        /// </summary>
        private void SetupAndShow<T>(T ui, BaseUIData data) where T : BaseUI
        {
            // [Audio Injection] NO Marshaling: object.ReferenceEquals 사용
            var audio = AudioManager.Instance;
            if (!object.ReferenceEquals(audio, null))
            {
                audio.PlaySFX(SFX.UIButtonClick);
            }

            ui.SetInfo(data);
            ui.ShowUI();
        }

        /// <summary>
        /// UI 닫기 (NO Marshaling: object.ReferenceEquals 사용)
        /// </summary>
        public void CloseUI(BaseUI ui)
        {
            if (object.ReferenceEquals(ui, null)) return;
            HandleClose(ui);
        }

        /// <summary>
        /// UI 닫기 처리 (비활성화, 로그)
        /// </summary>
        private void HandleClose(BaseUI ui)
        {
            ui.CloseUI();
            ui.gameObject.SetActive(false);

            Logger.Log($"[UIManager] {ui.GetType().Name} closed.");
        }
    }
}
