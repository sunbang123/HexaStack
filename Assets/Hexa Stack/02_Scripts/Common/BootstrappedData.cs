namespace HexaStack.Core
{
    // 모든 데이터 박스의 조상님 (추상 클래스)
    public abstract class SceneData { }

    // [예시] 로비 -> 인게임 갈 때 보낼 데이터
    public class InGameSceneData : SceneData
    {
        public int LevelIndex;
        public bool IsHardMode;

        public InGameSceneData(int level, bool isHard)
        {
            LevelIndex = level;
            IsHardMode = isHard;
        }
    }

    // [예시] 인게임 -> 로비 갈 때 보낼 데이터 (결과창 등)
    public class LobbySceneData : SceneData
    {
        public int GoldEarned;
        public LobbySceneData(int gold) => GoldEarned = gold;
    }
}