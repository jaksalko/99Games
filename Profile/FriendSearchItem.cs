using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendSearchItem : FriendItem
{
    public void Send_Request()
    {
        UserFriend userFriend = new UserFriend(AWSManager.instance.userInfo.nickname,friendInfo.nickname,0);
        UserFriend friend = new UserFriend(friendInfo.nickname, AWSManager.instance.userInfo.nickname, 1);

        jsonAdapter.CreateUserFriend(userFriend,friend, RequestCallback);
       
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
