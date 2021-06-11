using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitData : MonoBehaviour
{
    public float _Health;
    public float _Damage;

    public bool _isMovable;
    public float _AttackPerSecond;

    public string _UnitName;
    public Sprite _Sprite;

    [HideInInspector] public Cell targetCell;

    public Vector2Int Size;
    public List<Vector2Int> Path;

    public List<UnitData> ProducitonUnits;
}
