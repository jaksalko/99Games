using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : MonoBehaviour
{
    public Transform rewardParent;
    public Reward reward;

    public RectTransform okButtonPosition;

    Vector3 button_default_position;
    int reward_num;

    public void SetRewardList(Reward rewardList, int reward_num)
    {
        reward = rewardList;
        this.reward_num = reward_num;
        reward.transform.SetParent(rewardParent,false);


        button_default_position = okButtonPosition.anchoredPosition;
        Vector3 buttonPosition = button_default_position + Vector3.down * 90 + (Vector3.down * 50 * reward.rewardItems.Count);
        okButtonPosition.anchoredPosition = buttonPosition;

        
            
        
        //ok button position setting
        Debug.Log(okButtonPosition.position);
        Debug.Log(okButtonPosition.localPosition);

        gameObject.SetActive(true);
    }

    public void GetRewards()
    {

        reward.GetReward(this);

        
    }

    public void GetRewardCallback()
    {
        reward.transform.SetParent(CSVManager.instance.transform);
        okButtonPosition.anchoredPosition = button_default_position;
        gameObject.SetActive(false);
    }
}
