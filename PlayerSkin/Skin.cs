using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Skin
{
    public DateTime skin_get_time;
    public string skinName;
    public string skinInfo;
    public string path;
    public char skinRank;

    public int boong_buy;
    public int powder_buy;
    public int powder_payback;
    public int skin_num;

    public bool inPossession;
   
    public Skin(string _name, string _info ,string _path, int boong , int powder , int payback,int num)
    {
        skinName = _name;
        skinInfo = _info;
        path = _path;
        skinRank = path[0];

        boong_buy = boong;
        powder_buy = powder;
        powder_payback = payback;
        skin_num = num;
        inPossession = false;

        //skin_get_time = DateTime.ParseExact("0000-00-00 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
    }
    
}
