using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;


class InfoHistory
{
    public UserInfo userInfo;
    public UserHistory userHistory;

    

    public InfoHistory(UserInfo i, UserHistory h)
    {
        userInfo = i;
        userHistory = h;
    }
}
/*
[DynamoDBTable("PingPengBoong")]
public class PingPengBoong
{
    [DynamoDBHashKey] public int version { get; set; } // 0
    [DynamoDBProperty] public List<string> user { get; set; } //user list
    [DynamoDBProperty] public List<string> editorMap { get; set; } // editor map list
}

[DynamoDBTable("Friend")]
public class Friend
{
    [DynamoDBHashKey] public string nickname { get; set; }
    [DynamoDBProperty] public Dictionary<string, int> friends { get; set; }
    [DynamoDBProperty] public List<string> send { get; set; }
    [DynamoDBProperty] public List<string> receive { get; set; }
}



[DynamoDBTable("UserInfo")]
public class User
{
    [DynamoDBHashKey]public string nickname { get; set; } //hash key
    [DynamoDBProperty]public int boong { get; set; } // 유저의 붕 갯수
    [DynamoDBProperty]public int heart {get; set;} // 유저의 하트 갯수
    [DynamoDBProperty]public int heart_time {get; set;} // 하트 충전 타이머
    [DynamoDBProperty]public int stage_current {get; set;} // 유저가 깨야하는 스테이지
    [DynamoDBProperty]public string log_out {get; set;} //로그 아웃 시간 yyyy/MM/dd HH:mm
    [DynamoDBProperty]public string star_list { get; set; } // 별 갯수 = List
    [DynamoDBProperty]public string move_list { get; set; } // 움직인 횟수 = List
    [DynamoDBProperty]public List<int> reward_list { get; set; } // Set


    [DynamoDBProperty] public int ping_skin_num { get; set; }//캐릭터 1 스킨착용 기본 검은색
    [DynamoDBProperty] public int peng_skin_num { get; set; }//캐릭터 2 스킨착용 기본 분홍 

    [DynamoDBProperty] public int profile_skin_num { get; set; }//대표이미지 번호
    [DynamoDBProperty] public string profile_introduction { get; set; }//자기소개
    [DynamoDBProperty] public int profile_style_num { get; set; }//칭호 번호 //0은 없음.

    [DynamoDBProperty] public List<int> mySkinList { get; set; } // 보유 스킨 번호 리스트 --> 보유 대표이미지 번호 리스트
    [DynamoDBProperty] public List<int> myStyleList { get; set; } // 보유 칭호 번호 리스트

    [DynamoDBProperty] public int drop_count { get; set; }
    [DynamoDBProperty] public int crash_count { get; set; }
    [DynamoDBProperty] public int carry_count { get; set; }
    [DynamoDBProperty] public int reset_count { get; set; }
    [DynamoDBProperty] public int move_count { get; set; }
    [DynamoDBProperty] public int snow_count { get; set; }
    [DynamoDBProperty] public int parfait_done_count { get; set; }
    [DynamoDBProperty] public int crack_count { get; set; }
    [DynamoDBProperty] public int cloud_count { get; set; }

    [DynamoDBProperty] public int editor_make_count { get; set; }
    [DynamoDBProperty] public int editor_clear_count { get; set; }

    [DynamoDBProperty] public int boong_count { get; set; }
    [DynamoDBProperty] public int heart_count { get; set; }

    [DynamoDBProperty] public int skin_count { get; set; }

    [DynamoDBProperty] public long playTime { get; set; }
    [DynamoDBProperty] public int clear_count { get; set; }
    [DynamoDBProperty] public int fail_count { get; set; }


    [DynamoDBProperty] public bool facebook { get; set; }
}

[DynamoDBTable("FacebookUser")]
public class FacebookUser
{
    [DynamoDBHashKey] public string tokenId { get; set; } //facebook token id
    [DynamoDBProperty] public string nickname { get; set; } //facebook user nickname
}
*/
public class AWSManager : MonoBehaviour
{
    bool paused = false;
    public static AWSManager instance = null;
    JsonAdapter jsonAdapter;

