using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleSphereRotator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform targetSphere;
    [SerializeField] private float distance = 10f;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private float snapAngle = 60f; // 축구공 면 간격 (60도)
    [SerializeField] private float snapDuration = 0.3f;

    private float currentX = 0f; // 좌우 회전값
    private float currentY = 0f; // 상하 회전값
    private bool isDragging = false;
    private bool isSnapping = false;
    private Vector2 lastMousePosition;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
        ApplyPosition();
    }

    private void LateUpdate()
    {
        if (targetSphere == null || isSnapping) return;

        var mouse = Mouse.current;
        var touch = Touchscreen.current;

        // 1. 클릭 시작
        if ((mouse != null && mouse.leftButton.wasPressedThisFrame) || (touch != null && touch.primaryTouch.press.wasPressedThisFrame))
        {
            isDragging = true;
            lastMousePosition = GetInputPosition();
        }

        // 2. 드래그 중
        if (isDragging && ((mouse != null && mouse.leftButton.isPressed) || (touch != null && touch.primaryTouch.press.isPressed)))
        {
            Vector2 delta = GetInputPosition() - lastMousePosition;
            currentX += delta.x * sensitivity;
            currentY -= delta.y * sensitivity;
            currentY = Mathf.Clamp(currentY, -80f, 80f); // 수직 제한

            ApplyPosition();
            lastMousePosition = GetInputPosition();
        }

        // 3. 손 뗐을 때 스냅 시작
        if ((mouse != null && mouse.leftButton.wasReleasedThisFrame) || (touch != null && touch.primaryTouch.press.wasReleasedThisFrame))
        {
            if (isDragging)
            {
                isDragging = false;
                StartCoroutine(SnapRotation());
            }
        }
    }

    private void ApplyPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = rotation * new Vector3(0, 0, -distance) + targetSphere.position;
        transform.rotation = rotation;
    }

    private IEnumerator SnapRotation()
    {
        isSnapping = true;

        // 현재 위치에서 가장 가까운 60도 배수 지점 계산
        float targetX = Mathf.Round(currentX / snapAngle) * snapAngle;
        float targetY = Mathf.Round(currentY / snapAngle) * snapAngle;

        float startX = currentX;
        float startY = currentY;
        float elapsed = 0;

        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / snapDuration);

            currentX = Mathf.LerpAngle(startX, targetX, t);
            currentY = Mathf.LerpAngle(startY, targetY, t);
            ApplyPosition();
            yield return null;
        }

        isSnapping = false;
    }

    private Vector2 GetInputPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();
        return Mouse.current.position.ReadValue();
    }
}