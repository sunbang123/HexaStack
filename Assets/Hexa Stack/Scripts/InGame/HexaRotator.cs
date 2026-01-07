using System.Collections;
using UnityEngine;

public class GridRotator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform gridGenerator;
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float minDistanceThreshold = 100f; // 이 거리 이하에서는 회전 감쇠
    [SerializeField] private float snapAngle = 60f; // 육각형 스냅 각도 (60도)
    [SerializeField] private float snapDuration = 0.3f; // 스냅 애니메이션 지속 시간

    private float lastAngle;
    private bool isRotating = false;
    private bool wasDraggingHexagon = false;
    private bool isSnapping = false; // 스냅 애니메이션 중인지 확인

    private void Update()
    {
        // 스냅 애니메이션 중에는 입력 무시
        if (isSnapping)
            return;

        if (ShouldBlockRotation())
        {
            isRotating = false;
            return;
        }

        HandleRotationInput();
    }

    private bool ShouldBlockRotation()
    {
        if (StackController.IsDragging)
        {
            wasDraggingHexagon = true;
            return true;
        }

        if (wasDraggingHexagon)
        {
            wasDraggingHexagon = false;
            return true;
        }

        return false;
    }

    private void HandleRotationInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartRotation();
        }
        else if (Input.GetMouseButton(0) && isRotating)
        {
            UpdateRotation();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isRotating)
            {
                SnapToHexagonAngle();
            }
            isRotating = false;
        }
    }

    private void StartRotation()
    {
        isRotating = true;
        lastAngle = CalculateMouseAngle();
    }

    private void UpdateRotation()
    {
        if (gridGenerator == null) return;

        float currentAngle = CalculateMouseAngle();
        float angleDelta = Mathf.DeltaAngle(lastAngle, currentAngle);
        
        // 거리 기반 감쇠 적용
        float distanceFromCenter = GetDistanceFromCenter();
        float dampedAngle = ApplyDistanceDamping(angleDelta, distanceFromCenter);
        
        float rotationAmount = dampedAngle * rotationSpeed;
        gridGenerator.Rotate(0, rotationAmount, 0, Space.World);
        
        lastAngle = currentAngle;
    }

    private float CalculateMouseAngle()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 mouseDirection = (Vector2)Input.mousePosition - screenCenter;
        
        return Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
    }

    private float GetDistanceFromCenter()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        return Vector2.Distance(Input.mousePosition, screenCenter);
    }

    private float ApplyDistanceDamping(float angleDelta, float distance)
    {
        if (distance < minDistanceThreshold)
        {
            // 중심에 가까울수록 회전량 감소 (0~1)
            float damping = distance / minDistanceThreshold;
            return angleDelta * damping;
        }
        
        return angleDelta;
    }

    private void SnapToHexagonAngle()
    {
        if (gridGenerator == null) return;

        // 현재 Y축 회전 각도 가져오기
        float currentYRotation = gridGenerator.eulerAngles.y;
        
        // 가장 가까운 스냅 각도 계산
        float snappedAngle = Mathf.Round(currentYRotation / snapAngle) * snapAngle;
        
        // 각도 정규화 (0~360 범위)
        snappedAngle = snappedAngle % 360f;
        if (snappedAngle < 0f)
            snappedAngle += 360f;
        
        // 부드러운 스냅 애니메이션 시작
        StartCoroutine(SmoothSnapRotation(currentYRotation, snappedAngle));
    }

    private IEnumerator SmoothSnapRotation(float startAngle, float targetAngle)
    {
        isSnapping = true;
        
        // 각도 차이 계산 (가장 짧은 경로로 회전)
        float angleDifference = Mathf.DeltaAngle(startAngle, targetAngle);
        float endAngle = startAngle + angleDifference;
        
        float elapsedTime = 0f;
        Vector3 currentRotation = gridGenerator.eulerAngles;
        
        while (elapsedTime < snapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / snapDuration;
            
            // 부드러운 이징 함수 적용 (EaseOut)
            t = 1f - Mathf.Pow(1f - t, 3f);
            
            float currentYRotation = Mathf.LerpAngle(startAngle, endAngle, t);
            gridGenerator.rotation = Quaternion.Euler(currentRotation.x, currentYRotation, currentRotation.z);
            
            yield return null;
        }
        
        // 최종 각도로 정확히 설정
        gridGenerator.rotation = Quaternion.Euler(currentRotation.x, endAngle, currentRotation.z);
        
        isSnapping = false;
    }
}