using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        //TODO 
        int count = 0;
        foreach (var item in content.GetComponentsInChildren<Text>())
        {
            item.text = (++count).ToString(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float lastPositionY = 0;
    public void OnValueChange(Vector2 vector)
    {
        if (content.transform.localPosition.y > lastPositionY + 200)
        {
            lastPositionY += 200;
            //var index = (Mathf.FloorToInt(lastPositionY / 200) - 1) % Items.Count;
            var index = ((Items.Count - 1) + (Mathf.CeilToInt(lastPositionY / 200) - 1) % Items.Count) % Items.Count;
            var temp = Instantiate(Items[index], Items[index].transform.parent);
            temp.name = index.ToString();
            content.GetComponent<VerticalLayoutGroup>().padding.top += 200;
            Destroy(Items[index].gameObject);
            Items[index] = temp;
        }
        else if (content.transform.localPosition.y < lastPositionY - 200)
        {
            lastPositionY -= 200;
            var index = ((Items.Count - 1) + (Mathf.CeilToInt(lastPositionY / 200) + 1) % Items.Count) % Items.Count;
            var temp =  Instantiate(Items[index], Items[index].transform.parent);
            temp.name = index.ToString();
            temp.SetAsFirstSibling();
            content.GetComponent<VerticalLayoutGroup>().padding.top -= 200;
            Destroy(Items[index].gameObject);
            Items[index] = temp;
        }

        print(content.transform.localPosition.y);
    }

    void ArrangeList()
    {

    }
}
