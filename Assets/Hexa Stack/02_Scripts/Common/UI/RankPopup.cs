using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HexaStack.Core; // BaseUI가 있는 네임스페이스 (브로 프로젝트에 맞게 확인!)

namespace HexaStack.Views
{
    public class RankPopup : BaseUI
    {
        [Header(" UI Elements ")]
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _scrollView;
        [SerializeField] private Transform _contentRoot; // 아이템들이 생성될 부모 (Content)

        [Header(" Prefabs ")]
        [SerializeField] private GameObject _rankItemPrefab; // 개별 레벨/업적 아이템 프리팹

        // 초기화 여부 체크
        private bool _isInitialized = false;

        private void Awake()
        {
            // 닫기 버튼 연결 (BaseUI에 Close 기능이 있다고 가정하거나 직접 구현)
            if (_closeBtn != null)
            {
                _closeBtn.onClick.AddListener(OnCloseBtnClicked);
            }
        }

        private void OnEnable()
        {
            // 팝업이 켜질 때마다 데이터를 갱신하고 싶다면 여기서 호출
            RefreshUI();
        }

        /// <summary>
        /// UI 데이터 갱신 (잠긴 레벨/열린 레벨 표시)
        /// </summary>
        public void RefreshUI()
        {
            // 1. 기존 아이템 청소 (Pooling을 쓴다면 반환 로직으로 변경)
            foreach (Transform child in _contentRoot)
            {
                Destroy(child.gameObject);
            }

            // 2. 데이터 가져오기 (예: LevelManager나 UserDataManager에서)
            // 임시로 더미 데이터 10개를 돌린다고 가정할게.
            int maxLevel = 10;
            int currentLevel = 5; // 유저가 현재 도달한 레벨 (예시)

            for (int i = 1; i <= maxLevel; i++)
            {
                CreateAchieveItem(i, i <= currentLevel);
            }
        }

        private void CreateAchieveItem(int levelIndex, bool isUnlocked)
        {
            if (_rankItemPrefab == null) return;

            GameObject itemObj = Instantiate(_rankItemPrefab, _contentRoot);

            // 여기서 아이템 스크립트를 가져와서 세팅 (AchieveItem 스크립트가 있다면)
            // var itemScript = itemObj.GetComponent<AchieveItem>();
            // itemScript.SetData(levelIndex, isUnlocked);

            // (스크립트 없으면 간단히 이름만 변경)
            itemObj.name = $"Level_Item_{levelIndex}_{(isUnlocked ? "Open" : "Locked")}";
        }

        private void OnCloseBtnClicked()
        {
            // UIManager를 통해 닫거나, 스스로 꺼짐
            // 만약 UIManager 구조가 CloseUI<T>()를 지원한다면:
            // UIManager.Instance.CloseUI<AchievePopup>();

            // 단순히 끄는 거라면:
            this.gameObject.SetActive(false);
        }
    }
}