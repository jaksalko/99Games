using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendReceiveItem : FriendItem
{
    public void Accept_Request()
    {
        UserFriend request = new UserFriend(AWSManager.instance.userInfo.nickname, friendInfo.nickname, 2);
        
        jsonAdapter.UpdateData(request, "userFriend",RequestCallback);
        

    }
    public void Reject_Request()
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
