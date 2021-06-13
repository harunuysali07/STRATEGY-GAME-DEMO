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

    [HideInInspector]
    public Cell[,] Cells;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    /// <summary>
    /// Create grid of cells with requested amount
    /// </summary>
    protected void CreateGrid()
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

    /// <summary>
    /// Get available cells around center
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <returns></returns>
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
                return positions;
            }
        }

        return positions;
    }

    /// <summary>
    /// Get empty cells around center
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Check if cell if available to fill or move
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Find shortest path with A* algortihm
    /// </summary>
    /// <param name="startPos">Start position of path</param>
    /// <param name="targetPos">Target position of path </param>
    /// <returns></returns>
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

    //Turn path to a vector2Int list
    protected List<Vector2Int> RetracePath(Cell startNode, Cell endNode)
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

    /// <summary>
    /// Get empty neighbours of a target - 1 by 1 around (count = 8 aprox.)
    /// </summary>
    /// <param name="center"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get neighbours that contains Unity with given UnitType
    /// </summary>
    /// <param name="center"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public List<Cell> GetNeighborsWithUnits(Vector2Int center, UnitType targetType)
    {
        List<Cell> neighbors = new List<Cell>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (center.x + i >= 0 && center.x + i < _verticalCellCount && center.y + j >= 0 && center.y + j < _horizontalCellCount && !(i == 0 && j == 0))
                {
                    if (!CheckPositionAvailable(new Vector2Int(center.x + i, center.y + j)))
                    {
                        if (Cells[center.x + i, center.y + j].cellUnit != null)
                        {
                            if (Cells[center.x + i, center.y + j].cellUnit.unitType == targetType)
                            {
                                neighbors.Add(Cells[center.x + i, center.y + j]);
                            }
                        }
                        else if (Cells[center.x + i, center.y + j].OwnedByCell != null)
                        {
                            if (Cells[center.x + i, center.y + j].OwnedByCell.cellUnit.unitType == targetType)
                            {
                                neighbors.Add(Cells[center.x + i, center.y + j]);
                            }
                        }
                    }
                }
            }
        }

        return neighbors;
    }

    /// <summary>
    /// Get empty neighbours of a target ordered by closest to target
    /// </summary>
    /// <param name="center"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public List<Cell> GetNeighboursofTarget(Vector2Int center, Vector2Int target)
    {
        List<Cell> neighbors = GetNeighbors(target);

        foreach (var item in neighbors)
        {
            item.hCost = GetDistance(center, item.position);
        }

        neighbors = neighbors.OrderBy(x => x.hCost).ToList();

        return neighbors;
    }

    /// <summary>
    /// Find closest Emeny Unity on all cells
    /// </summary>
    /// <param name="center"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public Cell GetClosestEnemyUnit(Vector2Int center, UnitType targetType)
    {
        List<Cell> neighbors = new List<Cell>();

        for (int i = -_verticalCellCount; i <= _verticalCellCount; i++)
        {
            for (int j = -_horizontalCellCount; j <= _horizontalCellCount; j++)
            {
                if (center.x + i >= 0 && center.x + i < _verticalCellCount && center.y + j >= 0 && center.y + j < _horizontalCellCount && !(i == 0 && j == 0))
                {
                    if (!CheckPositionAvailable(new Vector2Int(center.x + i, center.y + j)))
                    {
                        if (Cells[center.x + i, center.y + j].cellUnit != null)
                        {
                            if (Cells[center.x + i, center.y + j].cellUnit.unitType == targetType)
                            {
                                Cells[center.x + i, center.y + j].hCost = GetDistance(center, Cells[center.x + i, center.y + j].position);
                                neighbors.Add(Cells[center.x + i, center.y + j]);
                            }
                        }
                        else if (Cells[center.x + i, center.y + j].OwnedByCell != null)
                        {
                            if (Cells[center.x + i, center.y + j].OwnedByCell.cellUnit.unitType == targetType)
                            {
                                Cells[center.x + i, center.y + j].hCost = GetDistance(center, Cells[center.x + i, center.y + j].position);
                                neighbors.Add(Cells[center.x + i, center.y + j]);
                            }
                        }
                    }
                }
            }
        }

        neighbors = neighbors.OrderBy(x => x.hCost).ToList();

        if (neighbors.Count > 0)
        {
            return neighbors[0];
        }
        else
        {
            return Cells[center.x, center.y];
        }
    }

    /// <summary>
    /// Get distance between two cells.
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
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

