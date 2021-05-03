using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendSendItem : FriendItem
{

    public void Cancel_Request()
    {
        UserFriend userFriend = new UserFriend(AWSManager.instance.userInfo.nickname, friendInfo.nickname, 2);
        jsonAdapter.DeleteFriend(userFriend, RequestCallback);
        
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
