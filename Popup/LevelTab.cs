using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class LevelTab : Tab
{
    public Dropdown sortDropdown;//popularity , updatetime , clear


    public override void InitializeTab()
    {
        foreach (CustomMapItem item in customMapItems)
        {
            item.transform.SetParent(content);
        }
    }

    public void Sort()
    {
        /*
        int sort = sortDropdown.value;

        foreach (CustomMapItem item in customMapItems)
        {
            item.transform.SetParent(null);
        }

        switch (sort)// 0 : 최신순 1: 인기순 2: 클리어순
        {
            case 0:
                Debug.Log("time sort");
                customMapItems.Sort((x,y) =>
                DateTime.Compare(DateTime.Parse(y.itemdata.updateTime), DateTime.Parse(x.itemdata.updateTime)));
                break;
            case 1:
                break;
            case 2:
                break;
        }

        foreach(CustomMapItem item in customMapItems)
        {
            item.transform.SetParent(content);
        }
        */
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
