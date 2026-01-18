using System;
using UnityEngine;

namespace HexaStack.Core
{
    /// <summary>
    /// UI에 전달할 데이터 상자
    /// </summary>
    public class BaseUIData
    {
        public Action OnShow;
        public Action OnClose;
        // 필요하다면 여기에 사운드 타입이나 연출 플래그를 추가할 수 있어.
    }

    /// <summary>
    /// 모든 UI 프리팹의 조상님
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))] // 페이드 효과 및 클릭 차단을 위해 필수
    public abstract class BaseUI : MonoBehaviour
    {
        [Header("Base UI Elements")]
        [SerializeField] protected CanvasGroup m_CanvasGroup;
        [SerializeField] protected Animation m_UIOpenAnim;

        protected Action m_OnShow;
        protected Action m_OnClose;

        /// <summary>
        /// UIManager가 Instantiate 직후에 호출 (주입)
        /// </summary>
        public virtual void Init(Transform anchor)
        {
            if (m_CanvasGroup == null) m_CanvasGroup = GetComponent<CanvasGroup>();

            // 초기 상태는 꺼두기
            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;
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
            // 전역 매니저에게 나를 닫아달라고 요청 (타입 기반)
            UIManager.Instance.CloseUI(this);
        }
    }
}