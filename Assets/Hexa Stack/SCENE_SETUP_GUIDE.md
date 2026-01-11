# HexaStack 씬 설정 가이드

MVC 패턴에 맞춘 Unity 씬 구조 설정 방법입니다.

## 전체 구조 개요

### 1. ManagerRegistry (모든 씬에서 사용)
- **위치**: 첫 번째 씬(보통 Lobby)에만 배치
- **설정**: `DontDestroyOnLoad = true` (씬 전환 시 유지)
- **역할**: 모든 Manager와 Controller를 중앙에서 관리

### 2. Lobby 씬 구조

```
Lobby (씬)
├── [Scene Root]
│   ├── --- ENVIRONEMENT ---
│   │   └── (환경 오브젝트들)
│   │
│   ├── [Managers] (빈 GameObject, 정리용)
│   │   └── LobbyManager (GameObject)
│   │       └── LobbyManager 컴포넌트
│   │
│   ├── [UI]
│   │   └── LobbyUICanvas (Canvas)
│   │       ├── (UI 요소들)
│   │       └── LobbyUIController (GameObject 또는 Canvas 자체)
│   │           └── LobbyUIController 컴포넌트
│   │
│   └── SceneLoader (GameObject) - 선택사항 (ManagerRegistry에 포함 가능)
│       └── SceneLoader 컴포넌트
│
└── ManagerRegistry (GameObject) ⭐ 중요: 첫 씬에만 배치
    └── ManagerRegistry 컴포넌트
        (DontDestroyOnLoad 설정됨)
```

### 3. InGame 씬 구조

```
InGame (씬)
├── [Scene Root]
│   ├── --- ENVIRONEMENT ---
│   │   └── (환경 오브젝트들, 그리드 등)
│   │
│   ├── [Managers] (빈 GameObject, 정리용)
│   │   └── InGameManager (GameObject)
│   │       └── InGameManager 컴포넌트
│   │
│   ├── [UI]
│   │   └── InGameUICanvas (Canvas)
│   │       ├── ScoreText (TMP_Text) - 점수 표시
│   │       └── InGameUIController (GameObject 또는 Canvas 자체)
│   │           └── InGameUIController 컴포넌트
│   │
│   ├── [Controllers]
│   │   ├── MergeController (GameObject)
│   │   │   └── MergeController 컴포넌트
│   │   │       - scoreText: ScoreText 참조
│   │   │       - breakImagePrefab: 파괴 이펙트 프리팹
│   │   │       - cameraUI: UI 카메라
│   │   │       - scoreTargetPosition: 점수 UI 위치
│   │   │       - increaseScoreEffectPrefab: 점수 증가 이펙트
│   │   │
│   │   ├── StackSpawnerController (GameObject)
│   │   │   └── StackSpawnerController 컴포넌트
│   │   │       - stackPositionsParent: 스택 생성 위치 부모
│   │   │       - hexagonPrefab: 헥사곤 프리팹
│   │   │       - hexagonStackPrefab: 스택 프리팹
│   │   │       - minMaxHexCount: 헥사곤 개수 범위
│   │   │       - colors: 색상 배열
│   │   │
│   │   ├── StackController (GameObject)
│   │   │   └── StackController 컴포넌트
│   │   │       - hexagonLayerMask: 헥사곤 레이어
│   │   │       - gridHexagonLayerMask: 그리드 헥사곤 레이어
│   │   │       - groundLayerMask: 지면 레이어
│   │   │
│   │   └── GridRotatorController (GameObject)
│   │       └── GridRotatorController 컴포넌트
│   │           - gridGenerator: 그리드 생성기 Transform
│   │           - rotationSpeed: 회전 속도
│   │           - minDistanceThreshold: 최소 거리
│   │           - snapAngle: 스냅 각도
│   │           - snapDuration: 스냅 애니메이션 시간
│   │
│   └── [Models]
│       └── ScoreManager (GameObject) - 선택사항
│           └── ScoreManager 컴포넌트
│               - scoreText: ScoreText 참조 (MergeController와 공유 가능)
│
└── (ManagerRegistry는 이미 첫 씬에서 생성됨, 자동으로 찾아짐)
```

## 상세 설정 방법

### ManagerRegistry 설정

1. **Lobby 씬에서만 생성**
   - Hierarchy에서 우클릭 → Create Empty
   - 이름: `ManagerRegistry`
   - Add Component → `ManagerRegistry` 추가
   - Inspector에서 확인:
     - `m_IsDestroyOnLoad = false` (씬 전환 시 유지됨)

2. **주의사항**
   - ManagerRegistry는 첫 번째 씬(보통 Lobby)에만 배치
   - 다른 씬에서는 자동으로 찾아서 사용
   - DontDestroyOnLoad로 설정되어 있어 씬 전환 시에도 유지됨

### Lobby 씬 설정

1. **LobbyManager 생성**
   ```
   [Managers] (빈 GameObject)
   └── LobbyManager (GameObject)
       └── LobbyManager 컴포넌트 추가
   ```

2. **LobbyUIController 설정**
   - Canvas 오브젝트에 직접 추가하거나
   - 별도 GameObject로 생성하여 Canvas 하위에 배치
   - `LobbyUIController` 컴포넌트 추가

