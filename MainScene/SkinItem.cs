using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinItem : MonoBehaviour
{
    public Text skinText;
    public Image skinImage;
    public Text boongPrice;
    public Text powderPrice;

    public int boong;
    public int skin_powder;
    public string info;
    public void Initialize(string name_ , string info_,int boong_,int skin_powder_, string path_)
    {
        skinText.text = name_;
        info = info_;
        boong = boong_;
        skin_powder = skin_powder_;
        skinImage.sprite = Resources.Load<Sprite>(path_);
        boongPrice.text = boong.ToString();
        powderPrice.text = powderPrice.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InfoButtonClicked()
    {

    }

    public void BuyBoongPrice()
    {
        UserInfo userInfo = AWSManager.instance.userInfo;
        UserHistory userHistory = AWSManager.instance.userHistory;
        UserInventory newItem = new UserInventory(userInfo.nickname, skinText.text);
        JsonAdapter.instance.CreateUserInventory(newItem, WebCallback);

        userInfo.boong -= boong;
        userHistory.boong_use += boong;
        JsonAdapter.instance.UpdateData(userInfo,"userInfo", WebCallback);
        JsonAdapter.instance.UpdateData(userHistory, "userHistory", WebCallback);
    }
    public void BuyPowderPrice()
    {
        UserInfo userInfo = AWSManager.instance.userInfo;
        UserHistory userHistory = AWSManager.instance.userHistory;
        UserInventory newItem = new UserInventory(userInfo.nickname, skinText.text);
        JsonAdapter.instance.CreateUserInventory(newItem, WebCallback);

        userInfo.skin_powder -= skin_powder;

        JsonAdapter.instance.UpdateData(userInfo, "userInfo", WebCallback);
    }

    void WebCallback(bool success)
    {
        if (success)
        {
            if (JsonAdapter.instance.EndLoading())
            {
                //구매완료 팝업.
            }

        }
        else
        {
            Debug.LogError("fail load user");
        }
    }
}
