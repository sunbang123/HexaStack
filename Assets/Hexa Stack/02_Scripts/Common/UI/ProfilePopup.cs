using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HexaStack.Core;

namespace HexaStack.Views
{
    public class ProfilePopup : BaseUI
    {
        [Header(" UI Elements ")]
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _scrollView;
        [SerializeField] private Transform _contentRoot; // 아이템들이 생성될 곳 (Content)

        [Header(" Prefabs ")]
        [SerializeField] private GameObject _achieveItemPrefab; // 각각의 랭크/등급 아이템 프리팹

        // 초기화 여부 확인
        private bool _isInitialized = false;

        private void Awake()
        {
            // 닫기 버튼 연결 (BaseUI에 Close 기능이 있다고 가정하지만 현재 구현)
            if (_closeBtn != null)
            {
                _closeBtn.onClick.AddListener(OnCloseBtnClicked);
            }
        }

        private void OnEnable()
        {
            // 팝업이 활성화될 때마다 데이터를 갱신하고 보여주면 좋을 것 같음
            RefreshUI();
        }

        /// <summary>
        /// UI 데이터 갱신 (잠금 랭크/열린 랭크 표시)
        /// </summary>
        public void RefreshUI()
        {
            // 1. 기존 아이템 제거 (Pooling을 쓴다면 풀링 방식으로 변경)
            foreach (Transform child in _contentRoot)
            {
                Destroy(child.gameObject);
            }

            // 2. 데이터 가져오기 (예: LevelManager나 UserDataManager에서)
            // 임시로 더미 데이터 10개를 가져온다고 가정할게.
            int maxLevel = 10;
            int currentLevel = 5; // 플레이어가 현재 달성한 랭크 (임시)

            for (int i = 1; i <= maxLevel; i++)
            {
                CreateAchieveItem(i, i <= currentLevel);
            }
        }

        private void CreateAchieveItem(int levelIndex, bool isUnlocked)
        {
            if (_achieveItemPrefab == null) return;

            GameObject itemObj = Instantiate(_achieveItemPrefab, _contentRoot);

            // 이후에 아이템 스크립트를 가져와서 설정 (AchieveItem 스크립트가 있다면)
            // var itemScript = itemObj.GetComponent<AchieveItem>();
            // itemScript.SetData(levelIndex, isUnlocked);

            // (스크립트 없으면 임시로 이름만 변경)
            itemObj.name = $"Level_Item_{levelIndex}_{(isUnlocked ? "Open" : "Locked")}";
        }

        private void OnCloseBtnClicked()
        {
            // UIManager를 통해 닫거나, 직접 비활성화
            // 만약 UIManager 구현이 CloseUI<T>()를 제공한다면:
            // UIManager.Instance.CloseUI<AchievePopup>();

            // 임시로 닫는 방법:
            this.gameObject.SetActive(false);
        }
    }
}
