using UnityEngine;
using UnityEngine.UI;
using HexaStack.Core;

namespace HexaStack.Views
{
    public class RankPopup : BaseUI
    {
        [Header(" UI Elements ")]
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _scrollView;
        [SerializeField] private Transform _contentRoot; // 아이템들이 생성될 곳 (Content)

        [Header(" Prefabs ")]
        [SerializeField] private GameObject _rankItemPrefab; // 각각의 랭크/등급 아이템 프리팹

        // 초기화 여부 확인
        private bool _isInitialized = false;

        private void Awake()
        {
            // 닫기 버튼 연결
            if (!object.ReferenceEquals(_closeBtn, null))
            {
                _closeBtn.onClick.AddListener(OnCloseBtnClicked);
            }
        }

        private void OnEnable()
        {
            // 팝업이 활성화될 때마다 데이터를 갱신
            // 즉시 표시 (Prewarm으로 인해 빠르게 표시됨)
            RefreshUI();
        }

        /// <summary>
        /// UI 데이터 갱신 (잠금 랭크/열린 랭크 표시)
        /// [로딩 UI 법칙] 데이터 로딩 시 스피너 표시 (1~3초)
        /// </summary>
        public void RefreshUI()
        {
            // 1. 기존 아이템 제거 (Pooling을 쓴다면 풀링 방식으로 변경)
            foreach (Transform child in _contentRoot)
            {
                Destroy(child.gameObject);
            }

            // 2. 데이터 로딩 시작 (서버 통신 등)
            StartCoroutine(LoadRankDataCoroutine());
        }

        /// <summary>
        /// 랭킹 데이터 로딩 코루틴
        /// [로딩 UI 법칙] 1~3초 짧은 로딩이므로 스피너 사용
        /// </summary>
        private System.Collections.IEnumerator LoadRankDataCoroutine()
        {
            // 글로벌 스피너 표시 (Blocking UI)
            // [안전] Instance가 null이면 스피너 없이 진행
            GlobalSpinner.Instance?.Show();

            // TODO: 실제 서버 통신 또는 데이터 로딩 로직
            // 예: yield return StartCoroutine(RankDataManager.Instance.FetchRankData());
            
            // 임시: 1~2초 시뮬레이션 (실제로는 서버 통신 시간)
            yield return new UnityEngine.WaitForSeconds(1.5f);

            // 3. 데이터 가져오기 (예: LevelManager나 UserDataManager에서)
            // 임시로 더미 데이터 10개를 가져온다고 가정할게.

            GlobalSpinner.Instance?.Hide();
        }

        /// <summary>
        /// 랭크 아이템 생성
        /// [최적화] NO Marshaling: object.ReferenceEquals 사용
        /// </summary>
        private void CreateAchieveItem(int levelIndex, bool isUnlocked)
        {
            // NO Marshaling: object.ReferenceEquals 사용
            if (object.ReferenceEquals(_rankItemPrefab, null)) return;

            GameObject itemObj = Instantiate(_rankItemPrefab, _contentRoot);

        }

        private void OnCloseBtnClicked()
        {
            // BaseUI의 OnClickCloseButton이 스피너 체크를 자동으로 처리함
            OnClickCloseButton();
        }
    }
}
