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

    public float _movementSpeed = .15f;

    [HideInInspector] public float _movementTimer;
    [HideInInspector] public float _hitTimer;

    [HideInInspector] public Cell targetCell;

    public Vector2Int Size;
    public List<Vector2Int> Path;

    public List<UnitData> ProducitonUnits;

    public UnitType unitType;

    /// <summary>
    /// Constructor methof for UnitData class
    /// </summary>
    public UnitData()
    {
        _hitTimer = 1f / _AttackPerSecond;
        _movementTimer = _movementSpeed;
    }
}


public enum UnitType
{
    Ally,
    Enemy,
}
