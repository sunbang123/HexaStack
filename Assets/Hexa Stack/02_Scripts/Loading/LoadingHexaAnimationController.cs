using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using HexaStack.Core;
using Logger = HexaStack.Core.Logger;

namespace HexaStack.Views
{
    /// <summary>
    /// BootScene에서 사용할 화려한 Hexa 애니메이션 컨트롤러
    /// 헥사곤이 위에서 떨어져 차곡차곡 쌓이는 애니메이션
    /// </summary>
    public class LoadingHexaAnimationController : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private Hexagon hexagonPrefab;
        [SerializeField] private Transform animationParent;

        [Header("Animation Cycle")]
        [SerializeField] private float cycleDuration = 3f;
        [SerializeField] private bool autoRepeat = true;

        [Header("Stack Settings")]
        [SerializeField] private int hexagonCount = 9;
        [SerializeField] private int stackColumns = 3; // 가로 열 개수
        [SerializeField] private float hexagonHeight = 0.6f; // 헥사곤 한 층 높이
        [SerializeField] private float hexagonWidth = 1.2f; // 헥사곤 가로 너비
        [SerializeField] private float stackSpacing = 0.1f; // 층간 간격

        [Header("Drop Animation")]
        [SerializeField] private float dropStartY = 10f; // 떨어지기 시작 높이
        [SerializeField] private float dropDelay = 0.15f; // 각 헥사곤 간격
        [SerializeField] private float dropDuration = 0.8f; // 낙하 시간
        [SerializeField] private float dropSpread = 3f; // 좌우 랜덤 범위

        [Header("Stack Impact")]
        [SerializeField] private float bounceHeight = 0.3f; // 착지 후 통통 튀는 높이
        [SerializeField] private float bounceDuration = 0.3f; // 통통 튀는 시간
        [SerializeField] private int bounceCount = 2; // 통통 횟수

        [Header("Visual Effects")]
        [SerializeField] private bool useRotation = true;
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private bool useGlow = true;
        [SerializeField] private float glowIntensity = 1.8f;
        [SerializeField] private bool useSquash = true; // 착지 시 찌그러짐
        [SerializeField] private float squashAmount = 0.85f;

        [Header("Colors")]
        [SerializeField]
        private Color[] hexagonColors = new Color[]
        {
            new Color(0.3f, 0.76666653f, 1f, 1f),
            new Color(1f, 0.2980392f, 0.2980392f, 1f),
            new Color(0.52673274f, 0.8f, 0f, 1f),
            new Color(1f, 0.40784314f, 0.654902f, 1f),
            new Color(1f, 0.6431373f, 0.4f, 1f),
            new Color(0.57499135f, 0.25465754f, 0.9308176f, 1f)
        };

        [Header("Hold & Fade")]
        [SerializeField] private float holdDuration = 1.0f;
        [SerializeField] private float fadeOutDuration = 0.4f;

        private List<GameObject> hexagonObjects = new List<GameObject>();
        private List<Hexagon> hexagonComponents = new List<Hexagon>();
        private List<Vector3> targetPositions = new List<Vector3>();
        private List<Color> originalColors = new List<Color>();
        private bool isAnimating = false;
        private float nextCycleTime = 0f;

        private void Start()
        {
            if (animationParent == null)
            {
                animationParent = transform;
            }

            CreateHexagons();
            PlayCycle();
        }

