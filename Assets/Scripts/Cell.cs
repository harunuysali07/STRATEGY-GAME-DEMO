﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public Vector2 position;

    public GameObject selectedImage;
    public Image cellImage;

    private bool isSelected = false;
    public void OnButtonPress()
    {
        GameController.Instance.SelectCell(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click" + gameObject.name);
        }
    }

    /// <summary>
    /// Select cell while its empty
    /// </summary>
    public void SelectThisCell(bool _selected = true)
    {
        selectedImage.SetActive(_selected);
        isSelected = _selected;
    }
}