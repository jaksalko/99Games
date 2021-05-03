using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockPiece : MonoBehaviour
{
    public string info;
    public string blockName;
    public int block_powder;

    public void Initialize(string name_ , string info_ , int block_powder_,string path_)
    {
        blockName = name_;
        info = info_;
        block_powder = block_powder_;
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(path_);
    }

    public void BuyBlockPowder()
    {
        UserInventory newItem = new UserInventory(AWSManager.instance.userInfo.nickname, blockName);
        JsonAdapter.instance.CreateUserInventory(newItem, WebCallback);

        AWSManager.instance.userInfo.block_powder -= block_powder;
        JsonAdapter.instance.UpdateData(AWSManager.instance.userInfo, "userInfo", WebCallback);
        
    }

    void WebCallback(bool success)
    {
        if (success)
        {
            if (JsonAdapter.instance.EndLoading())
            {
                //??
            }

        }
        else
        {
            Debug.LogError("fail load user");
        }
    }
}
