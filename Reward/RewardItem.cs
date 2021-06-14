using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour
{
    public Image rewardImage;
    public Sprite front;
    public Sprite back;

    public Text rewardText;

    bool backActive = false;

    public void SetRewardItem(string reward_path,string back_path, int quan)
    {
        front = Resources.Load<Sprite>(reward_path);
        back = Resources.Load<Sprite>(back_path);
        rewardImage.sprite = back;
        rewardText.text = "X"+quan;

        
    }

    public void FlipImage()
    {

        StartCoroutine(Flip());
    }

    public void ChangeDirection()
    {
        if(backActive)
        {
            rewardImage.sprite = back;
            backActive = false;
        }
        else
        {
            rewardImage.sprite = front;
            backActive = true;
        }
    }
    IEnumerator Flip()
    {
        float t = 0;
        float y = 3;


        for(int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Rotate(new Vector3(0, y, 0));
            t+=y;

            if(t == 90 || t == -90)
            {
                ChangeDirection();
            }
        }

        rewardText.gameObject.SetActive(true);
        yield break;

    }

}