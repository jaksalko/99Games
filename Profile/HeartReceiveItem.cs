using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class HeartReceiveItem : MonoBehaviour
{
    public Text dateText;
    public Text senderText;
    public Image senderStyleImage;
    //public Text friendshipText;
    Mailbox mailbox;

    JsonAdapter jsonAdapter;
    ProfilePopup profilePopup;
    public void Initialize(Mailbox mailbox_,ProfilePopup popup)
    {
        profilePopup = popup;
        mailbox = mailbox_;
        DateTime now = DateTime.Now;
        DateTime send_time = DateTime.ParseExact(mailbox.time, "yyyy-MM-dd HH:mm:ss", null);

        TimeSpan timeSpan = now - send_time;
        int day = timeSpan.Days;//지난 날
        int hour = timeSpan.Hours;//지난 시간

        int day_remain = 13 - day;
        int hour_remain = 24 - hour;

        Debug.Log(day + "d" + hour + "h");
        dateText.text = day_remain + "d" + hour_remain + "h";
        senderText.text = "<color=white>"+mailbox.sender+ "</color> 님이"+Environment.NewLine+"하트 1개를 선물했어요";

        UserInfo friendInfo = AWSManager.instance.allUserInfo.Find((x) => x.nickname == mailbox.sender);
        //UserFriend friend = AWSManager.instance.userFriend.Find(x => x.nickname_friend == friendInfo.nickname);
        senderStyleImage.sprite = Resources.Load<Sprite>("Icon/Skin/" + friendInfo.profile_skin);

        //friendshipText.text = friend.friendship.ToString();

        jsonAdapter = JsonAdapter.instance;
    }

    public void GetHeart()
    {
        jsonAdapter.UpdateData(mailbox, "getmailbox", RequestCallback);
    }

    void RequestCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                jsonAdapter.GetAllUserData(AWSManager.instance.userInfo.nickname, Refresh);
                
                /*
                jsonAdapter.GetUserFriend();
                jsonAdapter.GetUserInfo();
                jsonAdapter.GetUserHistory();
                jsonAdapter.GetUserMailbox();
                */
            }
        }
        else
        {

        }
    }

    void Refresh(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {

                profilePopup.Refresh();
            }
        }
        else
        {

        }
    }
}
