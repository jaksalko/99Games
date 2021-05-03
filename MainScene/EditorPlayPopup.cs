using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class EditorPlayPopup : UIScript
{

    public GameObject page;
    public List<GameObject> pages;

    public Transform pageContainer;

    public InputField inputField;
    public Dropdown searchDropdown;

    public List<CustomMapItem> editorMaps;

    [Header("Editor Text")]
    public Slider candySlider;
    public Text candy_text;
    public Image candyImage;
    public Text boong_text;
    public Image boongImage;

    Sprite candy_off;
    Sprite candy_on;
    int page_now;
    private void Start()
    {
        candy_off = Resources.Load<Sprite>("Editor/candy_off");
        candy_on = Resources.Load<Sprite>("Editor/candy_on");
    }

    private void Update()
    {
        int candy = awsManager.userInfo.candy;

        if (candy < 10)
            candyImage.sprite = candy_off;
        else
            candyImage.sprite = candy_on;




        candySlider.value = candy;
        candy_text.text = candy.ToString();
        //boong_text.text = awsManager.userInfo.candy.ToString();
    }

    public void FirstPage()
    {
        page_now = 0;
        for (int i = 0; i < pages.Count; i++)
        {
            if (page_now == i)
                pages[i].SetActive(true);

            else
                pages[i].SetActive(false);
        }
    }

    public void LastPage()
    {
        page_now = pages.Count - 1;
        for (int i = 0; i < pages.Count; i++)
        {
            if (page_now == i)
                pages[i].SetActive(true);

            else
                pages[i].SetActive(false);
        }

    }
    

    public void LeftPage()
    {
        if (page_now != 0)
            page_now--;

        for(int i = 0; i < pages.Count; i++)
        {
            if(page_now == i)
                pages[i].SetActive(true);

            else
                pages[i].SetActive(false);
        }
    }

    public void RightPage()
    {
        if (page_now < pages.Count-1)
            page_now++;

        for (int i = 0; i < pages.Count; i++)
        {
            if (page_now == i)
                pages[i].SetActive(true);

            else
                pages[i].SetActive(false);
        }
    }

    public void Search()
    {
        
        
        editorMaps.Clear();
        pages.Clear();
        foreach (Transform page in pageContainer)
        {
            Destroy(page.gameObject);
        }

        page_now = 0;

        switch(searchDropdown.value)
        {
            case 0://맵이름
                for(int i = 0; i < awsManager.editorMap.Count; i++)
                {
                    if(awsManager.editorMap[i].itemdata.title.Contains(inputField.text))
                    {
                        CustomMapItem copyItem = Instantiate(awsManager.editorMap[i]);
                        editorMaps.Add(copyItem);
                    }
                }
                break;
            case 1://제작자
                for (int i = 0; i < awsManager.editorMap.Count; i++)
                {
                    if (awsManager.editorMap[i].itemdata.maker.Contains(inputField.text))
                    {
                        CustomMapItem copyItem = Instantiate(awsManager.editorMap[i]);
                        editorMaps.Add(copyItem);
                    }
                }
                break;
        }

        Transform page_transform = page.transform;

        for (int i = 0; i < editorMaps.Count; i++)
        {

            if(i%4 == 0)
            {
                GameObject newPage = Instantiate(page);
                pages.Add(newPage);
                page_transform = newPage.transform;
                page_transform.SetParent(pageContainer,false);

                if (i == 0)
                {
                    newPage.SetActive(true);
                }
                    
            }

            editorMaps[i].transform.SetParent(page_transform, false);
        }
    }

    public void LevelButtonClicked()
    {

    }
    public void SortButtonClicked(int sort_type)
    {
        switch(sort_type)
        {
            case 0:
                editorMaps = editorMaps.OrderBy(x => x.itemdata.make_time).ToList();
                break;
            case 1:
                editorMaps = editorMaps.OrderBy(x => x.itemdata.play_count).ToList();
                break;
            case 2:
                editorMaps = editorMaps.OrderBy(x => x.itemdata.likes).ToList();
                break;
        }
        
        
        

    }


    public void SetPage()
    {

    }

    void MakeCustomeMapItem()
    {
        /*
        Debug.Log("make map");
        List<EditorMap> datas = XMLManager.ins.database

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
        */
       
    }

   
}
