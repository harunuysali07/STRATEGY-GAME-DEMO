using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("Enemy Production")]
    public List<UnitData> enemyUnits;

    public float spawnTime = 5;
    private float spawnTimer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer < 0)
        {
            SpawnEnemy();
            spawnTimer = spawnTime;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    public void SpawnEnemy()
    {
        Vector2Int spawnPoint;

        List<Vector2Int> availableSpawnPoints = new List<Vector2Int>();

        for (int i = 0; i < CellController.Instance._verticalCellCount; i++)
        {
            availableSpawnPoints.Add(new Vector2Int(i, 0));
            availableSpawnPoints.Add(new Vector2Int(i, CellController.Instance._horizontalCellCount - 1));
        }

        for (int i = 1; i < CellController.Instance._horizontalCellCount - 1; i++)
        {
            availableSpawnPoints.Add(new Vector2Int(0, i));
            availableSpawnPoints.Add(new Vector2Int(CellController.Instance._verticalCellCount - 1, i));
        }

        for (int i = 0; i < availableSpawnPoints.Count; i++)
        {
            spawnPoint = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
            if (CellController.Instance.Cells.GetValue(spawnPoint.x, spawnPoint.y) != null && CellController.Instance.CheckPositionAvailable(spawnPoint))
            {
                CellController.Instance.Cells[spawnPoint.x, spawnPoint.y].SetUnitData(enemyUnits[Random.Range(0, enemyUnits.Count)]);
                CellController.Instance.Cells[spawnPoint.x, spawnPoint.y].unitType = UnitType.Enemy;
                CellController.Instance.Cells[spawnPoint.x, spawnPoint.y].UpdateCellImage();
                break;
            }
            else
            {
                availableSpawnPoints.Remove(spawnPoint);
            }
        }
    }
}
