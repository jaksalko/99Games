using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour
{
    public Image rewardImage;
    public Text rewardText;

    public void SetRewardItem(string path, string text)
    {
        rewardImage.sprite = Resources.Load<Sprite>(path);
        rewardText.text = text;

        
    }

}