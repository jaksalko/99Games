using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlider : MonoBehaviour
{
    public Slider rewardSlider;
    public Image[] rewardImages; // 10개이고 4번째 7번째 10번째는 버튼 기능(보상 획득 가능?)

    public int island_num;
    public RewardPopup rewardPopup;

    public List<Reward> island_rewards;

    // Start is called before the first frame update
    

    

    public void SetSlider(int island_num, int maxValue , int userValue , List<Reward> rewards)//Initialize Slider
    {
        this.island_num = island_num;

        rewardSlider.maxValue = maxValue;
        rewardSlider.value = userValue;

        island_rewards = rewards;

        int reward_frequency = maxValue / rewardImages.Length; // 전체 별 갯수 / 보상 횟수
        Debug.Log("frequency : " + reward_frequency);

        for(int i = 0; i < rewardImages.Length; i++)
        {
            int reward_num = island_num * 3 + i;

            int frequency = reward_frequency * (i + 1);
            if (userValue < frequency)
            {
                //rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + frequency + "_none");
                rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + ((i+1)*20) + "_none");
            }
            else
            {
                List<int> reward_done = XMLManager.ins.itemDB.user.reward_list;
                bool done = false;
                for(int j = 0; j < reward_done.Count; j++)
                {
                    if(reward_num == reward_done[j])
                    {
                        //rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + frequency + "_done");
                        rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + ((i + 1) * 20) + "_done");
                        rewardImages[i].gameObject.GetComponent<Button>().interactable = false;
                        done = true;
                        break;
                    }
                }
                if(!done)
                {
                    //rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + frequency + "_reward");
                    rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + ((i + 1) * 20) + "_reward");
                }


            }

        }
    }

    

    public void GetReward(int index)// 4 7 10 Text(or Image) 에서 가능
    {
        rewardImages[index].gameObject.GetComponent<Button>().interactable = false;
        rewardImages[index].sprite = Resources.Load<Sprite>("Reward/Number/" + ((index + 1) * 20) + "_done");

        List<Reward> rewardPopupList = new List<Reward>();
        for(int i = 0; i < island_rewards.Count; i++)
        {
            if(island_rewards[i].star_index == index)
            {
                rewardPopupList.Add(island_rewards[i]);
            }
        }
        Debug.Log("reward index : " + index + " reward quan : " + rewardPopupList.Count);
        int reward_num = island_num * 3 + index;
        rewardPopup.SetRewardList(rewardPopupList,reward_num);
       
    }
}
