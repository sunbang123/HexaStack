using UnityEngine;
using UnityEngine.EventSystems;
using HexaStack.Core; // SingletonBehaviour가 있는 네임스페이스

namespace HexaStack.Core
{
    // 1. 우리가 만든 싱글톤 베이스를 상속받음
    public class GlobalEventSystem : SingletonBehaviour<GlobalEventSystem>
    {
        private EventSystem _eventSystem;

        protected override void Init()
        {
            // 2. 이 녀석은 파괴되면 안 되니까 불사신 설정
            m_IsDestroyOnLoad = false;
            base.Init();

            // 3. 내 오브젝트에 붙어있는 실제 EventSystem 컴포넌트 확인
            _eventSystem = GetComponent<EventSystem>();

            if (_eventSystem == null)
            {
                Logger.LogError("EventSystem 컴포넌트가 이 오브젝트에 없습니다!");
            }
        }

        // 씬 전환 시 중복된 놈이 들어오면 SingletonBehaviour의 Awake에서 
        // 이미 Instance가 있다면 알아서 Destroy(this.gameObject)를 해줄 거야.
    }
}