    private CognitoAWSCredentials _credentials;
    private CognitoAWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(PrivateData.identitiy_pool_id, RegionEndpoint.APNortheast2);
            return _credentials;
        }
    }//return CognitoAWSCredentials

    
    AmazonDynamoDBClient dbClient;
    DynamoDBContext dbContext;

    public delegate void LoadUserCallback(bool isLoad);
    public delegate void CreateUserCallback(bool success);

    public delegate void BooleanCallback(bool callback);


    public List<UserInfo> allUserInfo;

    public UserInfo userInfo;
    public UserHistory userHistory;

    public List<UserStage> userEditorStage;
    public List<UserReward> userReward;
    public List<UserStage> userStage;
    public List<UserInventory> userInventory;
    public List<UserFriend> userFriend;
    public List<CustomMapItem> editorMap;
    public List<Mailbox> mailbox;


    public Transform customMapList;
   

    void Awake()
    {
        
        if (instance == null)
        {
            Debug.Log("Single instance is null");
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Single instance is not Single.. Destroy gameobject!");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);//Dont destroy this singleton gameobject :(

        UnityInitializer.AttachToGameObject(this.gameObject); // Amazon Initialize
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest; //Bug fix code

        Credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result) {
            if (result.Exception != null)
            {
                Debug.Log(result.Exception);//Exception!!
            }


            else
            {
                Debug.Log("GetIdentityID" + result.Response);
            }
        });

        dbClient = new AmazonDynamoDBClient(Credentials, RegionEndpoint.APNortheast2);
        dbContext = new DynamoDBContext(dbClient);
        
       
        //credentials.ClearIdentityCache();
        //credentials.ClearCredentials();        
    }

    private void Start()
    {
        jsonAdapter = JsonAdapter.instance;
    }

    public void AddLogin_To_Credentials(string token)
    {
        Credentials.AddLogin ("graph.facebook.com", token);
    }

    public void Count_LogOut_Time()
    {
        Debug.Log(userInfo.log_out);
        DateTime log_out = DateTime.ParseExact(userInfo.log_out, "yyyy-MM-dd HH:mm:ss", null);
        long sec = (long)(DateTime.Now - log_out).TotalSeconds;
        Debug.Log("sec : " + sec);


        if (userInfo.heart < 5)
        {
            for (int i = 0; i < sec; i++)
            {

                userInfo.heart_time--;
                if (userInfo.heart_time <= 0)
                {
                    userInfo.heart++;
                    userInfo.heart_time = 600;

                    if (userInfo.heart == 5)
                    {
                        break;
                    }
                    jsonAdapter.UpdateData(new InfoHistory(userInfo,userHistory), "infoHistory", SaveDataCallback);
                    
                }
            }
        }

        
    }

    public IEnumerator StartTimer()//하트 타이머
    {
        float wait_second = 1f;
        int play_sec = 0;
        while (true)
        {
            play_sec++;
            if (play_sec == 60)
            {
                play_sec = 0;
                userHistory.play_time++;
            }

            if (userInfo.heart < 5)
            {
                userInfo.heart_time -= 1;
                if (userInfo.heart_time == 0)
                {
                    userInfo.heart++;
                    userInfo.heart_time = 600;
                    jsonAdapter.UpdateData(new InfoHistory(userInfo, userHistory), "infoHistory", SaveDataCallback);
                    
                    //XMLManager.ins.SaveItems();

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

    void OnApplicationQuit()
    {
        SaveData();
        PlayerPrefs.Save();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            paused = true;
            SaveData();
            PlayerPrefs.Save();
        }
        else
        {
            if (paused)//앱 시작 시 불리는 것을 방지하기 위함
            {
                Count_LogOut_Time();
                paused = false;
            }
        }
    }

    void SaveData()
    {
        InfoHistory infoHistory = new InfoHistory(userInfo, userHistory);
        userInfo.log_out = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        jsonAdapter.UpdateData(infoHistory, "infoHistory",SaveDataCallback);
        
    }

    void SaveDataCallback(bool success)
    {
        if(success)
        {
            if(jsonAdapter.EndLoading())
            {
                Debug.Log("success update");
            }
            else
            {
                Debug.LogError("loading...");
            }
        }
        else
        {
            Debug.LogError("fail save data");
        }
    }
    /*
    public void CreateEditorMap(Map map , int moveCount,int dif , BooleanCallback callback)
    {
        List<int> dynamoDB_datas = new List<int>();
        for (int i = 0; i < map.datas.Count; i++)
        {
            for(int j = 0; j < map.datas[i].Count; j++)
            {
                dynamoDB_datas.Add(map.datas[i][j]);
            }

           
        }

        List<int> dynamoDB_styles = new List<int>();
        for (int i = 0; i < map.styles.Count; i++)
        {
            for (int j = 0; j < map.styles[i].Count; j++)
            {
                dynamoDB_styles.Add(map.styles[i][j]);
            }


        }

        EditorMap editorMap = new EditorMap
        {
            

        

            map_id = GameManager.instance.user_aws.nickname + " " + DateTime.Now.ToString("yyyyMMddHHmmss"),
            //map_id = map.map_title,
            maker = GameManager.instance.user_aws.nickname,
            title = map.map_title,
            make_time = DateTime.Now.ToString("yyyyMMddHHmmss"),
            play_count = 0,
            like = 0,

            height = map.mapsizeH,
            width = map.mapsizeW,

            
            datas = Parser.ListToString(dynamoDB_datas),
            styles = Parser.ListToString(dynamoDB_styles),

            isParfait = map.parfait,

            step = moveCount,
            level = dif,
            star_limit = new List<int>() { moveCount, moveCount * 2, moveCount * 3 }

             

        };

        dbContext.SaveAsync(editorMap, (result) => {
            if (result.Exception == null)
            {
                //this = user;

                Debug.Log("Success saved editormap");
                callback(true);

            }
            else
            {
                Debug.LogWarning("EditorMap Save Exception : " + result.Exception);
                callback(false);
            }


        });



    }
    */






}
