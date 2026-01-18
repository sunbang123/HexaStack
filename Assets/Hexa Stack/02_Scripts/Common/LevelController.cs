using UnityEngine;
using HexaStack.Core;

namespace HexaStack.Controllers
{
    public class LevelController : MonoBehaviour
    {
        [Header("Level Settings")]
        [SerializeField] private int _currentChapterIndex;
        [SerializeField] private int _targetScore;

        [Header("References")]
        [SerializeField] private GridGeneratorController _gridGenerator;

        // 1. Bootstrap에 의해 호출됨 (의존성 주입)
        public void Setup(int chapterIndex)
        {
            _currentChapterIndex = chapterIndex;

            // Lookup Table(데이터 시트)에서 챕터 정보 가져오기
            LevelData data = GetLevelData(chapterIndex);

            if (data != null)
            {
                _targetScore = data.TargetScore;

                // 그리드 생성 명령 (마샬링 없이 직접 참조)
                if (!object.ReferenceEquals(_gridGenerator, null))
                {
                    _gridGenerator.GenerateGrid();
                }
            }

            Core.Logger.Log($"[LevelController] Chapter {_currentChapterIndex} Setup Complete.");
        }

        private LevelData GetLevelData(int index)
        {
            // 실제로는 ScriptableObject나 JSON에서 가져오는 것이 정석!
            // 일단은 샘플 데이터를 반환하는 구조
            return new LevelData(index);
        }
    }

    // 레벨 정보를 담는 데이터 클래스 (Struct나 ScriptableObject 권장)
    public class LevelData
    {
        public int GridWidth;
        public int GridHeight;
        public int TargetScore;

        public LevelData(int index)
        {
            // 챕터별 하드코딩 대신 나중에 엑셀/SO로 관리하자
            GridWidth = 5 + index;
            GridHeight = 5 + index;
            TargetScore = 1000 * (index + 1);
        }
    }
}