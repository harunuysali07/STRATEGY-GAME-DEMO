using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfintyScrollView : MonoBehaviour
{
    public Transform content;

    [SerializeField]
    private List<RectTransform> Items;

    private float itemsHeight;
    // Start is called before the first frame update
    void Start()
    {
        Items = new List<RectTransform>();

        foreach (RectTransform item in content)
        {
            Items.Add(item);
        }

        if (Items.Count == 0)
        {
            throw new System.Exception("Add Items to Scroll List !");
        }

        itemsHeight = Items[0].rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float lastPositionY;
    public void OnValueChange(Vector2 vector)
    {
        if (lastPositionY > vector.y + 2)
        {
            lastPositionY = vector.y;
        }

        print(vector.y);
    }

    void ArrangeList()
    {

    }
}
