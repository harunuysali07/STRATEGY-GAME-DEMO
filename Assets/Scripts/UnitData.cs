using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitDataScriptableObject : ScriptableObject
{
    public float _Health;
    public float _Damage;

    public string _UnitName;
    public Sprite _Sprite;

    public Vector2Int Size;

    public List<UnitDataScriptableObject> ProducitonUnits;
}
