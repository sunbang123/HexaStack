using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace HexaStack.Controllers
{
    public class StackSpawnerController : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform stackPositionsParent;
    [SerializeField] private Hexagon hexagonPrefab;
    [SerializeField] private HexStack hexagonStackPrefab;

    [Header(" Settings ")]
    [NaughtyAttributes.MinMaxSlider(2, 8)]
    [SerializeField] private Vector2Int minMaxHexCount;
    [SerializeField] private Color[] colors;
    private int stackCounter;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        StackController.onStackPlaced += StackPlacedCallback;
    }

    private void OnDestroy()
    {
        StackController.onStackPlaced -= StackPlacedCallback;
    }

    private void StackPlacedCallback(GridCell gridCell)
    {
        stackCounter++;

        if(stackCounter >= 3)
        {
            stackCounter = 0;
            GenerateStacks();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateStacks();
    }

    private void GenerateStacks()
    {
        for (int i = 0; i < stackPositionsParent.childCount; i++)
            GenerateStack(stackPositionsParent.GetChild(i));
    }

    private void GenerateStack(Transform parent)
    {
        // [변경 1] Flat Top 그리드에 맞추기 위해 30도 회전 쿼터니언 정의
        Quaternion flatTopRotation = Quaternion.Euler(0, 30, 0);

        // [변경 2] 스택 컨테이너(부모) 생성 시 30도 회전 적용
        // Quaternion.identity -> flatTopRotation으로 변경
        HexStack hexStack = Instantiate(hexagonStackPrefab, parent.position, flatTopRotation, parent);

        hexStack.name = $"Stack { parent.GetSiblingIndex() }";

        int amount = Random.Range(minMaxHexCount.x, minMaxHexCount.y);

        int firstColorHexagonCount = Random.Range(0, amount);

        Color[] colorArray = GetRandomColors();

        for (int i = 0; i < amount; i++)
        {
            Vector3 hexagonLocalPos = Vector3.up * i * .2f;
            Vector3 spawnPosition = hexStack.transform.TransformPoint(hexagonLocalPos);

            // [변경 3] 개별 육각형 생성 시에도 부모의 회전값(30도)을 적용
            // Quaternion.identity -> hexStack.transform.rotation (또는 flatTopRotation)
            Hexagon hexagonInstance = Instantiate(hexagonPrefab, spawnPosition, hexStack.transform.rotation, hexStack.transform);
            hexagonInstance.Color = i < firstColorHexagonCount ? colorArray[0] : colorArray[1];

            hexagonInstance.Configure(hexStack);

            hexStack.Add(hexagonInstance);
        }
    }

    private Color[] GetRandomColors()
    {
        List<Color> colorList = new List<Color>();
        colorList.AddRange(colors);

        if(colorList.Count <= 0)
        {
            Debug.LogError("No color found");
            return null;
        }

        Color firstColor = colorList.OrderBy(x => Random.value).First();
        colorList.Remove(firstColor);

        if (colorList.Count <= 0)
        {
            Debug.LogError("Only one color was found");
            return null;
        }

        Color secondColor = colorList.OrderBy(x => Random.value).First();

        return new Color[] { firstColor, secondColor };
    }
    }
}
