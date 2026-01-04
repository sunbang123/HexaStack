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
        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, gridHexagonLayerMask);

        if (hit.collider == null)
            DraggingAboveGround();
        else
            DraggingAboveGridCell(hit);
    }

    private void DraggingAboveGround()
    {
        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, groundLayerMask);

        if(hit.collider == null)
        {
            Debug.LogError("No ground detected, this is unusual...");
            return;
        }

        Vector3 currentStackTargetPos = hit.point.With(y: currentStackInitialPos.y);

        // 마우스 위치에 즉시 따라가도록 변경
        currentStack.transform.position = currentStackTargetPos;

        ClearPreview();
        targetCell = null;
    }

    private void DraggingAboveGridCell(RaycastHit hit)
    {
        GridCell gridCell = hit.collider.GetComponent<GridCell>();

        if (gridCell.IsOccupied)
            DraggingAboveGround();
        else
            DraggingAboveNonOccupiedGridCell(gridCell);
    }

    private void DraggingAboveNonOccupiedGridCell(GridCell gridCell)
    {
        Vector3 currentStackTargetPos = gridCell.transform.position.With(y: 2);

        // 마우스 위치에 즉시 따라가도록 변경
        currentStack.transform.position = currentStackTargetPos;

        if (targetCell != gridCell)
        {
            ClearPreview();
            targetCell = gridCell;
            ShowPreview(gridCell);
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
