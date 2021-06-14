using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class EditorPlayPopup : UIScript
{
    public ToggleGroup toggleGroup;//orderby
    public Toggle[] toggles;//where

    public RectTransform pageContainer;

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

    int order_num = 0;
    int page_max;
    int page_now;
    [SerializeField]
    float page_width;
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
        pageContainer.anchoredPosition = new Vector2(-page_width * page_now, pageContainer.anchoredPosition.y);
    }

    public void LastPage()
    {
        page_now = page_max;
        pageContainer.anchoredPosition = new Vector2(-page_width * page_now, pageContainer.anchoredPosition.y);

    }
    

    public void LeftPage()
    {
        if (page_now != 0)
            page_now--;

        pageContainer.anchoredPosition = new Vector2(-page_width * page_now, pageContainer.anchoredPosition.y);
    }

    public void RightPage()
    {
        if (page_now < page_max)
            page_now++;

        pageContainer.anchoredPosition = new Vector2(-page_width * page_now, pageContainer.anchoredPosition.y);
    }

    public void OrderbyToggleGroup(int toggle_num)
    {
        order_num = toggle_num;

        switch(order_num)
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

        foreach(CustomMapItem item in editorMaps)
        {
            item.transform.SetAsFirstSibling();
        }

        page_now = 0;
        pageContainer.anchoredPosition = new Vector2(-page_width * page_now, pageContainer.anchoredPosition.y);


    }

    public void Search()
    {
        

        foreach (CustomMapItem item in editorMaps)
        {
            Destroy(item.gameObject);
        }
        editorMaps.Clear();

        

        switch(searchDropdown.value)
        {
            case 0://맵이름
                for(int i = 0; i < awsManager.editorMap.Count; i++)
                {
                    if(awsManager.editorMap[i].itemdata.title.Contains(inputField.text))
                    {
                        CustomMapItem copyItem = Instantiate(awsManager.editorMap[i]);
                        editorMaps.Add(copyItem);
                        copyItem.transform.SetParent(pageContainer,false);
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
                        copyItem.transform.SetParent(pageContainer,false);
                    }
                }
                break;
        }

        FilteringItem();
        OrderbyToggleGroup(order_num);
        
        
    }

    public void AllButtonClicked()
    {
        foreach(Toggle toggle in toggles)
        {
            toggle.isOn = true;
            toggle.GetComponent<Image>().color = Color.white;
        }

        foreach(CustomMapItem item in  editorMaps)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void LevelButtonClicked(Toggle toggle)// 1 2 3 4 5
    {
        if (toggle.isOn)
            toggle.GetComponent<Image>().color = Color.white;
        else
            toggle.GetComponent<Image>().color = Color.gray;


        FilteringItem();
    }

    void FilteringItem()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                List<CustomMapItem> on = editorMaps.Where(x => x.itemdata.level == i + 1).ToList();
                foreach (CustomMapItem item in on)
                {
                    item.gameObject.SetActive(true);
                }
            }
            else
            {
                List<CustomMapItem> off = editorMaps.Where(x => x.itemdata.level == i + 1).ToList();
                foreach (CustomMapItem item in off)
                {
                    item.gameObject.SetActive(false);
                }
            }

        }

        int active_count = 0;
        foreach (CustomMapItem item in editorMaps)
        {
            if (item)
                if (item.gameObject.activeSelf)
                    active_count++;
        }

        page_max = (active_count-1) / 4; // 0 ~ page_max
        if (page_max < 0) page_max = 0;

        page_now = 0;
        Debug.Log("page max : " + page_max);
        pageContainer.anchoredPosition = new Vector2(-page_width * page_now, pageContainer.anchoredPosition.y);

    }
    


   

   
}