        private void Update()
        {
            if (autoRepeat && isAnimating)
            {
                if (Time.time >= nextCycleTime)
                {
                    PlayCycle();
                }
            }
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        private void CreateHexagons()
        {
            ClearHexagons();

            // 스택 형태 위치 계산 (아래부터 위로 쌓임)
            int rows = Mathf.CeilToInt((float)hexagonCount / stackColumns);

            for (int i = 0; i < hexagonCount; i++)
            {
                Hexagon hexagon = Instantiate(hexagonPrefab, animationParent);

                // 초기 위치는 화면 밖 위쪽
                float randomX = Random.Range(-dropSpread, dropSpread);
                hexagon.transform.localPosition = new Vector3(randomX, dropStartY, 0f);
                hexagon.transform.localScale = Vector3.one;
                hexagon.transform.localRotation = Quaternion.identity;

                Color color = hexagonColors[i % hexagonColors.Length];
                Color transparentColor = color;
                transparentColor.a = 0f;
                hexagon.Color = transparentColor;

                hexagonObjects.Add(hexagon.gameObject);
                hexagonComponents.Add(hexagon);
                originalColors.Add(color);

                // 목표 위치: 스택 형태 (아래부터 차곡차곡)
                int layer = i / stackColumns; // 몇 번째 층인지
                int posInLayer = i % stackColumns; // 층에서의 위치

                float totalWidth = (stackColumns - 1) * hexagonWidth;
                float startX = -totalWidth / 2f;

                float targetX = startX + (posInLayer * hexagonWidth);
                float targetY = layer * (hexagonHeight + stackSpacing);

                Vector3 targetPos = new Vector3(targetX, targetY, 0f);
                targetPositions.Add(targetPos);
            }

            Logger.Log($"BootHexaAnimation: Created {hexagonCount} hexagons for stacking.");
        }

        [Button("Play Cycle")]
        public void PlayCycle()
        {
            if (hexagonObjects.Count == 0)
            {
                CreateHexagons();
            }

            StopAllTweens();
            isAnimating = true;
            nextCycleTime = Time.time + cycleDuration;

            PlayStackAnimation();
        }

        private void PlayStackAnimation()
        {
            for (int i = 0; i < hexagonObjects.Count; i++)
            {
                GameObject hexagonObj = hexagonObjects[i];
                Hexagon hexagon = hexagonComponents[i];
                if (hexagonObj == null || hexagon == null) continue;

                float delay = i * dropDelay;
                Vector3 targetPos = targetPositions[i];
                Color targetColor = originalColors[i];

                // 초기화
                float randomX = Random.Range(-dropSpread, dropSpread);
                hexagonObj.transform.localPosition = new Vector3(randomX, dropStartY, 0f);
                hexagonObj.transform.localScale = Vector3.one;
                hexagonObj.transform.localRotation = Quaternion.identity;

                // === 1단계: 페이드 인 ===
                LeanTween.value(hexagonObj, 0f, 1f, 0.2f)
                    .setDelay(delay)
                    .setOnUpdate((float alpha) =>
                    {
                        if (hexagon != null)
                        {
                            Color c = targetColor;
                            c.a = alpha;
                            hexagon.Color = c;
                        }
                    })
                    .setEase(LeanTweenType.easeOutQuad);

                // === 2단계: 낙하 (중력 가속도 느낌) ===
                LeanTween.moveLocal(hexagonObj, targetPos, dropDuration)
                    .setDelay(delay)
                    .setEase(LeanTweenType.easeInQuad);

                // 떨어지면서 회전
                if (useRotation)
                {
                    bool clockwise = (i % 2 == 0);
                    float rotation = clockwise ? rotationSpeed : -rotationSpeed;

                    LeanTween.rotateZ(hexagonObj, rotation, dropDuration)
                        .setDelay(delay)
                        .setEase(LeanTweenType.easeInQuad);
                }

                // === 3단계: 착지 임팩트 ===
                float landDelay = delay + dropDuration;

                // 찌그러짐 효과 (Squash & Stretch)
                if (useSquash)
                {
                    Vector3 squashScale = new Vector3(1.2f, squashAmount, 1f);
                    LeanTween.scale(hexagonObj, squashScale, 0.08f)
                        .setDelay(landDelay)
                        .setEase(LeanTweenType.easeOutQuad)
                        .setOnComplete(() =>
                        {
                            // 원래대로 복구
                            LeanTween.scale(hexagonObj, Vector3.one, 0.12f)
                                .setEase(LeanTweenType.easeOutBack);
                        });
                }

                // 글로우 효과 (착지 시 번쩍)
                if (useGlow)
                {
                    LeanTween.value(hexagonObj, 1f, glowIntensity, 0.1f)
                        .setDelay(landDelay)
                        .setOnUpdate((float intensity) =>
                        {
                            if (hexagon != null)
                            {
                                Color glowColor = targetColor * intensity;
                                glowColor.a = 1f;
                                hexagon.Color = glowColor;
                            }
                        })
                        .setEase(LeanTweenType.easeOutQuad)
                        .setLoopPingPong(1);
                }

                // === 4단계: 통통 튀기 (바운스) ===
                float currentBounceHeight = bounceHeight;
                float currentBounceTime = landDelay + 0.1f;

                for (int b = 0; b < bounceCount; b++)
                {
                    float bounceUpDelay = currentBounceTime;
                    float bounceDownDelay = bounceUpDelay + bounceDuration * 0.5f;

                    // 위로 튀어오름
                    LeanTween.moveLocalY(hexagonObj, targetPos.y + currentBounceHeight, bounceDuration * 0.5f)
                        .setDelay(bounceUpDelay)
                        .setEase(LeanTweenType.easeOutQuad);

                    // 다시 떨어짐
                    LeanTween.moveLocalY(hexagonObj, targetPos.y, bounceDuration * 0.5f)
                        .setDelay(bounceDownDelay)
                        .setEase(LeanTweenType.easeInQuad);

                    // 다음 바운스는 더 낮게
                    currentBounceHeight *= 0.5f;
                    currentBounceTime = bounceDownDelay + bounceDuration * 0.5f;
                }

                // 회전 멈춤
                float rotationStopDelay = landDelay + 0.2f;
                LeanTween.rotateZ(hexagonObj, 0f, 0.3f)
                    .setDelay(rotationStopDelay)
                    .setEase(LeanTweenType.easeOutBack);

                // === 5단계: 쌓인 채로 머무름 (미세한 떠다님) ===
                float settleDelay = landDelay + bounceDuration * bounceCount + 0.2f;
                float floatOffset = Random.Range(-0.05f, 0.05f);

                LeanTween.moveLocalY(hexagonObj, targetPos.y + floatOffset, holdDuration * 0.5f)
                    .setDelay(settleDelay)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setLoopPingPong();

                // === 6단계: 페이드 아웃 (위에서부터 순차적으로) ===
                // 위에 있는 헥사곤부터 사라지도록 순서 역순
                int fadeOrder = hexagonCount - 1 - i;
                float fadeDelay = settleDelay + holdDuration + (fadeOrder * 0.05f);

                LeanTween.value(hexagonObj, 1f, 0f, fadeOutDuration)
                    .setDelay(fadeDelay)
                    .setOnUpdate((float alpha) =>
                    {
                        if (hexagon != null)
                        {
                            Color c = targetColor;
                            c.a = alpha;
                            hexagon.Color = c;
                        }
                    })
                    .setEase(LeanTweenType.easeInQuad);

                // 사라지면서 위로 떠오름
                LeanTween.moveLocalY(hexagonObj, targetPos.y + 2f, fadeOutDuration)
                    .setDelay(fadeDelay)
                    .setEase(LeanTweenType.easeInQuad);

                LeanTween.scale(hexagonObj, Vector3.zero, fadeOutDuration)
                    .setDelay(fadeDelay)
                    .setEase(LeanTweenType.easeInBack);
            }
        }

        [Button("Start Animation")]
        public void StartAnimation()
        {
            autoRepeat = true;
            PlayCycle();
        }

        [Button("Stop Animation")]
        public void StopAnimation()
        {
            isAnimating = false;
            autoRepeat = false;
            StopAllTweens();
        }

        private void StopAllTweens()
        {
            foreach (GameObject hexagonObj in hexagonObjects)
            {
                if (hexagonObj != null)
                {
                    LeanTween.cancel(hexagonObj);
                }
            }
        }

        private void ClearHexagons()
        {
            StopAllTweens();

            foreach (GameObject hexagonObj in hexagonObjects)
            {
                if (hexagonObj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(hexagonObj);
                    }
                    else
                    {
                        DestroyImmediate(hexagonObj);
                    }
                }
            }

            hexagonObjects.Clear();
            hexagonComponents.Clear();
            targetPositions.Clear();
            originalColors.Clear();
        }

        private void OnValidate()
        {
            hexagonCount = Mathf.Max(1, hexagonCount);
            stackColumns = Mathf.Max(1, stackColumns);
            cycleDuration = Mathf.Max(0.5f, cycleDuration);
            dropDuration = Mathf.Max(0.1f, dropDuration);
            hexagonHeight = Mathf.Max(0.1f, hexagonHeight);
            hexagonWidth = Mathf.Max(0.1f, hexagonWidth);
        }
    }
}