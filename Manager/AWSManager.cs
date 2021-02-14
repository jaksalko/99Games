using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;



[DynamoDBTable("UserInfo")]
public class User
{
    [DynamoDBHashKey]public string nickname { get; set; } //hash key
    [DynamoDBProperty]public int boong { get; set; } // 유저의 붕 갯수
    [DynamoDBProperty]public int heart {get; set;} // 유저의 하트 갯수
    [DynamoDBProperty]public int heart_time {get; set;} // 하트 충전 타이머
    [DynamoDBProperty]public int current_stage {get; set;} // 유저가 깨야하는 스테이지
    [DynamoDBProperty]public string log_out {get; set;} //로그 아웃 시간 yyyy/MM/dd HH:mm
    [DynamoDBProperty]public List<int> star_list { get; set; } // 별 갯수
    [DynamoDBProperty]public List<int> move_list { get; set; }
    [DynamoDBProperty]public List<int> reward_list { get; set; }


    [DynamoDBProperty] public int ping_skin_num { get; set; }//캐릭터 1 스킨착용 기본 검은색
    [DynamoDBProperty] public int peng_skin_num { get; set; }//캐릭터 2 스킨착용 기본 분홍 

    [DynamoDBProperty] public int profile_skin_num { get; set; }//대표이미지 번호
    [DynamoDBProperty] public string profile_introduction { get; set; }//자기소개
    [DynamoDBProperty] public int profile_style_num { get; set; }//칭호 번호 //0은 없음.

    [DynamoDBProperty] public List<int> mySkinList { get; set; } // 보유 스킨 번호 리스트 --> 보유 대표이미지 번호 리스트
    [DynamoDBProperty] public List<int> myStyleList { get; set; } // 보유 칭호 번호 리스트

    [DynamoDBProperty] public int drop_count { get; set; }
    [DynamoDBProperty] public int carry_count { get; set; }
    [DynamoDBProperty] public int reset_count { get; set; }
    [DynamoDBProperty] public int move_count { get; set; }
    [DynamoDBProperty] public int snow_count { get; set; }
    [DynamoDBProperty] public int parfait_done_count { get; set; }
    [DynamoDBProperty] public int crack_count { get; set; }

    [DynamoDBProperty] public int editor_make_count { get; set; }
    [DynamoDBProperty] public int editor_clear_count { get; set; }

    [DynamoDBProperty] public int boong_count { get; set; }
    [DynamoDBProperty] public int heart_count { get; set; }

    [DynamoDBProperty] public int skin_count { get; set; }

    [DynamoDBProperty] public long playTime { get; set; }
    [DynamoDBProperty] public bool facebook { get; set; }
}

[DynamoDBTable("FacebookUser")]
public class FacebookUser
{
    [DynamoDBHashKey] public string tokenId { get; set; } //facebook token id
    [DynamoDBProperty] public string nickname { get; set; } //facebook user nickname
}

public class AWSManager : MonoBehaviour
{
    public static AWSManager instance = null;


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
    
    string id;
    public User user;
    bool isPaused = false;

    void Awake()
    {
        Debug.Log("Single Class Awake..." + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));//Set instance
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
        
    }

    public void AddLogin_To_Credentials(string token)
    {
        Credentials.AddLogin ("graph.facebook.com", token);
    }

   
    public void Load_UserInfo(LoadUserCallback callback) //DB에서 캐릭터 정보 받기
    {
        Debug.Log(XMLManager.ins.itemDB.user.nickname);
        dbContext.LoadAsync<User>(XMLManager.ins.itemDB.user.nickname, (AmazonDynamoDBResult<User> result) =>
        {
            // id가 abcd인 캐릭터 정보를 DB에서 받아옴
            if (result.Exception != null)
            {
                Debug.LogException(result.Exception);
                callback(false);
            }
            if(result.Result == null)
            {
                Debug.Log("not exist!");
                callback(false);
            }
            else
            {
                user = result.Result;
                
                Debug.Log("user data :" + user.nickname); //찾은 캐릭터 정보 중 아이템 정보 출력
                callback(true);
            }
            
        });
       

    }
    public void Create_FacebookToken(string id , string nick)
    {
        FacebookUser facebookUser = new FacebookUser
        {
            tokenId = id,
            nickname = nick
        };

        dbContext.SaveAsync(facebookUser, (result) => {
            if (result.Exception == null)
            {
                //this = user;
               
                Debug.Log("Success saved user");
                
            }
            else
            {
                Debug.Log("DB Save Exception : " + result.Exception);
                
            }


        });
    }
    public void Create_UserInfo(bool isAuth, string nick,CreateUserCallback create_success)//call by LoadingScene(AddAccount)
    {


        User user = new User
        {

            nickname = nick,
            boong = 0,
            heart = 5,
            current_stage = 0,
            log_out = DateTime.Now.ToString("yyyyMMddHHmmss"),
            heart_time = 600, // 10분
            star_list = new List<int> { 0 },
            move_list = new List<int> { 0 },
            reward_list = new List<int>(),

            ping_skin_num = 0,//캐릭터 1 스킨착용 기본 검은색
            peng_skin_num = 1,//캐릭터 2 스킨착용 기본 분홍 

            profile_skin_num = 0,//대표이미지 번호
            profile_introduction ="",//자기소개
            profile_style_num =0,//칭호 번호 //0은 없음.

            mySkinList = new List<int> {0,1},// 보유 스킨 번호 리스트 --> 보유 대표이미지 번호 리스트
            myStyleList = new List<int> { 0},// 보유 칭호 번호 리스트 --> 0은 없D

            drop_count =0,
            carry_count =0,
            reset_count = 0,
            move_count = 0,
            snow_count = 0,
            parfait_done_count = 0,
            crack_count = 0,

            editor_make_count = 0,
            editor_clear_count = 0,

            boong_count = 0,
            heart_count = 0,

            skin_count = 0,

            playTime = 0,
            facebook = isAuth

        };

        dbContext.LoadAsync(nick, (AmazonDynamoDBResult<User> result) =>
        {
            // id가 abcd인 캐릭터 정보를 DB에서 받아옴
            if (result.Exception != null)
            {
                Debug.LogException(result.Exception);
                create_success(false);
            }
            if (result.Result == null)
            {
                Debug.Log("new user!");
                dbContext.SaveAsync(user, (save_result) => {


                    if (save_result.Exception == null)
                    {
                        this.user = user;
                        
                        Debug.Log("Success saved user");
                        create_success(true);
                    }
                    else
                    {
                        Debug.Log("DB Save Exception : " + save_result.Exception);
                        create_success(false);
                    }


                });
            }
            else
            {
               
                Debug.Log("user data :" + user.nickname + ", already exist :("); //찾은 캐릭터 정보 중 아이템 정보 출력
                create_success(false);
            }

        });
       
    }
    

    public void Update_UserInfo()
    {
        user.log_out = DateTime.Now.ToString("yyyyMMddddHHmmss");
        dbContext.SaveAsync(user,(res)=>
            {
                if(res.Exception == null)
                {
                    Debug.Log("success update");
                    
                }
                else
                {
                    Debug.Log(res.Exception);
                }
                     
        });
    }
    

}
