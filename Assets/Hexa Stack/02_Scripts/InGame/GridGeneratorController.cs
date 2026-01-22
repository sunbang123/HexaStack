using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

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

            Core.Logger.Log($"[GridGenerator] {gridSize} 사이즈의 수학적 그리드 생성 완료.");
        }

        private void ExecuteCubeGeneration()
        {
            // [변경 1] Flat Top 기준의 치수 정의 (반대로 설정)
            // Flat Top은 너비가 size * 2, 높이가 size * sqrt(3)입니다.
            float flatWidth = hexSize * 2f;
            float flatHeight = hexSize * Mathf.Sqrt(3f);

            transform.Clear();

            for (int q = -gridSize; q <= gridSize; q++)
            {
                for (int r = -gridSize; r <= gridSize; r++)
                {
                    for (int s = -gridSize; s <= gridSize; s++)
                    {
                        // QRS 좌표계 조건 (변함 없음)
                        if (q + r + s != 0)
                            continue;

                        // [변경 2] 위치 계산 (Red Blob Games 표준 공식 적용)
                        // Flat Top에서는 수평(X) 위치가 q에 의해 결정되고,
                        // 수직(Z) 위치는 r과 q의 조합으로 결정됩니다.

                        float xPos = hexSize * (3f / 2f) * q;
                        float zPos = flatHeight * (r + q / 2f);

                        Vector3 spawnPos = new Vector3(xPos, 0, zPos);

                        GameObject gridHexInstance = Instantiate(hexagon);
                        gridHexInstance.transform.position = spawnPos;

                        // [변경 3] 프리팹 회전 (Y축 30도)
                        // Pointy Top 모델을 Flat Top으로 보이게 하려면 30도 회전이 필요합니다.
                        gridHexInstance.transform.rotation = Quaternion.Euler(0, 30, 0);
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
    }
}
