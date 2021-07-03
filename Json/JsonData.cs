using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class UserInfo
{
    public string nickname; //primary key
    public int boong;
    public int heart;
    public int candy;
    public int skin_powder;
    public int block_powder;
    public int heart_time;
    public int stage_current;
    public string log_out;
    public int skin_a;
    public int skin_b;
    public int profile_skin;
    public string profile_style;
    public string facebook_token;

  

    public UserInfo()
    {

    }
    
    public UserInfo(string nick , string facebook)
    {
        nickname = nick;
        facebook_token = facebook;

        boong = 0;
        heart = 5;
        candy = 0;
        skin_powder = 0;
        block_powder = 0;
        heart_time = 600;
        stage_current = 0;
        log_out = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        skin_a = 0;
        skin_b = 1;
        profile_skin = 0;
        profile_style = ""; //null

      
    }

    public UserInfo DeepCopy()
    {
        UserInfo copy = new UserInfo(nickname, facebook_token);
        copy.boong = boong;
        copy.heart = heart;
        copy.candy = candy;
        copy.skin_powder = skin_powder;
        copy.block_powder = block_powder;
        copy.heart_time = heart_time;
        copy.stage_current = stage_current;
        copy.log_out = log_out;
        copy.skin_a = skin_a;
        copy.skin_b = skin_b;
        copy.profile_skin = profile_skin;
        copy.profile_style = profile_style; //null

        return copy;
    }
}

[Serializable]
public class UserHistory
{
    public string nickname;
    public int play_time;
    
    public int boong_get;
    public int boong_use;
    public int heart_get;
    public int heart_use;


    public int editor_make;
    public int editor_clear;
    public int editor_fail;

    public int stage_clear;
    public int stage_fail;

    public int drops; //떨어짐
    public int crash; //캐릭터와 부딪힘
    public int carry; //업기
    public int reset; //무르기
    public int move; //움직임
    public int snow; //치운 눈
    public int parfait;// 파르페 완성
    public int crack;// 부순 크래커
    public int cloud;// 솜사탕을 탄 횟수

    public UserHistory()
    {

    }

    public UserHistory(string nick)
    {
        nickname = nick;
        play_time = 0;
        boong_get = 0;
        boong_use = 0;
        heart_get = 0;
        heart_use = 0;

        editor_make = 0;
        editor_clear = 0;
        editor_fail = 0;

        stage_clear = 0;
        stage_fail = 0;

        drops = 0;
        crash = 0;
        carry = 0;
        reset = 0;
        move = 0;
        snow = 0;
        parfait = 0;
        crack = 0;
        cloud = 0;
    }

    public UserHistory DeepCopy()
    {
        UserHistory copy = new UserHistory();
        copy.nickname = nickname;
        copy.play_time = play_time;
        copy.boong_get = boong_get;
        copy.boong_use = boong_use;
        copy.heart_get = heart_get;
        copy.heart_use = heart_use;

        copy.editor_make = editor_make;
        copy.editor_clear = editor_clear;
        copy.editor_fail = editor_fail;

        copy.stage_clear = stage_clear;
        copy.stage_fail = stage_fail;

        copy.drops = drops;
        copy.crash = crash;
        copy.carry = carry;
        copy.reset = reset;
        copy.move = move;
        copy.snow = snow;
        copy.parfait = parfait;
        copy.crack = crack;
        copy.cloud = cloud;

        return copy;
    }
}

[Serializable]
public class UserReward
{
    public string nickname;
    public int reward_num;
    public string time_get;

    public UserReward()
    {

    }

    public UserReward(string nick, int num)
    {
        nickname = nick;
        reward_num = num;
        time_get = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public UserReward DeepCopy()
    {
        UserReward copy = new UserReward();
        copy.nickname = this.nickname;
        copy.reward_num = this.reward_num;
        copy.time_get = this.time_get;
        return copy;
    }
}

[Serializable]
public class UserStage
{
    public string nickname;
    public int stage_num;
    public int stage_star;
    public int stage_move;
    public string stage_clear_time;

