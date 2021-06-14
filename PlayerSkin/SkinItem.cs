using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SkinItem : MonoBehaviour
{
    class SkinBuy
    {
        public UserInfo userInfo;
        public UserHistory userHistory;
        public UserInventory item;

        public SkinBuy(UserInfo i , UserHistory h , UserInventory inventory)
        {
            userInfo = i;
            userHistory = h;
            item = inventory;
        }
    }

    public GameObject soldout;
    public Text skinText;
    public Image skinImage;
    public Image skinRank;
    public Text boongPrice;
    public Text powderPrice;

    public Button boongBuyButton;
    public Button powderBuyButton;

    public Skin skin;
    public void Initialize(Skin data)
    {
        skin = data;
        skinText.text = data.skinName;
        
        skinImage.sprite = Resources.Load<Sprite>("store/skin/"+data.path);
        skinRank.sprite = Resources.Load<Sprite>("store/rank/" + data.skinRank);
        boongPrice.text = skin.boong_buy.ToString();
        powderPrice.text = skin.powder_buy.ToString();

        var skinstream = this.ObserveEveryValueChanged(x => x.skin.inPossession)
                .Subscribe(x => UpdateItem(x));
    }
    // Start is called before the first frame update
    

    public void InfoButtonClicked()
    {

    }

    public void BuyBoongPrice()
    {
        UserInfo userInfo = AWSManager.instance.userInfo.DeepCopy();
        UserHistory userHistory = AWSManager.instance.userHistory.DeepCopy();

        if(userInfo.boong >= skin.boong_buy)
        {
           

            UserInventory newItem = new UserInventory(userInfo.nickname, skinText.text);
            userInfo.boong -= skin.boong_buy;
            userHistory.boong_use += skin.boong_buy;
            SkinBuy skinBuy = new SkinBuy(userInfo, userHistory, newItem);

            JsonAdapter.instance.UpdateData(skinBuy, "skinBuy", BuyCallback);
            
        }
        else
        {
            Debug.Log("cant buy");
        }
        
    }
    public void BuyPowderPrice()
    {
        UserInfo userInfo = AWSManager.instance.userInfo.DeepCopy();
        UserHistory userHistory = AWSManager.instance.userHistory.DeepCopy();

        if (userInfo.skin_powder >= skin.powder_buy)
        {
            UserInventory newItem = new UserInventory(userInfo.nickname, skinText.text);
            userInfo.skin_powder -= skin.powder_buy;
            SkinBuy skinBuy = new SkinBuy(userInfo, userHistory, newItem);

            JsonAdapter.instance.UpdateData(skinBuy, "skinBuy", BuyCallback);
        }
        else
        {
            Debug.Log("cant buy");
        }

        
    }

    void BuyCallback(bool success)
    {
        if (success)
        {
            if (JsonAdapter.instance.EndLoading())
            {

                //유저 데이터 가져오기 -- 인벤토리 업데이트를 위해
                JsonAdapter.instance.GetAllUserData(AWSManager.instance.userInfo.nickname,UpdateCallback);
            }

        }
        else
        {
            Debug.LogError("fail buy");
        }
    }

    void UpdateCallback(bool success)
    {
        if (success)
        {
            if (JsonAdapter.instance.EndLoading())
            {
                //구매 종료
                Debug.Log("success buy skin");
            }

        }
        else
        {
            Debug.LogError("fail get userinfo");
        }
    }

    void UpdateItem(bool possession)
    {
        soldout.SetActive(possession);
        boongBuyButton.interactable = !possession;
        powderBuyButton.interactable = !possession;
        if (possession)
        {
            skinImage.color = Color.gray;
            skinRank.color = Color.gray;
           
            //skinBox.color = Color.white;
        }
        else
        {
            skinImage.color = Color.white;
            skinRank.color = Color.white;
           
            //skinBox.color = Color.gray;
        }
    }
}
