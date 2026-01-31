using UnityEngine;

namespace HexaStack.Core
{
    /// <summary>
    /// 글로벌 스피너 컨트롤러 (Reference Counting 방식)
    /// [설계] UIManager와 분리하여 단일 책임 원칙(SRP) 준수
    /// [핵심] 여러 시스템이 동시에 Show()를 호출해도 모든 Hide()가 호출되어야 꺼짐
    /// </summary>
    public class GlobalSpinner : SingletonBehaviour<GlobalSpinner>
    {
        [Header("References")]
        [Tooltip("스피너 전체 바디 (배경 포함)")]
        [SerializeField] private GameObject m_SpinnerBody;

        [Tooltip("돌아가는 애니메이션이 있는 오브젝트 (선택)")]
        [SerializeField] private Animator m_SpinnerAnimator;

        // [핵심] 스피너 요청 횟수 카운터
        private int m_RequestCount = 0;

        protected override void Init()
        {
            base.Init();

            // 안전장치: 바디 할당 안 했으면 자기 자신을 바디로 사용
            if (object.ReferenceEquals(m_SpinnerBody, null))
            {
                m_SpinnerBody = this.gameObject;
            }

            // [핵심] 항상 Canvas 맨 뒤로 이동 (최상위 렌더링)
            transform.SetAsLastSibling();

            // 초기화: 강제 숨김
            ForceHideInternal();
        }

        /// <summary>
        /// 스피너 표시 요청
        /// [원리] 요청이 들어올 때마다 카운트 증가 (중복 호출 안전)
        /// </summary>
        public void Show()
        {
            m_RequestCount++;

            if (!m_SpinnerBody.activeSelf)
            {
                // [핵심] 항상 최상위로 올림 (나중에 생성된 UI 위에 표시)
                transform.SetAsLastSibling();
                
                m_SpinnerBody.SetActive(true);

                // 애니메이터가 있다면 활성화
                if (!object.ReferenceEquals(m_SpinnerAnimator, null))
                {
                    m_SpinnerAnimator.enabled = true;
                }
            }
        }

        /// <summary>
        /// 스피너 숨김 요청
        /// [원리] 요청이 끝날 때마다 카운트 감소. 0이 되어야 진짜로 꺼짐.
        /// </summary>
        public void Hide()
        {
            m_RequestCount--;

            // 카운트가 꼬여서 음수가 되면 0으로 보정
            if (m_RequestCount <= 0)
            {
                m_RequestCount = 0;
                m_SpinnerBody.SetActive(false);
            }
        }

        /// <summary>
        /// [비상용] 강제로 끄기 (씬 전환이나 에러 발생 시 초기화용)
        /// </summary>
        public void ForceHide()
        {
            ForceHideInternal();
        }

        private void ForceHideInternal()
        {
            m_RequestCount = 0;
            if (!object.ReferenceEquals(m_SpinnerBody, null))
            {
                m_SpinnerBody.SetActive(false);
            }
        }

        /// <summary>
        /// 현재 스피너가 활성화 상태인지 확인
        /// </summary>
        public bool IsActive => m_RequestCount > 0;
    }
}
