using UnityEngine;
using UnityEngine.UI;

namespace HexaStack.Core
{
    /// <summary>
    /// 스피너(모래시계) 기본 인터페이스
    /// [로딩 UI 법칙] 1~3초 짧은 로딩 시 사용
    /// </summary>
    public abstract class BaseSpinner : MonoBehaviour
    {
        /// <summary>
        /// 스피너 표시 (Blocking UI - 화면 터치 막음)
        /// </summary>
        public abstract void Show();

        /// <summary>
        /// 스피너 숨김
        /// </summary>
        public abstract void Hide();

        /// <summary>
        /// 스피너 활성화 상태 확인
        /// </summary>
        public abstract bool IsActive { get; }
    }
}
