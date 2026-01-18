using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace HexaStack.Controllers
{
    public class GridRotatorController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform gridGenerator;
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float minDistanceThreshold = 100f;
    [SerializeField] private float snapAngle = 60f;
    [SerializeField] private float snapDuration = 0.3f;

    private float lastAngle;
    private bool isRotating = false;
    private bool wasDraggingHexagon = false;
    private bool isSnapping = false;

    private void Update()
    {
        // 스냅 애니메이션 중에는 모든 입력 무시
        if (isSnapping) return;

        // 드래그 중인 헥사곤이 있다면 회전 로직 실행 안 함
        if (StackController.IsDragging)
        {
            isRotating = false;
            wasDraggingHexagon = true;
            return;
        }

        // 헥사곤 드래그가 방금 끝난 프레임이라면 한 프레임 쉼 (오작동 방지)
        if (wasDraggingHexagon)
        {
            wasDraggingHexagon = false;
            return;
        }

        HandleRotationInput();
    }

    private void HandleRotationInput()
    {
        bool mouseDown = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool mouseHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
        bool mouseUp = Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;

        bool touchDown = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        bool touchHeld = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;
        bool touchUp = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;

        // 1. 처음 누르는 순간 (Start)
        if (mouseDown || touchDown)
        {
            // [수정 핵심] 처음 누를 때만 UI 위에 있는지 체크합니다.
            if (!IsPointerOverUI())
            {
                StartRotation();
            }
        }
        // 2. 누르고 있는 동안 (Update)
        else if ((mouseHeld || touchHeld) && isRotating)
        {
            // 이미 회전이 시작되었다면 UI 위에 마우스가 있어도 상관없이 업데이트합니다.
            UpdateRotation();
        }
        // 3. 뗐을 때 (End)
        else if (mouseUp || touchUp)
        {
            if (isRotating)
            {
                SnapToHexagonAngle();
            }
            isRotating = false;
        }
    }

    private bool IsPointerOverUI()
    {
        // 마샬링 없는 비교!
        if (object.ReferenceEquals(EventSystem.current, null))
        {
            return true;
        }

        return EventSystem.current.IsPointerOverGameObject();
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

        float distanceFromCenter = GetDistanceFromCenter();
        float dampedAngle = ApplyDistanceDamping(angleDelta, distanceFromCenter);

        float rotationAmount = dampedAngle * rotationSpeed;
        gridGenerator.Rotate(0, rotationAmount, 0, Space.World);

        lastAngle = currentAngle;
    }

    private float CalculateMouseAngle()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 inputPosition = GetInputPosition();
        Vector2 mouseDirection = inputPosition - screenCenter;
        return Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
    }

    private float GetDistanceFromCenter()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 inputPosition = GetInputPosition();
        return Vector2.Distance(inputPosition, screenCenter);
    }

    private Vector2 GetInputPosition()
    {
        // 터치 또는 마우스 좌표 반환
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        return Vector2.zero;
    }

    private float ApplyDistanceDamping(float angleDelta, float distance)
    {
        if (distance < minDistanceThreshold)
        {
            float damping = distance / minDistanceThreshold;
            return angleDelta * damping;
        }
        return angleDelta;
    }

    private void SnapToHexagonAngle()
    {
        if (gridGenerator == null) return;

        float currentYRotation = gridGenerator.eulerAngles.y;
        float snappedAngle = Mathf.Round(currentYRotation / snapAngle) * snapAngle;

        snappedAngle = snappedAngle % 360f;
        if (snappedAngle < 0f) snappedAngle += 360f;

        StartCoroutine(SmoothSnapRotation(currentYRotation, snappedAngle));
    }

    private IEnumerator SmoothSnapRotation(float startAngle, float targetAngle)
    {
        isSnapping = true;
        float angleDifference = Mathf.DeltaAngle(startAngle, targetAngle);
        float endAngle = startAngle + angleDifference;
        float elapsedTime = 0f;
        Vector3 currentRotation = gridGenerator.eulerAngles;

        while (elapsedTime < snapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / snapDuration;
            t = 1f - Mathf.Pow(1f - t, 3f); // EaseOut

            float currentYRotation = Mathf.LerpAngle(startAngle, endAngle, t);
            gridGenerator.rotation = Quaternion.Euler(currentRotation.x, currentYRotation, currentRotation.z);
            yield return null;
        }

        gridGenerator.rotation = Quaternion.Euler(currentRotation.x, endAngle, currentRotation.z);
        isSnapping = false;
    }
    }
}