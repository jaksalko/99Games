using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class Reward : MonoBehaviour
{
    public int island;
    public int star_index;

    public int boong;
    public int heart;

    public Image rewardImage;

    public Text rewardText;

    public void SetReward(int island_num, int index, string reward_name , int quan)
    {
        island = island_num;
        star_index = index;

        if(reward_name == "boong")
        {
            boong = quan;
            rewardImage.sprite = Resources.Load<Sprite>("Reward/boong");
            rewardText.text = boong + " 붕";
        }
        else if(reward_name == "heart")
        {
            heart = quan;
            rewardImage.sprite = Resources.Load<Sprite>("Reward/heart");
            rewardText.text = heart + " ";
        }
        else
        {
            //something items..
        }
    }

    public void GetReward()
    {
        UserInfo user = XMLManager.ins.itemDB.user;
        user.boong += boong;
        user.heart += heart;


    }

    //something items...
}
