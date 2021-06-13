﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    public Image Item_image;
    public Text Item_text;
    public Button Item_button;

    /// <summary>
    /// Set Item's visible element's 
    /// </summary>
    /// <param name="Item"></param>
    public void SetItem(UnitData Item = null)
    {
        if (Item != null)
        {
            Item_image.enabled = true;
            Item_image.sprite = Item._Sprite;
            Item_text.text = Item._UnitName;

            Item_button.onClick.AddListener(delegate { GameController.Instance.InstatiateUnit(Item); });
        }
        else
        {
            Item_image.enabled = false;
            Item_image.sprite = Item._Sprite;
            Item_text.text = Item._UnitName;

            Item_button.onClick.RemoveAllListeners();
        }
    }
}
