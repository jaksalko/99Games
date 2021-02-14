using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchTab : Tab
{
    public InputField searchText;
    public Transform itemList;

    public override void InitializeTab()
    {
        foreach(CustomMapItem item in customMapItems)
        {
            item.transform.SetParent(itemList);
        }
    }

    public void Search()
    {
        int listCount = content.childCount;

        for (int i = 0; i < listCount; i++)
        {
            content.GetChild(0).transform.SetParent(itemList);
        }

        for (int i = 0; i < customMapItems.Count; i++)
        {
            if (customMapItems[i].title.text.Contains(searchText.text))
            {
                customMapItems[i].transform.SetParent(content);
            }
        }
    }

     
}
