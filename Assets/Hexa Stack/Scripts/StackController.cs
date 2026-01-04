using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackController : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private LayerMask hexagonLayerMask;
    [SerializeField] private LayerMask gridHexagonLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    private HexStack currentStack;
    private Vector3 currentStackInitialPos;

    [Header(" Data ")]
    private GridCell targetCell;
    private GridCell previousTargetCell;
    private MaterialPropertyBlock propertyBlock;


    [Header(" Actions ")]
    public static Action<GridCell> onStackPlaced;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ManageControl();
    }

    private void ManageControl()
    {
        if (Input.GetMouseButtonDown(0))
            ManageMouseDown();
        else if (Input.GetMouseButton(0) && currentStack != null)
            ManageMouseDrag();
        else if (Input.GetMouseButtonUp(0) && currentStack != null)
            ManageMouseUp();
    }


    private void ManageMouseDown()
    {

        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, hexagonLayerMask);

        if (hit.collider == null)
        {
            Debug.Log("We have not detected any hexagon");
            return;
        }

        currentStack = hit.collider.GetComponent<Hexagon>().HexStack;
        currentStackInitialPos = currentStack.transform.position;
    }


    private void ManageMouseDrag()
    {
        // 먼저 그라운드 레이캐스트로 마우스 위치 확인
        RaycastHit groundHit;
        Physics.Raycast(GetClickedRay(), out groundHit, 500, groundLayerMask);

        if (groundHit.collider == null)
        {
            Debug.LogError("No ground detected, this is unusual...");
            return;
        }

        // 마우스 위치를 부드럽게 따라가도록 함
        Vector3 mouseWorldPos = groundHit.point.With(y: currentStackInitialPos.y);
        currentStack.transform.position = Vector3.Lerp(
            currentStack.transform.position,
            mouseWorldPos,
            Time.deltaTime * 20f);

        // 가장 가까운 그리드 셀 찾기
        RaycastHit gridHit;
        Physics.Raycast(GetClickedRay(), out gridHit, 500, gridHexagonLayerMask);

        if (gridHit.collider != null)
        {
            GridCell gridCell = gridHit.collider.GetComponent<GridCell>();
            UpdateGridCellPreview(gridCell);
        }
        else
        {
            ClearPreview();
            targetCell = null;
        }
    }

    private void UpdateGridCellPreview(GridCell gridCell)
    {
        // 빈 자리일 때만 preview 표시 및 배치 가능
        if (!gridCell.IsOccupied)
        {
            if (targetCell != gridCell)
            {
                ClearPreview();
                targetCell = gridCell;
                ShowPreview(gridCell);
            }
        }
        else
        {
            // occupied된 셀 위에서는 preview 제거 및 배치 불가
            ClearPreview();
            targetCell = null;
        }
    }


    private void ManageMouseUp()
    {
        ClearPreview();

        if(targetCell == null)
        {
            currentStack.transform.position = currentStackInitialPos;
            currentStack = null;
            return;
        }

        currentStack.transform.position = targetCell.transform.position.With(y: .2f);
        currentStack.transform.SetParent(targetCell.transform);
        currentStack.Place();

        targetCell.AssignStack(currentStack);

        onStackPlaced?.Invoke(targetCell);

        targetCell = null;
        previousTargetCell = null;
        currentStack = null;
    }


    private Ray GetClickedRay() => Camera.main.ScreenPointToRay(Input.mousePosition);

    private void ShowPreview(GridCell gridCell)
    {
        if (gridCell == null)
            return;

        Renderer hexagonRenderer = gridCell.GetHexagonRenderer();
        if (hexagonRenderer != null)
        {
            previousTargetCell = gridCell;
            
            // MaterialPropertyBlock을 사용하여 Material 인스턴스를 생성하지 않고 색상 변경
            if (propertyBlock == null)
                propertyBlock = new MaterialPropertyBlock();
            
            hexagonRenderer.GetPropertyBlock(propertyBlock);
            
            Color currentColor = propertyBlock.HasColor("_Color") 
                ? propertyBlock.GetColor("_Color") 
                : hexagonRenderer.sharedMaterial.color;
            
            // Tint를 밝게 만들기 (HSV에서 밝기 증가)
            Color.RGBToHSV(currentColor, out float h, out float s, out float v);
            v = Mathf.Min(1f, v + 0.3f); // 밝기 30% 증가
            Color brighterColor = Color.HSVToRGB(h, s, v);
            
            propertyBlock.SetColor("_Color", brighterColor);
            hexagonRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void ClearPreview()
    {
        if (previousTargetCell != null)
        {
            Renderer hexagonRenderer = previousTargetCell.GetHexagonRenderer();
            if (hexagonRenderer != null)
            {
                // PropertyBlock을 제거하여 원본 Material로 복원
                hexagonRenderer.SetPropertyBlock(null);
            }
        }
        
        previousTargetCell = null;
    }
}
