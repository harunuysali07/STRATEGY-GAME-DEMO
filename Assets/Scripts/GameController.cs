using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Production")]
    public List<UnitData> ProductionUnits;

    [Header("Information")]
    public Image InformationImage;
    public Text InformationName;
    public List<Button> TroopProductionButtons;

    [Header("UI")]
    public GameObject warningPanel;
    public Text warningText;

    [Space(20)]
    [HideInInspector]
    public Cell currentlySelectedCell;

    private Coroutine warningCoroutine;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InfintyScrollView.Instance.SetItems(ProductionUnits);
    }

    public void SelectCell(Cell _SelectedCell)
    {
        currentlySelectedCell?.SelectThisCell(false);
        _SelectedCell.SelectThisCell();

        currentlySelectedCell = _SelectedCell;

        if (currentlySelectedCell.cellUnit != null)
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
                if (CellController.Instance.CheckPositionAvailable(_TargetCell.position))
                {
                    //Move to an empty cell
                    currentlySelectedCell.cellUnit.Path = CellController.Instance.FindPath(currentlySelectedCell.position, _TargetCell.position);
                }
                else
                {
                    if (currentlySelectedCell.cellUnit._Damage > 0 && CellController.Instance.Cells[_TargetCell.position.x, _TargetCell.position.y].cellUnit.unitType == UnitType.Enemy)
                    {
                        currentlySelectedCell.cellUnit.targetCell = _TargetCell;
                    }

                    if (CellController.Instance.GetDistance(currentlySelectedCell.position, _TargetCell.position) > 15)
                    {
                        var neighbours = CellController.Instance.GetNeighboursofTarget(currentlySelectedCell.position, _TargetCell.position);

                        foreach (var item in neighbours)
                        {
                            if (CellController.Instance.CheckPositionAvailable(item.position))
                            {
                                currentlySelectedCell.cellUnit.Path = CellController.Instance.FindPath(currentlySelectedCell.position, item.position);

                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void InstatiateUnit(UnitData Unit)
    {
        if (currentlySelectedCell)
        {
            List<Vector2Int> AvailableCells = CellController.Instance.CheckCellsAvailability(currentlySelectedCell.position, Unit.Size);
            if (AvailableCells.Count != Unit.Size.x * Unit.Size.y)
            {
                warningCoroutine = StartCoroutine(ShowWarning("Selected Cells Not Available !!"));
            }
            else
            {
                foreach (var position in AvailableCells)
                {
                    if (currentlySelectedCell.position == position)
                    {
                        currentlySelectedCell.OwnedCells = AvailableCells;
                        currentlySelectedCell.SetUnitData(Unit);

                        currentlySelectedCell.UpdateCellImage();
                    }
                    else
                    {
                        CellController.Instance.Cells[position.x, position.y].OwnedByCell = currentlySelectedCell;
                        CellController.Instance.Cells[position.x, position.y].UpdateCellImage();
                    }
                }

                SelectCell(currentlySelectedCell);
            }
        }
        else
        {
            warningCoroutine = StartCoroutine(ShowWarning("Select an empty cell first !!"));
        }
    }

    public void ProduceTroops(UnitData Troops)
    {
        Vector2Int spawnPoint = CellController.Instance.FindSpawnPosition(currentlySelectedCell.position, currentlySelectedCell.cellUnit.Size);

        if (spawnPoint == new Vector2Int(-1, -1))
        {
            warningCoroutine = StartCoroutine(ShowWarning("This product unabled to produce !"));
        }
        else
        {
            if (CellController.Instance.Cells.GetValue(spawnPoint.x, spawnPoint.y) != null && CellController.Instance.CheckPositionAvailable(spawnPoint))
            {
                CellController.Instance.Cells[spawnPoint.x, spawnPoint.y].SetUnitData(Troops);
                CellController.Instance.Cells[spawnPoint.x, spawnPoint.y].UpdateCellImage();
            }
        }
    }

    IEnumerator ShowWarning(string warning)
    {
        warningPanel.SetActive(true);
        warningText.text = warning;

        yield return new WaitForSeconds(3);

        warningPanel.SetActive(false);
    }

    public void OkayButton()
    {
        StopCoroutine(warningCoroutine);
        warningPanel.SetActive(false);
    }
}
