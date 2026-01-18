using System;
using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;

namespace HexaStack.Core
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [Header("Global Canvas")]
        [SerializeField] private Canvas _globalCanvas;

        // 1. 프리팹 주소를 타입별로 보관하는 Lookup Table
        // Key: 컴포넌트 타입, Value: 해당 컴포넌트가 붙은 프리팹 원본
        private Dictionary<Type, Component> _prefabRegistry = new Dictionary<Type, Component>();

        // 2. 이미 생성된 인스턴스를 관리하는 캐시 (오브젝트 풀링)
        private Dictionary<Type, Component> _uiCache = new Dictionary<Type, Component>();

        protected override void Init()
        {
            m_IsDestroyOnLoad = false;
            base.Init();
        }

        /// <summary>
        /// 초기 설정: 프리팹들을 등록함 (컴파일 타임에 타입 확정)
        /// </summary>
        public void RegisterPrefab<T>(T prefab) where T : Component
        {
            _prefabRegistry[typeof(T)] = prefab;
        }

        public T OpenUI<T>(BaseUIData data = null) where T : BaseUI // T는 무조건 BaseUI여야 함
        {
            Type type = typeof(T);

            // 1. 캐시(풀) 확인: 마샬링 없이 즉시 주소 반환
            if (_uiCache.TryGetValue(type, out Component cachedUI))
            {
                T ui = (T)cachedUI;
                ui.gameObject.SetActive(true);

                SetupAndShow(ui, data);

                return ui;
            }

            // 2. 레지스트리(프리팹) 확인
            if (!_prefabRegistry.TryGetValue(type, out Component prefab))
            {
                Logger.LogError($"[UIManager] {type} 프리팹 등록 누락!");
                return null;
            }

            // 3. 생성과 동시에 컴포넌트 획득 (마샬링 최소화)
            T instance = Instantiate((T)prefab, _globalCanvas.transform);
            _uiCache[type] = instance;
            instance.Init(_globalCanvas.transform);

            SetupAndShow(instance, data);

            return instance;
        }

        /// <summary>
        /// [Helper] UI 열기 직전의 공통 로직 (오디오, 데이터 주입, Show 호출)
        /// </summary>
        private void SetupAndShow<T>(T ui, BaseUIData data) where T : BaseUI
        {
            // [Audio Injection] 여기서 한 번만 관리하면 됨!
            if (!object.ReferenceEquals(AudioManager.Instance, null))
            {
                AudioManager.Instance.PlaySFX(SFX.UIButtonClick);
            }

            ui.SetInfo(data);
            ui.ShowUI();
        }

        public void CloseUI(BaseUI ui)
        {
            if (object.ReferenceEquals(ui, null)) return;
            HandleClose(ui);
        }

        private void HandleClose(BaseUI ui)
        {
            ui.CloseUI();
            ui.gameObject.SetActive(false);

            Logger.Log($"[UIManager] {ui.GetType().Name} closed.");
        }
    }
}