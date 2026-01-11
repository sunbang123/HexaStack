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

    //private void GenerateGrid()
    //{
    //    transform.Clear();

    //    for (int x = -gridSize; x <= gridSize; x++)
    //    {
    //        for (int y = -gridSize; y <= gridSize; y++)
    //        {
    //            Vector3 spawnPos = grid.CellToWorld(new Vector3Int(x, y, 0));

    //            if (spawnPos.magnitude > grid.CellToWorld(new Vector3Int(1, 0, 0)).magnitude * gridSize)
    //                continue;

    //            GameObject gridHexInstance = (GameObject)PrefabUtility.InstantiatePrefab(hexagon);
    //            gridHexInstance.transform.position = spawnPos;
    //            gridHexInstance.transform.rotation = Quaternion.identity;
    //            gridHexInstance.transform.SetParent(transform);

    //            //Instantiate(hexagon, spawnPos, Quaternion.identity, transform);
    //        }
    //    }
    //}

    private void GenerateGrid()
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
                    if(q + r + s != 0)
                        continue;

                    Vector3 qDirection = Quaternion.Euler(0,60,0) * Vector3.right;
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
    }
}
#endif
