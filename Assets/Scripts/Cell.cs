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
    public UnitType unitType;

    public Cell OwnedByCell;
    public List<Vector2Int> OwnedCells;

    [HideInInspector] public int gCost;
    [HideInInspector] public int hCost;
    [HideInInspector] public int fCost { get { return gCost + hCost; } }
    [HideInInspector] public Cell parent;

    private Sprite cellDefaultSprite;

    private float attackTimer;

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
            if (cellUnit._Health < 0)
            {
                cellUnit = null;
                UpdateCellImage();

                foreach (var item in OwnedCells)
                {
                    CellController.Instance.Cells[item.x, item.y].OwnedByCell = null;
                }

                OwnedCells = new List<Vector2Int>();
            }
            else if(cellUnit.targetCell != null && CellController.Instance.GetDistance(position, cellUnit.targetCell.position) < 15)
            {
                if (attackTimer < 0)
                {
                    Attack();
                    attackTimer = 1f / cellUnit._AttackPerSecond;
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                }
            }
        }
    }

    private void Attack()
    {
        if (cellUnit.targetCell.cellUnit._Health > 0)
        {
            cellUnit.targetCell.cellUnit._Health -= cellUnit._Damage;
            print(cellUnit._UnitName + " Attaced to : " + cellUnit.targetCell.cellUnit._UnitName + " Health = " + cellUnit.targetCell.cellUnit._Health);

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
            else if(unitType == UnitType.Ally)
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

public enum UnitType
{
    Ally,
    Neutral,
    Enemy
}