using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Collider _collider;

    public HexStack HexStack { get; private set; }

    public Color Color 
    {
        get => _renderer.material.color;
        set => _renderer.material.color = value;
    }

    public void Configure(HexStack hexStack)
    {
        HexStack = hexStack;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void DisableCollider() => _collider.enabled = false;

    public void Vanish(float delay)
    {
        LeanTween.cancel(gameObject);

        LeanTween.scale(gameObject, Vector3.zero, .4615f)
            .setEase(LeanTweenType.easeInBack)
            .setDelay(delay)
            .setOnComplete(() => Destroy(gameObject));
    }

    public void MoveToLocal(Vector3 targetLocalPos)
    {
        LeanTween.cancel(gameObject);

        float delay = transform.GetSiblingIndex() * .023f;

        // 애니메이션 완료 후 정확한 위치로 보정하여 어긋남 방지
        LeanTween.moveLocal(gameObject, targetLocalPos, .4615f)
            .setEase(LeanTweenType.easeInOutSine)
            .setDelay(delay)
            .setOnComplete(() => {
                // 애니메이션 완료 후 정확한 위치로 강제 설정
                transform.localPosition = targetLocalPos;
                // 회전도 초기화
                transform.localRotation = Quaternion.identity;
            });

        Vector3 direction = (targetLocalPos - transform.localPosition).With(y: 0).normalized;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        LeanTween.rotateAround(gameObject, rotationAxis, 180, .4615f)
            .setEase(LeanTweenType.easeInOutSine)
            .setDelay(delay)
            .setOnComplete(() => {
                // 회전 애니메이션 완료 후 정확한 회전으로 보정
                transform.localRotation = Quaternion.identity;
            });
    }
}
