using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public List<UnitDataScriptableObject> ProductionUnits;

    [SerializeField]
    private Cell currentlySelectedCell;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InfintyScrollView.Instance.SetItems(ProductionUnits);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectCell(Cell _SelectedCell)
    {

        currentlySelectedCell?.SelectThisCell(false);
        _SelectedCell.SelectThisCell();

        currentlySelectedCell = _SelectedCell;
    }

    public void InstatiateUnit(UnitDataScriptableObject Unit)
    {
        if (currentlySelectedCell)
        {
            List<Vector2Int> AvailableCells = CellController.Instance.CheckCellsAvailability(currentlySelectedCell.position, Unit.Size, isCapableofProduction: Unit.ProducitonUnits.Count > 0);
            if (AvailableCells.Count == 0)
            {
                Debug.LogError("Selected Cells Not Available !");
            }
            else
            {
                foreach (var position in AvailableCells)
                {
                    if (currentlySelectedCell.position == position)
                    {
                        currentlySelectedCell.OwnedCells = AvailableCells;
                        currentlySelectedCell.cellUnit = Unit;

                        currentlySelectedCell.cellImage.sprite = Unit._Sprite;
                    }
                    else
                    {
                        CellController.Instance.Cells[position.x, position.y].OwnedByCell = currentlySelectedCell;
                        CellController.Instance.Cells[position.x, position.y].cellImage.sprite = Unit._Sprite;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Önce bi hücre seçiniz !!");
        }
    }
}
