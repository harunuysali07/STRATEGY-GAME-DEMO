using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public static CellController Instance;

    public GameObject cellPrefab;

    public Transform content;

    [SerializeField]
    private int _verticalCellCount = 15;
    [SerializeField]
    private int _horizontalCellCount = 15;

    [SerializeField]
    private float _spacing = 10;
    private float _sizeOfCells = 0;

    [HideInInspector]
    public Cell[,] Cells;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cells = new Cell[_verticalCellCount, _horizontalCellCount];

        _sizeOfCells = cellPrefab.GetComponent<RectTransform>().rect.width;

        RectTransform _ContentRect = content.GetComponent<RectTransform>();
        _ContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _horizontalCellCount * (_sizeOfCells + _spacing) + _spacing);
        _ContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _verticalCellCount * (_sizeOfCells + _spacing) + _spacing);


        for (int i = 0; i < _verticalCellCount; i++)
        {
            for (int j = 0; j < _horizontalCellCount; j++)
            {
                Cell current = Instantiate(cellPrefab, content, false).GetComponent<Cell>();
                current.gameObject.transform.localPosition = new Vector2((j * (_sizeOfCells + _spacing) + _spacing + _sizeOfCells / 2) - _ContentRect.rect.width / 2, (i * (_sizeOfCells + _spacing) + _spacing + _sizeOfCells / 2) - _ContentRect.rect.height / 2);
                current.position = new Vector2Int(i, j);
                Cells[i, j] = current;
            }
        }
    }


    public List<Vector2Int> CheckCellsAvailability(Vector2Int center, Vector2Int size, bool isCapableofProduction = false)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        Vector2Int spawnPosition = new Vector2Int(-1, -1);

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                positions.Add(new Vector2Int(center.x + i, center.y + j));
            }
        }

        for (int i = -1; i < size.x + 1; i++)
        {
            for (int j = -1; j < size.y; j += size.y + 2)
            {
                if (CheckPositionAvailable(new Vector2Int(center.x + i, center.y + j)))
                {
                    spawnPosition = new Vector2Int(center.x + i, center.y + j);
                    break;
                }
            }

            if ((i == -1 || i == size.x) && spawnPosition == new Vector2Int(-1, -1))
            {
                for (int j = 0; j < size.y - 1; j++)
                {
                    if (CheckPositionAvailable(new Vector2Int(center.x + i, center.y + j)))
                    {
                        spawnPosition = new Vector2Int(center.x + i, center.y + j);
                        break;
                    }
                }
            }

            if (spawnPosition != new Vector2Int(-1, -1))
            {
                break;
            }
        }

        foreach (var position in positions)
        {
            if (!CheckPositionAvailable(position))
            {
                positions = new List<Vector2Int>();
                break;
            }
        }

        Debug.Log(spawnPosition + " : " + center + " : " + size);

        if (spawnPosition == new Vector2Int(-1, -1))
        {
            return positions;
        }
        else
        {
            positions.Insert(0, spawnPosition);
            return positions;
        }
    }

    private bool CheckPositionAvailable(Vector2Int position)
    {
        if (position.x < 0 || position.x >= _verticalCellCount || position.y < 0 || position.y >= _horizontalCellCount)
        {
            return false;
        }
        else if (Cells[position.x, position.y].OwnedByCell != null || Cells[position.x, position.y].OwnedCells.Count != 0)
        {
            return false;
        }
        else if (Cells.GetValue(position.x, position.y) == null || Cells[position.x, position.y].isSpawnPoint)
        {
            return false;
        }

        return true;
    }
}
