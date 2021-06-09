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

    public UnitDataScriptableObject cellUnit;
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

    private void Update()
    {
        if (cellUnit)
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
            else
            {
                if (attackTimer < 0)
                {
                    foreach (var item in CellController.Instance.GetNeighbors(position))
                    {
                        if (item.cellUnit)
                        {

                        }
                    }
                    attackTimer = 1f / cellUnit._AttackPerSecond;
                }
                else
                {
                    attackTimer -= Time.deltaTime;
                }
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
            GameController.Instance.SelectCell(this);
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
        else if (cellUnit)
        {
            cellImage.sprite = cellUnit._Sprite;
        }
        else
        {
            cellImage.sprite = cellDefaultSprite;
        }
    }
}
