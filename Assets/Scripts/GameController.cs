using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;


    private Cell currentlySelectedCell;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
