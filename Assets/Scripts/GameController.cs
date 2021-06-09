using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Production")]
    public List<UnitDataScriptableObject> ProductionUnits;

    [Header("Information")]
    public Image InformationImage;
    public Text InformationName;

    public List<Button> TroopProductionButtons;

    [Space(20)]
    [HideInInspector]
    public Cell currentlySelectedCell;
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

        if (currentlySelectedCell.cellUnit)
        {
            if (!InformationImage.isActiveAndEnabled)
            {
                InformationImage.enabled = true;
            }

            InformationImage.sprite = currentlySelectedCell.cellUnit._Sprite;
            InformationName.text = currentlySelectedCell.cellUnit._UnitName;

            int troopIndex = 0;
            foreach (var troop in currentlySelectedCell.cellUnit.ProducitonUnits)
            {
                if (troopIndex < TroopProductionButtons.Count)
                {
                    TroopProductionButtons[troopIndex].GetComponent<Image>().sprite = troop._Sprite;
                    TroopProductionButtons[troopIndex].GetComponent<Image>().enabled = true;
                    TroopProductionButtons[troopIndex].GetComponentInChildren<Text>().text = troop._UnitName;

                    TroopProductionButtons[troopIndex].onClick.AddListener(delegate { ProduceTroops(troop); });

                    troopIndex++;
                }
            }

            for (int i = troopIndex; i < TroopProductionButtons.Count; i++)
            {
                TroopProductionButtons[i].GetComponent<Image>().sprite = null;
                TroopProductionButtons[i].GetComponent<Image>().enabled = false;
                TroopProductionButtons[i].GetComponentInChildren<Text>().text = "";

                TroopProductionButtons[i].onClick.RemoveAllListeners();
            }
        }
        else
        {
            InformationImage.enabled = false;
            InformationName.text = "";

            for (int i = 0; i < TroopProductionButtons.Count; i++)
            {
                TroopProductionButtons[i].GetComponent<Image>().sprite = null;
                TroopProductionButtons[i].GetComponent<Image>().enabled = false;
                TroopProductionButtons[i].GetComponentInChildren<Text>().text = "";

                TroopProductionButtons[i].onClick.RemoveAllListeners();
            }
        }
    }

    public void TargetCell(Cell _TargetCell)
    {
        if (currentlySelectedCell.cellUnit != null)
        {
            if (currentlySelectedCell.cellUnit._isMovable)
            {
                currentlySelectedCell.cellUnit.Path = CellController.Instance.FindPath(currentlySelectedCell.position, _TargetCell.position);

                //if (CellController.Instance.MovingList.Contains(currentlySelectedCell))
                //{
                //    CellController.Instance.MovingList.Remove(currentlySelectedCell);
                //}
                CellController.Instance.MovingCell = currentlySelectedCell;
            }
        }
    }

    public void InstatiateUnit(UnitDataScriptableObject Unit)
    {
        if (currentlySelectedCell)
        {
            List<Vector2Int> AvailableCells = CellController.Instance.CheckCellsAvailability(currentlySelectedCell.position, Unit.Size);
            if (AvailableCells.Count != Unit.Size.x * Unit.Size.y)
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

                        currentlySelectedCell.UpdateCellImage();
                    }
                    else
                    {
                        CellController.Instance.Cells[position.x, position.y].OwnedByCell = currentlySelectedCell;
                        CellController.Instance.Cells[position.x, position.y].UpdateCellImage();
                    }
                }
            }

            SelectCell(currentlySelectedCell);
        }
        else
        {
            Debug.LogError("Önce bi hücre seçiniz !!");
        }
    }

    public void ProduceTroops(UnitDataScriptableObject Troops)
    {
        Vector2Int spawnPoint = CellController.Instance.FindSpawnPosition(currentlySelectedCell.position, currentlySelectedCell.cellUnit.Size);

        if (spawnPoint == new Vector2Int(-1, -1))
        {
            Debug.LogError("This product unabled to produce !");
        }
        else
        {
            if (CellController.Instance.Cells.GetValue(spawnPoint.x, spawnPoint.y) != null && CellController.Instance.CheckPositionAvailable(spawnPoint))
            {
                CellController.Instance.Cells[spawnPoint.x, spawnPoint.y].cellUnit = Troops;
                CellController.Instance.Cells[spawnPoint.x, spawnPoint.y].UpdateCellImage();
            }
        }
    }

    public void MoveCell()
    {

    }
}
