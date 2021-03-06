﻿using System.Collections;
using System.Collections.Generic;// let us use lists
using UnityEngine;
using System.Xml;               // basic xml attributes
using System.Xml.Serialization; // access xmlserializer
using System.IO;                //file management
using System.Text;
using UnityEngine.UI;
using System;
public class XMLManager : MonoBehaviour
{
    bool paused = false;
    public delegate void IsExist(bool isExist);

    public static XMLManager ins = null;//terrible singleton pattern
                                        // Use this for initialization
    private void Awake()
    {
        Debug.Log("XMLManager awake");

        if (ins == null)
        {
            Debug.Log("instance is null");
            ins = this;
        }
        else if (ins != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    //list of items 
    public Database database;

    //save function
    public void SaveItems()
    {

        XmlSerializer serializer = new XmlSerializer(typeof(Database));

        /*
        if(database.userInfo != null)
            database.userInfo.log_out = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        */
#if UNITY_EDITOR
        using (FileStream stream = new FileStream(
            Application.dataPath + "/XML/item_data.xml", FileMode.Create))
        {
            StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
            serializer.Serialize(sw, database); // put into sml files)
            sw.Close();//important :)
        }


#elif UNITY_IOS || UNITY_ANDROID
          
        string path = Application.persistentDataPath;
        
        if(!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

            using (FileStream stream = new FileStream(
            path + "/item_data.xml", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
                serializer.Serialize(sw, database); // put into sml files)
                sw.Close();//important :)
            }
#endif


    }
   
    //load function
    public void LoadItems()
    {
        string path;
        XmlSerializer serializer = new XmlSerializer(typeof(Database));

#if UNITY_EDITOR
        try
        {
            FileStream stream = new FileStream(
            Application.dataPath + "/XML/item_data.xml", FileMode.Open);
            database = serializer.Deserialize(stream) as Database;
            stream.Close();
            

        }
        catch(System.Exception e)//doesn't exist file
        {
            Debug.LogWarning("LoadItems Exception Error : " + e.ToString());
            SaveItems();//create file
            LoadItems();

        }
#elif UNITY_IOS || UNITY_ANDROID
        try
        {
        path = Application.persistentDataPath;
        //path += "datas";
        FileStream stream = new FileStream(
            path + "/item_data.xml", FileMode.Open);
            database = serializer.Deserialize(stream) as Database;
            stream.Close();
        }
        catch(System.Exception e)
        {

            Debug.LogWarning("LoadItems Exception Error : " + e.ToString());
            SaveItems();//create file
            LoadItems();

        }
#endif

    }


    void OnApplicationQuit()
    {
        SaveItems();
        PlayerPrefs.Save();
    }
    
    void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            paused = true;
            SaveItems();
            PlayerPrefs.Save();
        }
        else
        {
            if(paused)//앱 시작 시 불리는 것을 방지하기 위함
            {
                Count_LogOut_Time();
                paused = false;
            }
        }
    }

    public void Count_LogOut_Time()
    {
        /*
        Debug.Log(database.userInfo.log_out);
        DateTime log_out = DateTime.ParseExact(database.userInfo.log_out, "yyyy-MM-dd HH:mm:ss", null);
        long sec = (long)(DateTime.Now - log_out).TotalSeconds;
        Debug.Log("sec : " + sec);


        if (database.userInfo.heart < 5)
        {
            for (int i = 0; i < sec; i++)
            {

                database.userInfo.heart_time--;
                if (database.userInfo.heart_time <= 0)
                {
                    database.userInfo.heart++;
                    database.userInfo.heart_time = 600;

                    if(database.userInfo.heart == 5)
                    {
                        break;
                    }
                }
            }
        }

        SaveItems();
        PlayerPrefs.Save();
        */
    }
        
}
/*
[Serializable]
public class UserInfo
{
    public string nickname;
    public int boong = 0; // 유저의 붕 갯수
    public int heart = 5; // 유저의 하트 갯수
    public int heart_time = 600; // 하트 충전 타이머
    public int stage_current = 0; // 유저가 깨야하는 스테이지
    public string log_out; //로그 아웃 시간 yyyy/MM/dd HH:mm
    public List<int> star_list; // 유저의 닉네임 (UNIQUE)
    public List<int> move_list; // 유저의 닉네임 (UNIQUE)
    public List<int> reward_list; // 받은 보상 번호

    public int ping_skin_num = 0;//캐릭터 1 스킨착용 기본 검은색
    public int peng_skin_num = 1;//캐릭터 2 스킨착용 기본 분홍 

    public int profile_skin_num = 0;//대표이미지 번호
    public string profile_introduction = "";//자기소개
    public int profile_style_num = 0;//칭호 번호 //0은 없음.

    public List<int> mySkinList; // 보유 스킨 번호 리스트 --> 보유 대표이미지 번호 리스트
    public List<int> myStyleList; // 보유 칭호 번호 리스트

    public int drop_count = 0;
    public int crash_count = 0;
    public int carry_count = 0;
    public int reset_count = 0;
    public int move_count = 0;
    public int snow_count = 0;
    public int parfait_done_count = 0;
    public int crack_count = 0;
    public int cloud_count = 0;

    public int editor_make_count = 0;
    public int editor_clear_count = 0;

    public int boong_count = 0;
    public int heart_count = 0;
    
    public int skin_count = 0;

    public long playTime = 0;
    public int clear_count = 0;
    public int fail_count = 0;

    public bool facebook;
    

}
*/
[Serializable]
public class Database
{
    /*
    //List<Dictionary<string, object>> data;
    [XmlElement("UserInfo")]
    public UserInfo userInfo = new UserInfo();

    [XmlElement("UserHistory")]
    public UserHistory userHistory = new UserHistory();

    [XmlArray("UserReward")]
    public List<UserReward> userReward = new List<UserReward>();

    [XmlArray("UserStage")]
    public List<UserStage> userStage = new List<UserStage>();

    [XmlArray("UserInventory")]
    public List<UserInventory> userInventory = new List<UserInventory>();

    [XmlArray("UserFriend")]
    public List<UserFriend> userFriend = new List<UserFriend>();

    //EditorMap

    public void InitializeInfo(string facebook, string nickname)//NewGame
    {
        userInfo = new UserInfo(nickname, facebook);
        userHistory = new UserHistory(nickname);
        
        Debug.Log("initialize");
        PlayerPrefs.SetString("nickname",nickname);
        XMLManager.ins.SaveItems();
    }

    public void InitializeHistory(string nickname)
    {
        userHistory.nickname = nickname;
    }
    
    public IEnumerator StartTimer()
    {
        float wait_second = 1f;
        int play_sec = 0;
        while(true)
        {
            play_sec++;
            if(play_sec == 60)
            {
                play_sec = 0;
                userHistory.play_time++;
            }

            if(userInfo.heart < 5)
            {
                userInfo.heart_time -= 1;
                if(userInfo.heart_time == 0)
                {
                    userInfo.heart++;
                    userInfo.heart_time = 600;
                    XMLManager.ins.SaveItems();
                    
                }
            }
            else
            {
                userInfo.heart_time = 600;
            }
            //Debug.Log("heart time " + user.heart_time);
            

            yield return new WaitForSeconds(wait_second);
        }
    }
      
    */

}