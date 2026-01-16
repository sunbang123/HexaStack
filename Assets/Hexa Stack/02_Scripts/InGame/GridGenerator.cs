using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
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
            float height = hexSize * 2;
            float width = hexSize * Mathf.Sqrt(3);

            transform.Clear();

            for (int q = -gridSize; q <= gridSize; q++)
            {
                for (int r = -gridSize; r <= gridSize; r++)
                {
                    for (int s = -gridSize; s <= gridSize; s++)
                    {
                        if (q + r + s != 0)
                            continue;

                        Vector3 qDirection = Quaternion.Euler(0, 60, 0) * Vector3.right;
                        Vector3 rDirection = Vector3.back;
                        Vector3 sDirection = Quaternion.Euler(0, 120, 0) * Vector3.right;

                        Vector3 spawnPos =
                            rDirection * r * height * 1.5f +
                            qDirection * q * width +
                            sDirection * s * width;

                        GameObject gridHexInstance = (GameObject)PrefabUtility.InstantiatePrefab(hexagon);
                        gridHexInstance.transform.position = spawnPos;
                        gridHexInstance.transform.rotation = Quaternion.identity;
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
#endif
