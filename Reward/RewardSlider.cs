using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RewardSlider : MonoBehaviour
{
    public Slider rewardSlider;
    public Image[] rewardImages; // 10개이고 4번째 7번째 10번째는 버튼 기능(보상 획득 가능?)

    public int island_num;
    public RewardPopup rewardPopup;

    public List<Reward> island_rewards;

    // Start is called before the first frame update

    public GameObject[] rewardEffect;

    //섬 번호 //해당 섬에서의 별 최대 값 //유저의 보유 별의 개수 // 해당 섬에서 얻을 수 있는 보상(3개)
    public void SetSlider(int island_num, int maxValue , int userValue , List<Reward> rewards)//Initialize Slider
    {
        this.island_num = island_num;

        rewardSlider.maxValue = maxValue;
        rewardSlider.value = userValue;

        island_rewards = rewards;

        int reward_frequency = maxValue / rewardImages.Length; // 전체 별 갯수 / 보상 횟수 = 보상을 받을 수 있는 별의 간격
        Debug.Log("frequency : " + reward_frequency);//보상을 받을 수 있는 별의 간격

        for (int i = 0; i < rewardImages.Length; i++)
        {
            int reward_num = island_num * 3 + i; //보상 번호
            int frequency = reward_frequency * (i + 1);


            if (userValue < frequency)//아직 도달하지 못함
            {
                rewardEffect[i].SetActive(false);
                rewardImages[i].gameObject.GetComponent<Button>().interactable = false;
                //rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + ((i+1)*20) + "_none");
            }
            else//보상을 받을 수 있음
            {
                if(AWSManager.instance.userReward.Exists(x => x.reward_num == reward_num))
                {
                    rewardEffect[i].SetActive(false);
                    //rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + ((i + 1) * 20) + "_done");
                    rewardImages[i].gameObject.GetComponent<Button>().interactable = false;
                }
                else
                {
                    rewardEffect[i].SetActive(true);
                    //rewardImages[i].sprite = Resources.Load<Sprite>("Reward/Number/" + ((i + 1) * 20) + "_reward");
                    rewardImages[i].gameObject.GetComponent<Button>().interactable = true;
                }
                

            }

        }
    }

    
    //Button Clicked
    public void GetReward(int index)// 4 7 10 Text(or Image) 에서 가능 index 값 012
    {
        rewardEffect[index].SetActive(false);
        rewardImages[index].gameObject.GetComponent<Button>().interactable = false;
        //rewardImages[index].sprite = Resources.Load<Sprite>("Reward/Number/" + ((index + 1) * 20) + "_done");

        
        Debug.Log("reward index : " + index + " reward quan : " + island_rewards[index].rewardItems.Count);
        int reward_num = island_num * 3 + index;
        rewardPopup.SetRewardList(island_rewards[index],reward_num);
       
    }
}
