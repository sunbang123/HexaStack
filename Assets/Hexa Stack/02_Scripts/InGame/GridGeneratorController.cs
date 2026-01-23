using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor; // Handles를 쓰기 위해 필요
#endif

namespace HexaStack.Controllers
{
    public class GridGeneratorController : MonoBehaviour
    {
        [Header(" Elements ")]
        [SerializeField] private Grid grid;
        [SerializeField] private GameObject hexagon;

        [Header(" Settings ")]
        [OnValueChanged("GenerateGrid")]
        [SerializeField] private int gridSize;
        [SerializeField] private float hexSize;

        /// <summary>
        /// 외부(LevelController 등)에서 그리드 생성을 요청할 때 사용하는 공식 입구
        /// </summary>
        public void GenerateGrid()
        {
            ClearGrid();

            ExecuteCubeGeneration();

            // Core.Logger가 없으면 일반 Debug.Log로 대체 (에러 방지용)
            Debug.Log($"[GridGenerator] {gridSize} 사이즈의 수학적 그리드 생성 완료.");
        }
        private Vector3 CalculateLocalPos(int q, int r, int s)
        {
            //// Pointy Top 계산식
            //float height = hexSize * 2;
            //float width = hexSize * Mathf.Sqrt(3);

            //Vector3 qDirection = Quaternion.Euler(0, 60, 0) * Vector3.right;
            //Vector3 rDirection = Vector3.back;
            //Vector3 sDirection = Quaternion.Euler(0, 120, 0) * Vector3.right;

            //// [핵심] spawnOffset = q벡터 + r벡터 + s벡터의 합으로 계산
            //Vector3 spawnOffset =
            //    rDirection * r * height * 1.5f +
            //    qDirection * q * width +
            //    sDirection * s * width;

            //return spawnOffset;

            // [핵심] Flat Top은 "가로(Width)"가 더 김 (반지름 * 2)
            float flatHeight = hexSize * Mathf.Sqrt(3f);

            // [핵심] Flat Top 좌표 표준 공식 (Red Blob Games)
            // X축: q(열)가 1개 늘어날 때마다 지름의 3/4(1.5배)만큼 이동
            float x = hexSize * (1.5f) * q;
            // Z축: r(행)에 따라 높이 이동 + q(열)에 따라 반 칸씩(0.5) 지그재그 보정
            float z = flatHeight * (r + q / 2f);

            return new Vector3(x, 0, z);
        }

        // ==================================================================================
        // 2. 실제 헥사곤 오브젝트 생성 (버튼/값 변경 시 실행)
        // ==================================================================================
        private void ExecuteCubeGeneration()
        {
            transform.Clear();

            for (int q = -gridSize; q <= gridSize; q++)
            {
                for (int r = -gridSize; r <= gridSize; r++)
                {
                    for (int s = -gridSize; s <= gridSize; s++)
                    {
                        if (q + r + s != 0)
                            continue;

                        // 공통 함수로 위치 계산
                        Vector3 spawnPos = CalculateLocalPos(q, r, s);

                        GameObject gridHexInstance = Instantiate(hexagon);

                        // 로컬 좌표를 월드 좌표로 변환 (부모가 이동했을 때 대비)
                        gridHexInstance.transform.position = transform.position + spawnPos;
                        // gridHexInstance.transform.rotation = Quaternion.identity; // Pointy Top
                        gridHexInstance.transform.rotation = Quaternion.Euler(0, 30, 0); // Flat Top
                        gridHexInstance.transform.SetParent(transform);
                    }
                }
            }
        }

        private void ClearGrid()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        // ==================================================================================
        // 3. 씬 뷰 시각화 (유니티가 매 프레임 자동 호출)
        // ==================================================================================
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 스타일 설정
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.yellow;
            style.fontSize = 12; // 글자 크기 적당히 조절
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;

            int order = 0;

            // QRS 루프 로직
            for (int q = -gridSize; q <= gridSize; q++)
            {
                for (int r = -gridSize; r <= gridSize; r++)
                {
                    for (int s = -gridSize; s <= gridSize; s++)
                    {
                        if (q + r + s != 0)
                            continue;

                        // 공통 함수로 위치 계산 (로직 통일)
                        Vector3 localPos = CalculateLocalPos(q, r, s);

                        // Gizmos는 월드 좌표 기준이므로 transform.position 더하기
                        Vector3 worldPos = transform.position + localPos;

                        // 순서와 좌표 텍스트 표시
                        string label = $"{order}\n({q},{r},{s})";
                        Handles.Label(worldPos, label, style);

                        order++;
                    }
                }
            }
        }
#endif
    }
}