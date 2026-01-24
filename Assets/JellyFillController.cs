using System;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System 필수
using UnityEngine.EventSystems;

public class JellyFillController : MonoBehaviour
{
    public event Action<float> OnProgressUpdated;

    private static readonly int FillID = Shader.PropertyToID("_FillPercent");

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private Vector3 _initialScale;

    [Header("Logic Settings")]
    [Range(0, 1)]
    [SerializeField] private float _currentProgress = 0f;
    [SerializeField] private float _fillSpeed = 0.5f;

    [Header("Visual Settings")]
    [SerializeField] private float _visualMaxHeight = 0.021f;

    [Header("Jelly Animation")]
    [SerializeField] private float _jellySpeed = 20f;
    [SerializeField] private float _jellyAmount = 0.1f;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        _initialScale = transform.localScale;
    }

    void Update()
    {
        // 1. 입력 감지 (마우스 또는 터치)
        bool isPressed = CheckInputPressed();

        // 2. UI 위를 누르고 있는지 확인 (UI 충돌 방지)
        // 누르고 있는 상태라면, 그 위치가 UI 위인지 체크합니다.
        if (isPressed && IsPointerOverUI())
        {
            // UI를 누르고 있다면 젤리 채우기 로직을 건너뜁니다.
            return;
        }

        // 3. 젤리 채우기 로직
        // 누르고 있고 + 아직 꽉 안 찼을 때
        bool isFilling = isPressed && _currentProgress < 1.0f;

        if (isFilling)
        {
            _currentProgress += Time.deltaTime * _fillSpeed;
            ApplyJellyEffect(); // 젤리 출렁거림 효과
        }
        else
        {
            // 손을 뗐거나 꽉 찼으면 원래 크기로 탄성 있게 복구
            transform.localScale = Vector3.Lerp(transform.localScale, _initialScale, Time.deltaTime * 10f);
        }

        // 값 보정 (0~1 사이 유지)
        _currentProgress = Mathf.Clamp01(_currentProgress);

        // 쉐이더 및 UI 알림 업데이트
        UpdateShader();
        OnProgressUpdated?.Invoke(_currentProgress);
    }

    /// <summary>
    /// 마우스나 터치 입력을 감지합니다. (New Input System)
    /// </summary>
    private bool CheckInputPressed()
    {
        // A. 마우스 체크
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            return true;

        // B. 터치 체크 (화면을 누르고 있는 터치가 하나라도 있으면 true)
        if (Touchscreen.current != null)
        {
            // 메모리 할당 방지를 위해 foreach 대신 for문 사용 가능하지만,
            // Touchscreen.current.touches는 ReadOnlyArray라 가독성을 위해 foreach 사용
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 현재 입력(마우스/터치)이 UI 위에 있는지 확인합니다. (New Input System)
    /// </summary>
    private bool IsPointerOverUI()
    {
        // 1. 마우스가 UI 위에 있는지 확인
        if (Mouse.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;

        // 2. 터치가 UI 위에 있는지 확인
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed)
                {
                    // New Input System에서는 touchId를 통해 UI 터치 여부를 판별합니다.
                    int pointerId = touch.touchId.ReadValue();
                    if (EventSystem.current.IsPointerOverGameObject(pointerId))
                        return true;
                }
            }
        }

        return false;
    }

    private void ApplyJellyEffect()
    {
        float sineWave = Mathf.Sin(Time.time * _jellySpeed);

        // Y축(높이)은 늘어나고 (+), X/Z축(두께)은 줄어듬 (-) -> 젤리 느낌
        float stretchY = 1 + (sineWave * _jellyAmount);
        float squashXZ = 1 - (sineWave * _jellyAmount * 0.5f);

        Vector3 targetScale = new Vector3(
            _initialScale.x * squashXZ,
            _initialScale.y * stretchY,
            _initialScale.z * squashXZ
        );

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
    }

    private void UpdateShader()
    {
        float finalShaderValue = _currentProgress * _visualMaxHeight;

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(FillID, finalShaderValue);
        _renderer.SetPropertyBlock(_propBlock);
    }
}