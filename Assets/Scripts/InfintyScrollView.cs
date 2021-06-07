﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfintyScrollView : MonoBehaviour
{
    public static InfintyScrollView Instance;

    public Transform content;

    [SerializeField]
    private List<RectTransform> Items;

    private void Awake()
    {
        Instance = this;

        Items = new List<RectTransform>();

        foreach (RectTransform item in content)
        {
            Items.Add(item);
        }

        if (Items.Count == 0)
        {
            throw new System.Exception("Add Items to Scroll List !");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ////HACK
        //int count = 0;
        //foreach (var item in content.GetComponentsInChildren<Text>())
        //{
        //    item.text = (++count).ToString();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItems(List<UnitDataScriptableObject> UnitList)
    {
        int Unitindex = -1;
        if (UnitList.Count < 1)
        {
            Debug.LogError("Unit list cannot be empty !");
            return;
        }

        foreach (var item in Items)
        {
            var listitems = item.GetComponentsInChildren<ListItem>();
            foreach (var listitem in listitems)
            {
                Unitindex++;
                if (Unitindex >= UnitList.Count)
                {
                    Unitindex = 0;
                }

                listitem.SetItem(UnitList[Unitindex]);
            }
        }
    }

    private float lastPositionY = 0;
    public void OnValueChange(Vector2 vector)
    {
        if (content.transform.localPosition.y > lastPositionY + 200)
        {
            lastPositionY += 200;
            var index = ((Items.Count - 1) + (Mathf.CeilToInt(lastPositionY / 200) - 1) % Items.Count) % Items.Count;
            Items[index].SetAsLastSibling();
            content.GetComponent<VerticalLayoutGroup>().padding.top += 200;
        }
        else if (content.transform.localPosition.y < lastPositionY - 200)
        {
            lastPositionY -= 200;
            var index = ((Items.Count - 1) + (Mathf.CeilToInt(lastPositionY / 200) + 1) % Items.Count) % Items.Count;
            Items[index].SetAsFirstSibling();
            content.GetComponent<VerticalLayoutGroup>().padding.top -= 200;
        }
    }
}
