using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform stackPositionsParent; // "Stack Positions" 부모 오브젝트
    [SerializeField] private Transform bootsLogoPosition;

    [Header("Settings")]
    [SerializeField] private float moveInterval = 2f; // 이동 간격 (초)
    [SerializeField] private bool autoStart = true; // 자동 시작 여부
    [SerializeField] private bool loop = true; // 반복 여부

    [Header("Continuous Rotation")]
    [SerializeField] private float rotationSpeed = 50f; // 초당 회전 각도

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.4615f; // 애니메이션 지속 시간
    [SerializeField] private float delayBetweenHexagons = 0.023f; // 헥사곤 간 지연 시간
    [SerializeField] private float stackSpacing = 0.2f; // 스택 간격

    private List<Transform> allStackPositions = new List<Transform>();
    private bool isAnimating = false;

    private void Start()
    {
        // Stack Positions 부모 찾기
        if (stackPositionsParent == null)
        {
            GameObject stackPositionsObj = GameObject.Find("Stack Positions");
            if (stackPositionsObj != null)
            {
                stackPositionsParent = stackPositionsObj.transform;
            }
        }

        // 모든 Stack Position 찾기
        FindAllStackPositions();

        if (autoStart)
        {
            StartAnimation();
        }
    }

    private void Update()
    {
        if (bootsLogoPosition != null)
        {
            bootsLogoPosition.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
        }
    }

    private void FindAllStackPositions()
    {
        allStackPositions.Clear();

        if (stackPositionsParent == null)
            return;

        // Stack Positions의 모든 자식(Stack Position 00, 01, ...) 찾기
        for (int i = 0; i < stackPositionsParent.childCount; i++)
        {
            Transform stackPos = stackPositionsParent.GetChild(i);
            if (stackPos.name.Contains("Stack Position"))
            {
                allStackPositions.Add(stackPos);
            }
        }

        // 숫자 순서대로 정렬
        allStackPositions.Sort((a, b) => GetStackPositionNumber(a).CompareTo(GetStackPositionNumber(b)));
    }

    private int GetStackPositionNumber(Transform stackPosition)
    {
        // "Stack Position 00" 또는 "Stack Position 0"에서 숫자 추출
        string name = stackPosition.name;
        string numberStr = name.Replace("Stack Position", "").Trim();
        
        if (int.TryParse(numberStr, out int number))
        {
            return number;
        }
        
        return int.MaxValue; // 파싱 실패 시 맨 뒤로
    }

    public void StartAnimation()
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimationLoop());
        }
    }

    public void StopAnimation()
    {
        isAnimating = false;
        StopAllCoroutines();
    }

    private IEnumerator AnimationLoop()
    {
        isAnimating = true;

        do
        {
            yield return StartCoroutine(MoveFromFilledToEmpty());

            if (loop)
            {
                yield return new WaitForSeconds(moveInterval);
            }
        } while (loop && isAnimating);

        isAnimating = false;
    }

    private IEnumerator MoveFromFilledToEmpty()
    {
        // 채워진 Stack Position 찾기 (숫자 순서대로)
        List<Transform> filledPositions = GetFilledStackPositions();
        
        if (filledPositions.Count == 0)
        {
            yield break;
        }

        // 채워진 위치에서 HexaLogo들을 가져와서 인접한 빈 위치로 이동
        foreach (Transform filledPos in filledPositions)
        {
            List<Hexagon> hexagons = GetHexagonsFromStackPosition(filledPos);
            if (hexagons.Count == 0)
                continue;

            // 현재 위치의 번호 확인
            int currentNumber = GetStackPositionNumber(filledPos);
            
            // 인접한 빈 위치 찾기 (±1)
            Transform targetPos = GetAdjacentEmptyStackPosition(currentNumber);
            
            if (targetPos == null)
            {
                continue; // 인접한 빈 위치가 없으면 스킵
            }

            // HexaLogo들을 이동
            yield return StartCoroutine(MoveHexagonsToPosition(filledPos, targetPos, hexagons));
        }
    }

    private Transform GetAdjacentEmptyStackPosition(int currentNumber)
    {
        // ±1 위치의 Stack Position 찾기
        List<int> adjacentNumbers = new List<int>();
        
        // 앞쪽 (-1)
        if (currentNumber > 0)
        {
            adjacentNumbers.Add(currentNumber - 1);
        }
        
        // 뒤쪽 (+1)
        if (currentNumber < 6)
        {
            adjacentNumbers.Add(currentNumber + 1);
        }

        // 인접한 위치 중 빈 곳 찾기
        foreach (int adjacentNumber in adjacentNumbers)
        {
            Transform adjacentPos = GetStackPositionByNumber(adjacentNumber);
            if (adjacentPos != null && GetHexagonsFromStackPosition(adjacentPos).Count == 0)
            {
                return adjacentPos;
            }
        }

        return null;
    }

    private Transform GetStackPositionByNumber(int number)
    {
        foreach (Transform stackPos in allStackPositions)
        {
            if (GetStackPositionNumber(stackPos) == number)
            {
                return stackPos;
            }
        }
        return null;
    }

    private List<Transform> GetFilledStackPositions()
    {
        List<Transform> filledPositions = new List<Transform>();

        foreach (Transform stackPos in allStackPositions)
        {
            if (GetHexagonsFromStackPosition(stackPos).Count > 0)
            {
                filledPositions.Add(stackPos);
            }
        }

        // 숫자 순서대로 정렬 (0, 1, 2, 3, 4, 5, 6)
        filledPositions.Sort((a, b) => GetStackPositionNumber(a).CompareTo(GetStackPositionNumber(b)));

        return filledPositions;
    }

    private List<Hexagon> GetHexagonsFromStackPosition(Transform stackPosition)
    {
        List<Hexagon> hexagons = new List<Hexagon>();

        // Stack Position의 모든 자식에서 Hexagon 컴포넌트 찾기
        for (int i = 0; i < stackPosition.childCount; i++)
        {
            Transform child = stackPosition.GetChild(i);
            Hexagon hexagon = child.GetComponent<Hexagon>();
            
            // HexaLogo 오브젝트 안에 Hexagon이 있을 수 있음
            if (hexagon == null)
            {
                hexagon = child.GetComponentInChildren<Hexagon>();
            }

            if (hexagon != null)
            {
                hexagons.Add(hexagon);
            }
        }

        return hexagons;
    }

    private IEnumerator MoveHexagonsToPosition(Transform sourcePosition, Transform targetPosition, List<Hexagon> hexagonsToMove)
    {
        if (hexagonsToMove == null || hexagonsToMove.Count == 0)
            yield break;

        // 타겟 위치의 기존 Hexagon 개수 확인
        int existingHexagonCount = GetHexagonsFromStackPosition(targetPosition).Count;

        // 각 Hexagon을 타겟 위치로 이동
        for (int i = 0; i < hexagonsToMove.Count; i++)
        {
            Hexagon hexagon = hexagonsToMove[i];
            if (hexagon == null)
                continue;

            // 타겟 로컬 위치 계산 (스택 높이에 따라)
            float targetY = existingHexagonCount * stackSpacing + i * stackSpacing;
            Vector3 targetLocalPosition = Vector3.up * targetY;

            // 부모를 타겟 Stack Position으로 변경
            hexagon.SetParent(targetPosition);

            // 애니메이션 시작
            hexagon.MoveToLocal(targetLocalPosition);
        }

        // 애니메이션 완료 대기
        float totalDelay = animationDuration + (hexagonsToMove.Count * delayBetweenHexagons);
        yield return new WaitForSeconds(totalDelay);
    }

    // 외부에서 호출 가능한 메서드들
    public void SetMoveInterval(float interval)
    {
        moveInterval = interval;
    }

    public void SetLoop(bool shouldLoop)
    {
        loop = shouldLoop;
    }

    public void RefreshStackPositions()
    {
        FindAllStackPositions();
    }
}
