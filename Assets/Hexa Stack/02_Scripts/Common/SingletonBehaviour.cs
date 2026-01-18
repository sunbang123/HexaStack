using UnityEngine;

namespace HexaStack.Core
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        // 씬 전환 시 삭제할지 여부
        [SerializeField] protected bool m_IsDestroyOnLoad = false;

        // 이 클래스의 스태틱 인스턴스 변수
        protected static T m_Instance;

        // [안정성] 앱이 종료되는 중인지 체크 (좀비 생성 방지)
        private static bool m_IsShuttingDown = false;

        public static T Instance
        {
            get
            {
                // 앱 종료 중이면 null 리턴 (에러 방지)
                if (m_IsShuttingDown)
                {
                    // Debug.LogWarning($"[Singleton] {typeof(T)}는 이미 종료되었습니다.");
                    return null;
                }
                return m_Instance;
            }
        }

        private void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            // [마샬링 최적화 핵심] 
            // m_Instance == null 은 유니티 엔진 내부를 검사함 (Fake Null Check).
            // (object)m_Instance == null 은 순수 C# 참조만 검사함 (초고속).
            if ((object)m_Instance == null)
            {
                m_Instance = (T)this;

                if (!m_IsDestroyOnLoad)
                {
                    DontDestroyOnLoad(this);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // 삭제 시 실행되는 함수
        protected virtual void OnDestroy()
        {
            if ((object)m_Instance == this)
            {
                Dispose();
            }
        }

        // [안정성] 게임 종료 시그널 감지
        private void OnApplicationQuit()
        {
            m_IsShuttingDown = true;
        }

        // 삭제 시 추가로 처리해 주어야할 작업을 여기서 처리
        protected virtual void Dispose()
        {
            m_IsShuttingDown = true; // 확실하게 종료 플래그 박기
            m_Instance = null;
        }
    }
}