3. **UI 버튼 연결**
   - Start 버튼의 OnClick 이벤트에
   - `LobbyUIController.OnClickStartBtn()` 연결

### InGame 씬 설정

1. **InGameManager 생성**
   ```
   [Managers] (빈 GameObject)
   └── InGameManager (GameObject)
       └── InGameManager 컴포넌트 추가
   ```
   - Start()에서 자동으로 InGameUIController, MergeController, StackSpawnerController를 찾음

2. **MergeController 설정**
   - GameObject 생성 → `MergeController` 컴포넌트 추가
   - Inspector 설정:
     - `scoreText`: Canvas의 ScoreText (TMP_Text) 드래그 앤 드롭
     - `breakImagePrefab`: 파괴 이펙트 프리팹
     - `cameraUI`: UI 카메라 (보통 Canvas의 Render Camera)
     - `scoreTargetPosition`: 점수가 표시될 UI 위치 (Transform)
     - `increaseScoreEffectPrefab`: 점수 증가 이펙트 프리팹

3. **StackSpawnerController 설정**
   - GameObject 생성 → `StackSpawnerController` 컴포넌트 추가
   - Inspector 설정:
     - `stackPositionsParent`: 스택이 생성될 위치들의 부모 Transform
     - `hexagonPrefab`: Hexagon 프리팹
     - `hexagonStackPrefab`: HexStack 프리팹
     - `minMaxHexCount`: 생성될 헥사곤 개수 범위 (예: 2~8)
     - `colors`: 사용할 색상 배열

4. **StackController 설정**
   - GameObject 생성 → `StackController` 컴포넌트 추가
   - Inspector 설정:
     - `hexagonLayerMask`: 헥사곤이 있는 레이어
     - `gridHexagonLayerMask`: 그리드 헥사곤 레이어
     - `groundLayerMask`: 지면 레이어

5. **GridRotatorController 설정**
   - GameObject 생성 → `GridRotatorController` 컴포넌트 추가
   - Inspector 설정:
     - `gridGenerator`: 회전할 그리드의 Transform (보통 그리드 생성기)
     - `rotationSpeed`: 회전 속도 (기본: 1.0)
     - `minDistanceThreshold`: 회전 최소 거리 (기본: 100)
     - `snapAngle`: 스냅 각도 (기본: 60도)
     - `snapDuration`: 스냅 애니메이션 시간 (기본: 0.3초)

6. **ScoreManager 설정 (선택사항)**
   - 현재는 MergeController에서 점수를 직접 관리
   - 나중에 분리하고 싶다면:
     - GameObject 생성 → `ScoreManager` 컴포넌트 추가
     - `scoreText`: Canvas의 ScoreText 참조
     - MergeController에서 ScoreManager 사용하도록 수정 필요

7. **InGameUIController 설정**
   - Canvas 오브젝트에 직접 추가하거나
   - 별도 GameObject로 생성하여 Canvas 하위에 배치
   - `InGameUIController` 컴포넌트 추가
   - UI 버튼들 연결:
     - Settings 버튼 → `OnClickSettingsBtn()`
     - Profile 버튼 → `OnClickProfileBtn()`
     - Chapter 버튼 → `OnClickCurrChapter()`
     - Lobby 버튼 → `OnClickLobbyBtn()`

## 초기화 순서

1. **씬 로드 시**
   - ManagerRegistry.Start() → 모든 Manager 등록
   - 각 Manager.Start() → 해당 씬의 Controller 초기화

2. **Lobby 씬**
   - ManagerRegistry가 등록됨 (이미 존재하거나 새로 생성)
   - LobbyManager.Start() → LobbyUIController 찾아서 Init()

3. **InGame 씬**
   - ManagerRegistry가 자동으로 찾아짐 (DontDestroyOnLoad)
   - InGameManager.Start() → InGameUIController, MergeController, StackSpawnerController 찾아서 초기화

## 디버깅 팁

1. **ManagerRegistry 로그 확인**
   - 게임 시작 시 Console에 "=== ManagerRegistry Registration Status ===" 로그 확인
   - 각 Manager의 등록 상태(✓/✗) 확인

2. **FindObjectOfType 실패 시**
   - 해당 컴포넌트가 씬에 있는지 확인
   - GameObject가 Active인지 확인
   - 컴포넌트가 제대로 추가되었는지 확인

3. **씬 전환 문제**
   - SceneLoader.Instance가 정상 작동하는지 확인
   - ManagerRegistry가 DontDestroyOnLoad로 설정되어 있는지 확인

## 권장사항

1. **Hierarchy 정리**
   - [Managers], [UI], [Controllers], [Models] 같은 빈 GameObject로 그룹화
   - 씬이 복잡해져도 구조 파악이 쉬움

2. **프리팹 사용**
   - 자주 사용하는 Manager 조합은 프리팹으로 만들어 재사용
   - 예: [Managers] 구조를 프리팹으로

3. **네이밍 규칙**
   - Manager: `[씬이름]Manager`
   - Controller: `[기능]Controller`
   - View: `[씬이름]UIController`

## 참고

- 모든 Manager는 `FindObjectOfType`으로 자동 찾기 사용
- 필요시 직접 참조로 변경 가능 (성능 최적화)
- ManagerRegistry를 통해 중앙에서 모든 Manager 접근 가능: `ManagerRegistry.Instance.InGameManager`
