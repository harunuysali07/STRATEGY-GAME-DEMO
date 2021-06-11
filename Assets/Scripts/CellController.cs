using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public static CellController Instance;

    public GameObject cellPrefab;

    public Transform content;

    public int _verticalCellCount = 15;
    public int _horizontalCellCount = 15;

    [SerializeField]
    private float _spacing = 10;
    private float _sizeOfCells = 0;

    public Cell MovingCell;

    private float movementSpeed = .15f;
    private float movementTimer = 0;

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

    void Update()
    {
        if (movementTimer < 0)
        {
            if (MovingCell)
            {
                if (MovingCell.cellUnit.Path.Count > 0 && CheckPositionAvailable(MovingCell.cellUnit.Path[0]))
                {
                    var nextCell = Cells[MovingCell.cellUnit.Path[0].x, MovingCell.cellUnit.Path[0].y];
                    var lastCell = Cells[MovingCell.position.x, MovingCell.position.y];

                    nextCell.cellUnit = MovingCell.cellUnit;
                    nextCell.UpdateCellImage();
                    nextCell.cellUnit.Path.RemoveAt(0);

                    if (nextCell.cellUnit.Path.Count > 0)
                    {
                        MovingCell = nextCell;
                    }
                    else
                    {
                        MovingCell = null;
                    }

                    if (lastCell == GameController.Instance.currentlySelectedCell)
                    {
                        GameController.Instance.SelectCell(nextCell);
                    }

                    lastCell.cellUnit = null;
                    lastCell.UpdateCellImage();
                }
            }
            movementTimer = movementSpeed;
        }
        else
        {
            movementTimer -= Time.deltaTime;
        }
    }

    public List<Vector2Int> CheckCellsAvailability(Vector2Int center, Vector2Int size)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                positions.Add(new Vector2Int(center.x + i, center.y + j));
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

        return positions;
    }

    public Vector2Int FindSpawnPosition(Vector2Int center, Vector2Int size)
    {
        Vector2Int spawnPosition = new Vector2Int(-1, -1);

        for (int i = -1; i <= size.x; i++)
        {
            for (int j = -1; j <= size.y; j++)
            {
                if (CheckPositionAvailable(new Vector2Int(center.x + i, center.y + j)))
                {
                    spawnPosition = new Vector2Int(center.x + i, center.y + j);
                    break;
                }
            }

            if (spawnPosition != new Vector2Int(-1, -1))
            {
                break;
            }
        }

        return spawnPosition;
    }

    public bool CheckPositionAvailable(Vector2Int position)
    {
        if (position.x < 0 || position.x >= _verticalCellCount || position.y < 0 || position.y >= _horizontalCellCount)
        {
            return false;
        }
        else if (Cells[position.x, position.y].OwnedByCell != null || Cells[position.x, position.y].OwnedCells.Count != 0 || Cells[position.x, position.y].cellUnit != null)
        {
            return false;
        }
        else if (Cells.GetValue(position.x, position.y) == null)
        {
            return false;
        }

        return true;
    }

    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        foreach (var item in Cells)
        {
            item.gCost = 0;
            item.hCost = 0;
            item.parent = null;
        }

        List<Vector2Int> path = new List<Vector2Int>();

        Cell startNode = Cells[startPos.x, startPos.y];
        Cell targetNode = Cells[targetPos.x, targetPos.y];

        List<Cell> openSet = new List<Cell>();
        List<Cell> closeSet = new List<Cell>();

        openSet.Add(startNode);

        int tour = 0;

        while (openSet.Count > 0)
        {
            Cell currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            tour++;

            if (tour > 1000)
            {
                Debug.LogError("İnfinite");

                return path;
            }

            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                path = RetracePath(startNode, targetNode);
                break;
            }

            foreach (Cell neighbour in GetNeighbors(currentNode.position))
            {
                if (!CheckPositionAvailable(neighbour.position) || closeSet.Contains(neighbour))
                {
                    continue;
                }

                int newCost = currentNode.gCost + GetDistance(currentNode.position, neighbour.position);

                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour.position, targetNode.position);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return path;
    }

    public List<Vector2Int> RetracePath(Cell startNode, Cell endNode)
    {
        List<Vector2Int> pathR = new List<Vector2Int>();
        Cell currentNode = endNode;

        while (currentNode != startNode)
        {
            Vector2Int next = currentNode.position;
            pathR.Add(next);
            currentNode = currentNode.parent;
        }
        pathR.Reverse();

        return pathR;
    }

    public List<Cell> GetNeighbors(Vector2Int center)
    {
        List<Cell> neighbors = new List<Cell>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (CheckPositionAvailable(new Vector2Int(center.x + i, center.y + j)))
                {
                    neighbors.Add(Cells[center.x + i, center.y + j]);
                }
            }
        }

        return neighbors;
    }

    public List<Cell> GetNeighboursShortedByDistanceToTarget(Vector2Int center, Vector2Int target)
    {
        List<Cell> neighbors = GetNeighbors(target);

        foreach (var item in neighbors)
        {
            item.hCost = GetDistance(center, item.position);
        }

        neighbors = neighbors.OrderBy(x => x.hCost).ToList();

        return neighbors;
    }

    public int GetDistance(Vector2Int A, Vector2Int B)
    {
        int distX = Mathf.Abs(A.x - B.x);
        int distY = Mathf.Abs(A.y - B.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}

