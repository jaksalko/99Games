using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class EditorPlayPopup : MonoBehaviour
{
    public CustomMapItem item_prefab;

    public Tab[] tabs;

    private void OnEnable()
    {
        MakeCustomeMapItem();
        foreach (Tab tab in tabs) tab.InitializeTab();
    }

    public void ExitButton()
    {
        foreach (Tab tab in tabs) tab.ClearList();
        
        gameObject.SetActive(false);
    }

    public void ChangeTab(int tab_number)
    {
        foreach(Tab tab in tabs)
        {
            if (tab.gameObject.activeSelf)
            {
                tab.gameObject.SetActive(false);
                break;
            } 
        }
        tabs[tab_number].gameObject.SetActive(true);
    }

   

    void MakeCustomeMapItem()
    {
        Debug.Log("make map");
        List<JsonData> datas = GameManager.instance.customMapdatas;

        for (int i = 0; i < datas.Count ; i++)
        {
            CustomMapItem newItem = Instantiate(item_prefab);
            newItem.Initialize(datas[i]);
            tabs[0].GetItem(newItem);

            CustomMapItem levelItem = Instantiate(item_prefab);
            levelItem.Initialize(datas[i]);
            int level = datas[i].difficulty;
            tabs[level].GetItem(levelItem);
           
        }

       
    }

   
}
