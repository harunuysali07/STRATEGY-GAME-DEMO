using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public GameObject cellPrefab;

    public Transform content;

    [SerializeField]
    private int _verticalCellCount = 15;
    [SerializeField]
    private int _horizontalCellCount = 15;

    [SerializeField]
    private float _spacing = 10;
    [SerializeField]
    private float _sizeOfCells = 100;
    // Start is called before the first frame update
    void Start()
    {
        RectTransform _ContentRect = content.GetComponent<RectTransform>();
        _ContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _horizontalCellCount * (_sizeOfCells + _spacing) + _spacing);
        _ContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _verticalCellCount * (_sizeOfCells + _spacing) + _spacing);


        for (int i = 0; i < _verticalCellCount; i++)
        {
            for (int j = 0; j < _horizontalCellCount; j++)
            {
                Instantiate(cellPrefab, content, false).transform.localPosition = new Vector2((j * (_sizeOfCells + _spacing) + _spacing + _sizeOfCells / 2) - _ContentRect.rect.width / 2, (i * (_sizeOfCells + _spacing) + _spacing + _sizeOfCells / 2) - _ContentRect.rect.height / 2);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
