using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

public class MyIglooSkinItem : MonoBehaviour
{

    public bool inUse;
    public Text skinText;
    public Image skinImage;
    public Image skinRank;
    public GameObject inUseImage;
    public Button useButton;
    
    [SerializeField]
    public Skin skin;

    MyIgloo myIgloo;

    public void InitializeItem(Skin skin,MyIgloo igloo)
    {

        myIgloo = igloo;
        this.skin = skin;
        skinText.text = skin.skinName;
        skinImage.sprite = Resources.Load<Sprite>("store/skin/"+skin.path);
        skinRank.sprite = Resources.Load<Sprite>("store/rank/"+skin.skinRank);

        var skinstream = this.ObserveEveryValueChanged(x => x.skin.inPossession)
                .Subscribe(x => UpdateItem(x));

        

    }

    void UpdateItem(bool possession)
    {
        if(possession)
        {
           
            skinImage.color = Color.white;
            skinRank.color = Color.white;
            //skinBox.color = Color.white;
        }
        else
        {
            
            skinImage.color = Color.gray;
            skinRank.color = Color.gray;
            //skinBox.color = Color.gray;
        }
    }

    

    public void ClickItem()
    {
        bool isActive = useButton.gameObject.activeSelf;

        myIgloo.MoveUseButton();

        if(!isActive && !inUse && skin.inPossession)
            useButton.gameObject.SetActive(true);

        
    }

    public void ChangeSkin()//useButton Click
    {
        if(myIgloo.character == 0)//skin_a
            AWSManager.instance.userInfo.skin_a = skin.skin_num;
        else
            AWSManager.instance.userInfo.skin_b = skin.skin_num;

        useButton.gameObject.SetActive(false);
        

    }

    public void SeeInfo()
    {
        myIgloo.infoPopup.ActivatePopup(skin);
    }

    

    void WebCallback(bool success)
    {
        if (success)
        {
            if (JsonAdapter.instance.EndLoading())
            {
                //구매완료 팝업.
                //유저 데이터 가져오기 -- 인벤토리 업데이트를 위해
            }

        }
        else
        {
            Debug.LogError("fail load user");
        }
    }
}
