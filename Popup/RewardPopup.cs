using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : MonoBehaviour
{
    public Transform rewardParent;
    public Reward reward;

    int reward_num;

    public void SetRewardList(Reward rewardList, int reward_num)
    {
        reward = rewardList;
        this.reward_num = reward_num;
        reward.transform.SetParent(rewardParent,false);
        reward.GetComponent<RectTransform>().localPosition = default;

        /*
        button_default_position = okButtonPosition.anchoredPosition;
        Vector3 buttonPosition = button_default_position + Vector3.down * 90 + (Vector3.down * 50 * reward.rewardItems.Count);
        okButtonPosition.anchoredPosition = buttonPosition;

        */
        
            
        
        

        gameObject.SetActive(true);

        foreach(RewardItem item in reward.rewardItems)
        {
            item.FlipImage();
        }
        
    }

    public void GetRewards()
    {

        reward.GetReward(this);

        
    }

    public void GetRewardCallback()
    {
        reward.transform.SetParent(CSVManager.instance.transform);
        gameObject.SetActive(false);
    }
}
