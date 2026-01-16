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

        /// <summary>
        /// 제네릭 UI 호출: 마샬링 없이 즉시 타입 반환
        /// </summary>
        public T GetUI<T>() where T : Component
        {
            Type type = typeof(T);

            // 1. 캐시 확인 (이미 생성되어 있다면 바로 반환)
            if (_uiCache.TryGetValue(type, out Component cachedUI))
            {
                cachedUI.gameObject.SetActive(true);
                return (T)cachedUI;
            }

            // 2. 레지스트리에서 프리팹 확인
            if (!_prefabRegistry.TryGetValue(type, out Component prefab))
            {
                Logger.LogError($"[UIManager] {type} 타입의 프리팹이 등록되지 않았습니다.");
                return null;
            }

            // 3. 생성: Instantiate<T>를 사용하여 생성과 동시에 컴포넌트 주소 획득 (마샬링 최소화)
            T instance = Instantiate((T)prefab, _globalCanvas.transform);
            _uiCache[type] = instance;

            return instance;
        }

        public T OpenUI<T>(BaseUIData data = null) where T : BaseUI // T는 무조건 BaseUI여야 함
        {
            Type type = typeof(T);

            // 1. 캐시(풀) 확인: 마샬링 없이 즉시 주소 반환
            if (_uiCache.TryGetValue(type, out Component cachedUI))
            {
                T ui = (T)cachedUI;
                ui.gameObject.SetActive(true);
                ui.SetInfo(data); // 데이터 주입
                ui.ShowUI();      // 애니메이션 및 로직 실행
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

            instance.Init(_globalCanvas.transform); // 초기화
            instance.SetInfo(data);
            instance.ShowUI();

            return instance;
        }

        // 1. 타입을 알고 있을 때 (외부 컨트롤러용)
        // 예: UIManager.Instance.CloseUI<OptionPopup>();
        public void CloseUI<T>() where T : BaseUI
        {
            Type type = typeof(T);
            if (_uiCache.TryGetValue(type, out Component cachedUI))
            {
                HandleClose(cachedUI as BaseUI);
            }
        }

        // 2. 인스턴스를 알고 있을 때 (BaseUI 내부 버튼용)
        // 예: UIManager.Instance.CloseUI(this);
        public void CloseUI(BaseUI ui)
        {
            if (object.ReferenceEquals(ui, null)) return;
            HandleClose(ui);
        }

        // 3. 실제 마샬링과 로직을 담당하는 핵심 (내부용)
        private void HandleClose(BaseUI ui)
        {
            ui.CloseUI();
            ui.gameObject.SetActive(false);

            Logger.Log($"[UIManager] {ui.GetType().Name} closed.");
        }
    }
}