using System;
using UnityEngine;
using UnityEngine.UI;

namespace HexaStack.Core
{
    /// <summary>
    /// UI에 전달할 데이터 구조
    /// </summary>
    public class BaseUIData
    {
        public Action OnShow;
        public Action OnClose;
        // 필요하다면 이후에 커스텀 타입이나 콜백 함수를 추가할 수 있어.
    }

    /// <summary>
    /// 모든 UI 프리팹의 기본 클래스
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))] // 애니메이션 편의와 블록 레이어를 위해 필요
    public abstract class BaseUI : MonoBehaviour
    {
        [Header("Base UI Elements")]
        [SerializeField] protected CanvasGroup m_CanvasGroup;
        [SerializeField] protected Animation m_UIOpenAnim;

        [Header("Loading UI (Optional)")]
        [Tooltip("데이터 로딩 중 표시할 스피너 (1~3초 짧은 로딩용) - 선택적")]
        [SerializeField] protected BaseSpinner m_Spinner; // 스피너 참조 (Inspector에서 할당)

        [Tooltip("스피너 표시 시 화면 터치를 막을 배경 (Blocking UI) - 선택적")]
        [SerializeField] protected UnityEngine.UI.Image m_BlockingBackground; // 투명 배경으로 터치 차단

        protected Action m_OnShow;
        protected Action m_OnClose;

        /// <summary>
        /// UIManager가 Instantiate 이후에 호출 (설정)
        /// </summary>
        public virtual void Init(Transform anchor)
        {
            if (m_CanvasGroup == null) m_CanvasGroup = GetComponent<CanvasGroup>();

            // 초기 상태는 숨김
            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;

            // Blocking Background 초기화 (스피너가 있는 경우)
            if (!object.ReferenceEquals(m_BlockingBackground, null))
            {
                m_BlockingBackground.gameObject.SetActive(false);
                m_BlockingBackground.raycastTarget = true; // 터치 차단 활성화
            }
        }

        public virtual void SetInfo(BaseUIData uiData)
        {
            if (uiData == null) return;
            m_OnShow = uiData.OnShow;
            m_OnClose = uiData.OnClose;
        }

        public virtual void ShowUI()
        {
            m_CanvasGroup.alpha = 1;
            m_CanvasGroup.interactable = true;
            m_CanvasGroup.blocksRaycasts = true;

            if (m_UIOpenAnim != null) m_UIOpenAnim.Play();

            m_OnShow?.Invoke();
            m_OnShow = null; // 1회성 실행 후 초기화 (메모리 누수 방지)
        }

        public virtual void CloseUI(bool isCloseAll = false)
        {
            if (!isCloseAll) m_OnClose?.Invoke();

            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;
        }

        // 버튼 OnClick에 연결용
        public virtual void OnClickCloseButton()
        {
            // 스피너가 표시 중이면 닫기 불가 (로딩 중에는 닫기 방지)
            if (!object.ReferenceEquals(m_Spinner, null) && m_Spinner.IsActive)
            {
                return; // 로딩 중에는 닫기 불가
            }

            // 이후 UIManager로 가서 닫아달라고 요청 (타입 안정성)
            UIManager.Instance.CloseUI(this);
        }

        #region Spinner Helper Methods (Optional)

        /// <summary>
        /// 스피너 표시 (Blocking UI - 화면 터치 막음)
        /// [로딩 UI 법칙] 1~3초 짧은 로딩 시 사용
        /// [최적화] NO Marshaling: object.ReferenceEquals 사용
        /// </summary>
        protected void ShowSpinner()
        {
            if (!object.ReferenceEquals(m_Spinner, null))
            {
                m_Spinner.Show();
            }

            // Blocking Background 표시 (터치 차단)
            if (!object.ReferenceEquals(m_BlockingBackground, null))
            {
                m_BlockingBackground.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 스피너 숨김
        /// [최적화] NO Marshaling: object.ReferenceEquals 사용
        /// </summary>
        protected void HideSpinner()
        {
            if (!object.ReferenceEquals(m_Spinner, null))
            {
                m_Spinner.Hide();
            }

            // Blocking Background 숨김
            if (!object.ReferenceEquals(m_BlockingBackground, null))
            {
                m_BlockingBackground.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 스피너 활성화 상태 확인
        /// </summary>
        protected bool IsSpinnerActive => !object.ReferenceEquals(m_Spinner, null) && m_Spinner.IsActive;

        #endregion
    }
}
