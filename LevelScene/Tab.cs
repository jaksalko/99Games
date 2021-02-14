using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Tab : MonoBehaviour
{
    public List<CustomMapItem> customMapItems;
    public Transform content;

    public void GetItem(CustomMapItem item)
    {
        customMapItems.Add(item);
    }
    public void ClearList()
    {
        int listCount = customMapItems.Count;

        for (int i = 0; i < listCount; i++)
        {
            Destroy(customMapItems[i].gameObject);
        }

        customMapItems.Clear();
    }
    abstract public void InitializeTab();

}
