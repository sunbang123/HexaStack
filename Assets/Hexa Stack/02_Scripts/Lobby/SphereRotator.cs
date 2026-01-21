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

    private float currentX = 0f; // 좌우 회전값
    private float currentY = 0f; // 상하 회전값
    private bool isDragging = false;
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
        if (targetSphere == null) return;

        var mouse = Mouse.current;
        var touch = Touchscreen.current;

        // 1. 클릭/터치 시작
        if ((mouse != null && mouse.leftButton.wasPressedThisFrame) || (touch != null && touch.primaryTouch.press.wasPressedThisFrame))
        {
            isDragging = true;
            lastMousePosition = GetInputPosition();
        }

        // 2. 드래그 중
        if (isDragging && ((mouse != null && mouse.leftButton.isPressed) || (touch != null && touch.primaryTouch.press.isPressed)))
        {
            Vector2 delta = GetInputPosition() - lastMousePosition;

            // 감도 적용하여 회전값 갱신
            currentX += delta.x * sensitivity;
            currentY -= delta.y * sensitivity;

            // 수직 회전 제한 (위아래 뒤집힘 방지)
            currentY = Mathf.Clamp(currentY, -80f, 80f);

            ApplyPosition();
            lastMousePosition = GetInputPosition();
        }

        // 3. 손 뗐을 때 (스냅 없이 단순히 드래그 상태 해제)
        if ((mouse != null && mouse.leftButton.wasReleasedThisFrame) || (touch != null && touch.primaryTouch.press.wasReleasedThisFrame))
        {
            isDragging = false;
        }
    }

    private void ApplyPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = rotation * new Vector3(0, 0, -distance) + targetSphere.position;
        transform.rotation = rotation;
    }

    private Vector2 GetInputPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();
        return Mouse.current.position.ReadValue();
    }
}