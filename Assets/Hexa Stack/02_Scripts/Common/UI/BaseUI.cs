using System;
using UnityEngine;

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
        [Tooltip("로딩 스피너 프리팹 (배경+애니메이션 포함) - SetActive로 표시/숨김")]
        [SerializeField] protected GameObject m_SpinnerObject;

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

            // 스피너 초기 상태: 숨김
            if (!object.ReferenceEquals(m_SpinnerObject, null))
            {
                m_SpinnerObject.SetActive(false);
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
            // [최적화] object.ReferenceEquals: 마샬링 없이 null 체크
            if (!object.ReferenceEquals(m_SpinnerObject, null) && m_SpinnerObject.activeSelf)
            {
                return; // 로딩 중에는 닫기 불가
            }

            // 이후 UIManager로 가서 닫아달라고 요청 (타입 안정성)
            UIManager.Instance.CloseUI(this);
        }

        #region Spinner Helper Methods (Optional)

        /// <summary>
        /// 스피너 표시 (배경+애니메이션 포함 프리팹)
        /// </summary>
        protected void ShowSpinner()
        {
            if (m_SpinnerObject != null)
            {
                m_SpinnerObject.SetActive(true);
            }
        }

        /// <summary>
        /// 스피너 숨김
        /// </summary>
        protected void HideSpinner()
        {
            if (m_SpinnerObject != null)
            {
                m_SpinnerObject.SetActive(false);
            }
        }

        /// <summary>
        /// 스피너 활성화 상태 확인
        /// </summary>
        protected bool IsSpinnerActive => m_SpinnerObject != null && m_SpinnerObject.activeSelf;

        #endregion
    }
}
