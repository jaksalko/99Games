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
    public int skin_powder;
    public int block_powder;

    public RewardItem rewardItemPrefab;
    public List<RewardItem> rewardItems;
    UserInventory userInventory;

    RewardPopup rewardPopup;
    AWSManager aws;

    UserInfo copy_user;
    UserHistory copy_history;
    UserReward userReward;
    public void SetReward(int island_num, int level, int boong , int heart , int block_powder , int skin_powder , string item)
    {
        island = island_num;
        star_index = level;
        this.boong = boong;
        this.heart = heart;
        this.block_powder = block_powder;
        this.skin_powder = skin_powder;


        if(boong != 0)
        {
            RewardItem reward = Instantiate(rewardItemPrefab);
            reward.transform.SetParent(transform,false);
            reward.SetRewardItem("Reward/boong", "Reward/back", boong);
            rewardItems.Add(reward);
        }
        if(heart != 0)
        {
            RewardItem reward = Instantiate(rewardItemPrefab);
            reward.transform.SetParent(transform,false);
            reward.SetRewardItem("Reward/heart", "Reward/back", heart);
            rewardItems.Add(reward);
        }
        if (block_powder != 0)
        {
            RewardItem reward = Instantiate(rewardItemPrefab);
            reward.transform.SetParent(transform,false);
            reward.SetRewardItem("Reward/block_powder", "Reward/back", block_powder);
            rewardItems.Add(reward);
        }
        if (skin_powder != 0)
        {
            RewardItem reward = Instantiate(rewardItemPrefab);
            reward.transform.SetParent(transform,false);
            reward.SetRewardItem("Reward/skin_powder", "Reward/back", skin_powder);
            rewardItems.Add(reward);
        }

        if(item != "none")
        {
            Debug.Log("item");
            userInventory = new UserInventory(PlayerPrefs.GetString("nickname","pingpengboong") , item);
            RewardItem reward = Instantiate(rewardItemPrefab);
            reward.transform.SetParent(transform,false);
            reward.SetRewardItem("Reward/"+ item,"Reward/skinback", 1);//Image path , reward object
            rewardItems.Add(reward);
        }
        else
        {
            Debug.Log("none");
            userInventory = new UserInventory(PlayerPrefs.GetString("nickname", "pingpengboong"), "none");
        }

      
    }

    public void GetReward(RewardPopup popup)
    {
        aws = AWSManager.instance;

        copy_user = aws.userInfo.DeepCopy();
        copy_history = aws.userHistory.DeepCopy();
        userReward = new UserReward(aws.userInfo.nickname, island * 3 + star_index);

        rewardPopup = popup;
        

        copy_user.boong += boong;
        copy_user.heart += heart;
        copy_user.block_powder += block_powder;
        copy_user.skin_powder += skin_powder;

        copy_history.boong_get += boong;
        copy_history.heart_get += heart;


        
        JsonAdapter.RewardRequest rewardRequest = new JsonAdapter.RewardRequest(copy_user, copy_history, userReward, userInventory);
        JsonAdapter.instance.GetReward(rewardRequest, WebCallback);
        
        
    }

    void WebCallback(bool success)
    {
        if (success)
        {
            if (JsonAdapter.instance.EndLoading())
            {
                rewardPopup.GetRewardCallback();
                aws.userInfo = copy_user;
                aws.userHistory = copy_history;
                aws.userReward.Add(userReward);
                if (userInventory.item_name != "none")
                    aws.userInventory.Add(userInventory);
                //JsonAdapter.instance.CreateUserReward(userReward,GetRewardCallback);

            }

        }
        else
        {

            Debug.LogError("fail load user");
        }
    }

    void GetRewardCallback(bool success)
    {
        if (success)
        {
            if (JsonAdapter.instance.EndLoading())
            {
                rewardPopup.GetRewardCallback();
                //get all data
            }

        }
        else
        {
            Debug.LogError("fail load user");
            //back up all data
        }
    }
    //something items...
}

