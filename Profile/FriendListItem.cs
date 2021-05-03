using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FriendListItem : FriendItem
{
    public Button sendHeartButton;
    public Text favorabilityText;

   
    public override void Initialize(UserInfo friendinfo, UserFriend friend_, ProfilePopup profilePopup)
    {
        base.Initialize(friendinfo, friend_, profilePopup);
        if(friend_.send)
        {
            sendHeartButton.interactable = false;
        }
        else
        {
            sendHeartButton.interactable = true;
        }

        favorabilityText.text = friend.friendship.ToString();
    }

    public void Send_Heart()
    {
        Mailbox mailbox = new Mailbox(
            receiver_: friendInfo.nickname,
            sender_: AWSManager.instance.userInfo.nickname,
            item_: "heart",
            quantitiy_: 1);

        UserFriend userFriend = friend.DeepCopy();
        userFriend.friendship++;
        userFriend.send = true;
        userFriend.time_request = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        JsonAdapter.HeartRequest heartRequest = new JsonAdapter.HeartRequest(mailbox, userFriend);
        jsonAdapter.UpdateData(heartRequest, "sendmailbox", HeartRequestCallback);
    }
    public void Delete_Friend()
    {
        UserFriend userFriend = new UserFriend(AWSManager.instance.userInfo.nickname, friendInfo.nickname, 2);
        jsonAdapter.DeleteFriend(userFriend, RequestCallback);
    }

    void HeartRequestCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                friend.send = true;
                friend.time_request = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                jsonAdapter.GetAllUserData(AWSManager.instance.userInfo.nickname, Refresh);
                popup.Refresh();
            }
        }
        else
        {

        }
    }

    void RequestCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                jsonAdapter.GetAllUserData(AWSManager.instance.userInfo.nickname, Refresh);
                
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
                
                popup.Refresh();
            }
        }
        else
        {
            
        }
    }

}
