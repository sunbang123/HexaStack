using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class JellyFillController : MonoBehaviour
{
    public event Action<float> OnProgressUpdated;

    private static readonly int FillID = Shader.PropertyToID("_FillPercent");

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private Vector3 _initialScale; // 원래 크기 저장용

    [Header("Logic Settings")]
    [Range(0, 1)]
    [SerializeField] private float _currentProgress = 0f;
    [SerializeField] private float _fillSpeed = 0.5f;

    [Header("Visual Settings")]
    [SerializeField] private float _visualMaxHeight = 0.021f;

    [Header("Jelly Animation")]
    [SerializeField] private float _jellySpeed = 20f;   // 젤리 떨림 속도
    [SerializeField] private float _jellyAmount = 0.1f; // 젤리 변형 강도

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        _initialScale = transform.localScale;
    }

    void Update()
    {
        // 1. 입력 처리 (New Input System)
        // 마우스를 누르고 있고 + 아직 꽉 안 찼을 때
        bool isFilling = Mouse.current != null &&
                         Mouse.current.leftButton.isPressed &&
                         _currentProgress < 1.0f;

        // 2. 로직 계산
        if (isFilling)
        {
            _currentProgress += Time.deltaTime * _fillSpeed;

            ApplyJellyEffect();
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _initialScale, Time.deltaTime * 10f);
        }

        _currentProgress = Mathf.Clamp01(_currentProgress);

        UpdateShader();

        OnProgressUpdated?.Invoke(_currentProgress);
    }

    private void ApplyJellyEffect()
    {
        // 시간(Time)에 따라 사인파(Sin)를 만들어서 떨림 생성
        float sineWave = Mathf.Sin(Time.time * _jellySpeed);

        // Y축(높이)은 늘어나고 (+), X/Z축(두께)은 줄어듬 (-) -> 질량 보존 법칙 느낌
        float stretchY = 1 + (sineWave * _jellyAmount);
        float squashXZ = 1 - (sineWave * _jellyAmount * 0.5f); // Y보다 절반 정도만 줄어들게

        // 원래 크기에 곱해주기
        Vector3 targetScale = new Vector3(
            _initialScale.x * squashXZ,
            _initialScale.y * stretchY,
            _initialScale.z * squashXZ
        );

        // 부드럽게 적용
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
    }

    private void UpdateShader()
    {
        // 로직(0~1) * 시각적 최대높이(0.21) = 최종값
        float finalShaderValue = _currentProgress * _visualMaxHeight;

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(FillID, finalShaderValue);
        _renderer.SetPropertyBlock(_propBlock);
    }
}