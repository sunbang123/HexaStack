# HexaStack

## 프로젝트 소개

**HexaStack**는 육각형 타일을 전략적으로 배치하고 정렬하는 퍼즐 게임입니다. 이 프로젝트는 단순한 코드 구현을 넘어, 최종적으로 **모바일 게임 시장 출시**를 목표로 하는 모바일 최적화 게임 프로젝트입니다.

## 게임 장르 및 핵심 컨셉

* **장르**: 하이퍼 캐주얼 퍼즐 (Hyper-casual Puzzle)
* **핵심 메카닉**: 육각형 타일 스택 및 색상 정렬 (Color Sorting)
* **타겟층**: 직관적이고 중독성 있는 퍼즐을 즐기는 모든 연령대의 모바일 유저

## 🎮 게임 규칙 (Gameplay Rules)

HexaStack의 핵심 로직(Core Logic)은 간단하지만 전략적인 판단을 요구합니다.

1. **타일 배치**: 플레이어는 화면 하단에 생성된 다양한 색상의 육각형 타일 더미(Stack)를 선택하여 그리드 위의 빈 칸에 배치합니다.
2. **색상 정렬 및 이동**: 타일을 배치했을 때, 인접한 칸에 **같은 색상의 타일**이 있다면 해당 타일들이 한곳으로 자동으로 이동하며 쌓입니다.
3. **스택 완성 (Clear)**: 하나의 그리드 칸에 같은 색상의 타일이 일정 개수 이상 쌓이게 되면, 해당 타일 더미가 사라지면서 점수를 획득합니다.
4. **연쇄 반응 (Combo)**: 타일이 이동하고 사라지는 과정에서 새로운 공간이 생기거나 인접 타일 색상이 일치하게 되면 연쇄적인 정렬이 일어납니다.
5. **게임 오버**: 그리드의 모든 칸이 타일로 가득 차서 더 이상 새로운 타일 더미를 놓을 수 없게 되면 게임이 종료됩니다.

## 프로젝트 목표 및 방향성 (Mobile Focused)

1. **모바일 최적화**: 저사양 기기에서도 부드럽게 구동되도록 Unity 성능 최적화에 집중하고 있습니다.
2. **직관적인 UX/UI**: 모바일 환경에 맞춘 원터치 조작 방식과 한눈에 들어오는 깔끔한 인터페이스를 지향합니다.
3. **레벨 디자인 고도화**: 단순 반복이 아닌, 플레이어가 성취감을 느낄 수 있도록 점진적인 난이도 곡선을 설계합니다.
4. **효율적인 개발 프로세스**: Cursor(AI 에디터)를 활용하여 게임 로직을 개선하고 생산성을 극대화했습니다.

# 이미지 / 영상 리소스

https://github.com/user-attachments/assets/de890afa-3775-4372-9111-864a0b2ea75f

## 개발 환경 (Technical Stack)

* **Engine**: **Unity 2022.3.62f1 (LTS)**
* **Tools**: **Cursor (AI Code Editor)**
* **Language**: C#
* **Platform**: Android / iOS (출시 목표)

## 참고 자료 (References)

본 프로젝트는 아래의 강의 영상들을 학습하여 모바일 게임으로 재구성되었습니다.

* [YouTube - Hexa Stack 개발 참고 1](https://www.youtube.com/watch?v=jHIX4MJG0mA)
* [YouTube - Hexa Stack 개발 참고 2](https://www.youtube.com/watch?v=HcE_7lOaUa4)
* [YouTube - Hexa Stack 개발 참고 3](https://www.youtube.com/watch?v=6ZMRA8xtOiU)

---

**GitHub:** [sunbang123](https://github.com/sunbang123)