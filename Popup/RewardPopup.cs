using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : MonoBehaviour
{
    public Transform rewardParent;
    List<Reward> rewards = new List<Reward>();

    public Transform okButtonPosition;

    Vector3 button_default_position;
    int reward_num;

    public void SetRewardList(List<Reward> rewardList, int reward_num)
    {
        rewards = rewardList;
        this.reward_num = reward_num;

        button_default_position = okButtonPosition.position;
        Vector3 buttonPosition = transform.position + Vector3.down * 90 + (Vector3.down * 50 * rewards.Count);
        okButtonPosition.position = buttonPosition;

        for(int i = 0; i < rewards.Count; i++)
        {
            rewards[i].transform.SetParent(rewardParent);
        }
        //ok button position setting
        Debug.Log(okButtonPosition.position);
        Debug.Log(okButtonPosition.localPosition);

        gameObject.SetActive(true);
    }

    public void GetRewards()
    {
        for(int i = 0; i < rewards.Count; i++)
        {
            rewards[i].GetReward();
            rewards[i].transform.SetParent(CSVManager.instance.transform);
        }
        okButtonPosition.position = button_default_position;
        XMLManager.ins.itemDB.AddRewardList(reward_num);
        
        gameObject.SetActive(false);
    }
}
