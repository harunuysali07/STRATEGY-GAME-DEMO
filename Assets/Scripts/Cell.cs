using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int position;

    public GameObject selectedImage;
    public Image cellImage;

    public UnitData cellUnit;

    public Cell OwnedByCell;
    public List<Vector2Int> OwnedCells;

    [HideInInspector] public int gCost;
    [HideInInspector] public int hCost;
    [HideInInspector] public int fCost { get { return gCost + hCost; } }
    [HideInInspector] public Cell parent;

    private Sprite cellDefaultSprite;

    private void Start()
    {
        cellDefaultSprite = cellImage.sprite;
    }

    public void SetUnitData(UnitData _Unit)
    {
        cellUnit = Instantiate(_Unit);
    }

    private void Update()
    {
        if (cellUnit != null)
        {
            if (cellUnit._Health <= 0)
            {
                cellUnit = null;
                UpdateCellImage();

                foreach (var item in OwnedCells)
                {
                    CellController.Instance.Cells[item.x, item.y].OwnedByCell = null;
                    CellController.Instance.Cells[item.x, item.y].UpdateCellImage();
                }

                OwnedCells = new List<Vector2Int>();

                if (GameController.Instance.currentlySelectedCell == this)
                {
                    GameController.Instance.SelectCell(this);
                }
            }
            else if (cellUnit.Path.Count > 0 && CellController.Instance.CheckPositionAvailable(cellUnit.Path[0]))
            {
                if (cellUnit._movementTimer < 0)
                {
                    cellUnit._movementTimer = cellUnit._movementSpeed;

                    var nextCell = CellController.Instance.Cells[cellUnit.Path[0].x, cellUnit.Path[0].y];

                    nextCell.cellUnit = cellUnit;
                    nextCell.UpdateCellImage();
                    nextCell.cellUnit.Path.RemoveAt(0);

                    if (GameController.Instance.currentlySelectedCell == this)
                    {
                        GameController.Instance.SelectCell(nextCell);
                    }

                    cellUnit = null;
                    UpdateCellImage();
                }
                else
                {
                    cellUnit._movementTimer -= Time.deltaTime;
                }
            }
            else if (cellUnit.targetCell != null && cellUnit._Damage > 0 && CellController.Instance.GetDistance(position, cellUnit.targetCell.position) < 15)
            {
                if (cellUnit.targetCell.cellUnit != null || cellUnit.targetCell.OwnedByCell?.cellUnit != null)
                {
                    if (cellUnit._hitTimer < 0)
                    {
                        Attack();
                        cellUnit._hitTimer = 1f / cellUnit._AttackPerSecond;
                    }
                    else
                    {
                        cellUnit._hitTimer -= Time.deltaTime;
                    }
                }
            }
            else
            {
                if (cellUnit.unitType == UnitType.Ally)
                {
                    var targets = CellController.Instance.GetNeighborsWithUnits(position, UnitType.Enemy);
                    if (targets.Count  > 0)
                    {
                        cellUnit.targetCell = targets[0];
                    }
                }
                else if (cellUnit.unitType == UnitType.Enemy)
                {
                    var targets = CellController.Instance.GetNeighborsWithUnits(position, UnitType.Ally);
                    if (targets.Count > 0)
                    {
                        cellUnit.targetCell = targets[0];
                    }
                }
            }
        }
    }

    private void Attack()
    {
        if (cellUnit.targetCell.cellUnit != null)
        {
            if (cellUnit.targetCell.cellUnit._Health > 0)
            {
                cellUnit.targetCell.cellUnit._Health -= cellUnit._Damage;

                if (cellUnit.targetCell.cellUnit._Health <= 0)
                {
                    cellUnit.targetCell = null;
                }
            }
            else
            {
                cellUnit.targetCell = null;
            }
        }
        else if(cellUnit.targetCell.OwnedByCell.cellUnit != null)
        {
            if (cellUnit.targetCell.OwnedByCell.cellUnit._Health > 0)
            {
                cellUnit.targetCell.OwnedByCell.cellUnit._Health -= cellUnit._Damage;
                //print(cellUnit._UnitName + " Attaced to : " + cellUnit.targetCell.OwnedByCell.cellUnit._UnitName + " Health = " + cellUnit.targetCell.OwnedByCell.cellUnit._Health);

                if (cellUnit.targetCell.OwnedByCell.cellUnit._Health <= 0)
                {
                    cellUnit.targetCell = null;
                }
            }
            else
            {
                cellUnit.targetCell = null;
            }
        }
    }

    public void OnButtonPress()
    {
        if (OwnedByCell)
        {
            OwnedByCell.OnButtonPress();
        }
        else
        {
            if (cellUnit == null)
            {
                GameController.Instance.SelectCell(this);
            }
            else if (cellUnit.unitType == UnitType.Ally)
            {
                GameController.Instance.SelectCell(this);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameController.Instance.TargetCell(this);
        }
    }

    /// <summary>
    /// Select cell while its empty
    /// </summary>
    public void SelectThisCell(bool _selected = true)
    {
        selectedImage.SetActive(_selected);
    }

    public void UpdateCellImage()
    {
        if (OwnedByCell)
        {
            cellImage.sprite = OwnedByCell.cellImage.sprite;
        }
        else if (cellUnit != null)
        {
            cellImage.sprite = cellUnit._Sprite;
        }
        else
        {
            cellImage.sprite = cellDefaultSprite;
        }
    }
}