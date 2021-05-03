using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Style
{
    public string style_text;
    public string style_info;
    public string type;
    public string condition;
    public int standard;

    public int reward_boong;
    public int reward_heart;
    public string reward_item;

    AWSManager awsManager;
    public Style(string styleText, string styleInfo,string type_, string con_, int standard_ , int boong , int heart, string item)
    {
        style_text = styleText;
        style_info = styleInfo;
        type = type_;
        condition = con_;
        standard = standard_;

        reward_boong = boong;
        reward_heart = heart;
        reward_item = item;

        awsManager = AWSManager.instance;
        
    }

    public void CheckGetStyleCondition()
    {
        if(type == "userInfo")
        {
            var result = awsManager.userInfo.GetType().GetField(condition).GetValue(awsManager.userInfo);
            Debug.Log("condition : " + condition + " value : " + result);
            

        }
        else if (type == "userHistory")
        {
            var result = awsManager.userHistory.GetType().GetField(condition).GetValue(awsManager.userHistory);
            Debug.Log("condition : " + condition + " value : " + result);
        }
        else if (type == "userInventory")
        {
            var result = awsManager.userInventory.GetType().GetField(condition).GetValue(awsManager.userInventory);
            Debug.Log("condition : " + condition + " value : " + result);
        }
        
    }

    public IEnumerator CheckStyle()
    {
        while(true)
        {
            CheckGetStyleCondition();
            yield return new WaitForSeconds(1f);
        }

    }
}