    public UserStage()
    {

    }

    public UserStage(string nick , int num , int star, int move)
    {
        nickname = nick;
        stage_num = num;
        stage_star = star;
        stage_move = move;
        stage_clear_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public UserStage DeepCopy()
    {
        UserStage copy = new UserStage(nickname, stage_num, stage_star, stage_move);
        return copy;
    }
}

[Serializable]
public class UserInventory // 
{
    public string nickname;
    public string item_name;
    public string time_get;

    public UserInventory()
    {

    }

    public UserInventory(string nick, string name)
    {
        nickname = nick;
        item_name = name;
        time_get = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public UserInventory DeepCopy()
    {
        UserInventory copy = new UserInventory(nickname, item_name);
        return copy;
    }
}

[Serializable]
public class Mailbox
{
    public string receiver;
    public string sender;
    public string time;
    public string item;
    public int quantity;

    public Mailbox()
    {

    }

    public Mailbox(string receiver_ , string sender_ , string item_ , int quantitiy_)
    {
        receiver = receiver_;
        sender = sender_;
        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        item = item_;
        quantity = quantitiy_;
    }

    public Mailbox DeepCopy()
    {
        Mailbox copy = new Mailbox(receiver, sender, item, quantity);
        return copy;
    }
}

[Serializable]
public class UserFriend
{
    public string nickname_mine;
    public string nickname_friend;
    public int friendship;
    public int state; // 0 1 2
    public string time_request;
    public bool send;

    public UserFriend()
    {

    }

    public UserFriend(string mine , string friend,int state)
    {
        nickname_mine = mine;
        nickname_friend = friend;
        friendship = 0;
        this.state = state;
        send = false;
        time_request = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public UserFriend DeepCopy()
    {
        UserFriend copy = new UserFriend(nickname_mine, nickname_friend, state);
        return copy;
    }
}

[Serializable]
public class EditorMap
{
    public string map_id;//nickname + title // 같은 닉네임으로 같은 타이틀 제작 금지
    public int map_no;//난수 생성 // 중복 금지
    public string maker;
    public string title;
    public string make_time;
    public int play_count;
    public int likes;
    public int height;
    public int width;
    public string datas;//list parsing;
    public string styles;//list parsing;
    public string star_limit;//list parsing
    public int move;
    public int level;

    public EditorMap()
    {

    }

    public EditorMap(Map map,string nick , int moveCount)
    {
        map_id = nick + map.map_title;
        maker = nick;
        title = map.map_title;
        make_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        play_count = 0;
        likes = 0;
        height = map.mapsizeH;
        width = map.mapsizeW;
        datas = Parser.ListListToString(map.datas);
        styles = Parser.ListListToString(map.styles);
        star_limit = Parser.ListToString(map.star_limit);

        move = moveCount;

        level = move / 5 + 1;
        if (level > 5) level = 5;

        List<CustomMapItem> maps = AWSManager.instance.editorMap;
        int ran = UnityEngine.Random.Range(10000000, 99999999);
        bool uniq = false;
        while(!uniq)
        {
            uniq = true;
            for (int i = 0; i < maps.Count; i++)
            {
                if (ran == maps[i].itemdata.map_no)
                {
                    ran = UnityEngine.Random.Range(10000000, 99999999);
                    uniq = false;
                    break;
                }
            }

        }

        map_no = ran;
        

    }

    public EditorMap DeepCopy()
    {
        EditorMap copy = new EditorMap();


        return copy;
    }
}

//gksrmf djEjgrp Tmsms r jfRk wjdansrud qkqh ajdcjddl Ehdro tkfkdgo fkqbfkq love you
//한글 어덯게 스는 ㄱ ㅓㄹ가 정문경 바보 멍청이 둥개 사랑해 라뷰랍 ㅣㅐㅍㄷ ㅛㅐㅕ