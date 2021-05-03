using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System;

using Amazon;
using Amazon.CognitoIdentity;


using Facebook.Unity;

using System.Text;
using System.Security.Cryptography;



public class LoadingScene : MonoBehaviour
{
    bool isAuth;
    string facebookUserId;
    public Text id;
    public Image fade;
    public Image title;
    public float minSize;
    public float maxSize;

    public InputField nickname;
    public Text addAccountText;
    public GameObject accountPanel;
    public Button make_account_button;
    public Button play_button;

    public Button facebook_login_button;
    public Button guest_login_button;

    AWSManager awsManager = AWSManager.instance;
    XMLManager xmlManager = XMLManager.ins;
    JsonAdapter jsonAdapter = JsonAdapter.instance;

    private void Awake()
    {
        
        StartCoroutine(Interpolation());//Animation Effect

       

    }
   
    void Start()
    {

        //PlayerPrefs.SetInt("tutorial",1);
        //PlayerPrefs.SetInt("tutorial", IslandData.tutorial + 1);//튜토리얼 건너 뛰기

        awsManager = AWSManager.instance;
        xmlManager = XMLManager.ins;
        jsonAdapter = JsonAdapter.instance;

        xmlManager.LoadItems();//if xml file not exist create it.
        jsonAdapter.GetAllUserInfo(WebRequestCallback);
        jsonAdapter.GetAllEditorMap(WebRequestCallback);

        

        
        



        

    }

    void LoginWithUserState()
    {
        string playerPrefs_login = PlayerPrefs.GetString("Login", "none");


        //스마일게이트 제출용
        //string playerPrefs_login = PlayerPrefs.GetString("Login", "guest");
        //PlayerPrefs.SetString("nickname", "스마게");
        //PlayerPrefs.SetInt("tutorial", 7);
        //


        if (playerPrefs_login == "none")//계정 생성하지 않았으면
        {

            //facebook_login_button.gameObject.SetActive(true);
            guest_login_button.gameObject.SetActive(true);
        }
        else if (playerPrefs_login == "facebook")
        {
            Debug.Log("facebook login");
            xmlManager.Count_LogOut_Time();

            FB.Init(delegate () {

                if (FB.IsLoggedIn)
                { //User already logged in from a previous session

                    awsManager.AddLogin_To_Credentials(AccessToken.CurrentAccessToken.TokenString);

                    //editormap 빼고 전부 다
                    //LoadUserData();
                    jsonAdapter.GetAllUserData(PlayerPrefs.GetString("nickname", "pingpengboong"), GetWebRequestCallback);
                }
                else
                {
                    Debug.LogError("critical error");
                    //var perms = new List<string>() { "email" };
                    //FB.LogInWithReadPermissions(perms, FacebookNotLoggedInCallback);
                    //Critical Error!
                }
            });
        }
        else if (playerPrefs_login == "guest")
        {
            Debug.Log("Hi " + PlayerPrefs.GetString("nickname", "pingpengboong") + "Guest Login");
            //LoadUserData();
            jsonAdapter.GetAllUserData(PlayerPrefs.GetString("nickname", "pingpengboong"), GetWebRequestCallback);


        }
        else
        {
            Debug.LogWarning("login error");
        }
    }
    
    /*
    void FacebookNotLoggedInCallback(ILoginResult result)//처음 앱을 사용할 경우 또는 다시 앱을 설치했을 경우
    {
        if (FB.IsLoggedIn)//success get token
        {

            Debug.Log("You get Access Token : " + AccessToken.CurrentAccessToken.UserId);
            awsManager.AddLogin_To_Credentials(AccessToken.CurrentAccessToken.TokenString);
            jsonAdapter.GetUserInfo(xmlManager.database.userInfo.nickname);
        }
        else//error...
        {
            Debug.Log("FB Login error");
        }
    }
    void Callback_Load_UserInfo(bool isExist)
    {
        //xml은 언제나 최신 업데이트라는 가정.
        //xml -> dynamodb

    }
    */

    #region 로그인 버튼
    public void SignUpGuest()
    {
        isAuth = false;
        accountPanel.SetActive(true);
        
    }
    public void SignUpWithFacebook()//SignUp Button Click Event
    {
        isAuth = true;

        FB.Init(delegate () {

            if (FB.IsLoggedIn)
            {
                facebookUserId = AccessToken.CurrentAccessToken.UserId;
                awsManager.AddLogin_To_Credentials(AccessToken.CurrentAccessToken.TokenString);

                if(awsManager.allUserInfo.Exists(x => x.facebook_token == facebookUserId))
                {
                    PlayerPrefs.SetString("nickname", "pingpengboong");
                    jsonAdapter.GetAllUserData(PlayerPrefs.GetString("nickname", "pingpengboong"), GetWebRequestCallback);
                }
                else
                {
                    accountPanel.SetActive(true);
                }
                
            }
            else
            {
                var perms = new List<string>() { "email" };
                FB.LogInWithReadPermissions(perms, FacebookLoginCallback);
            }
        });
        //페이스북 토큰 요청


        //accountPanel.SetActive(true);


    }
    void FacebookLoginCallback(ILoginResult result)//처음 앱을 사용할 경우 또는 다시 앱을 설치했을 경우
    {
        if (FB.IsLoggedIn)//success get token
        {
            facebookUserId = AccessToken.CurrentAccessToken.UserId;
            Debug.Log("You get Access Token : " + facebookUserId);
            awsManager.AddLogin_To_Credentials(AccessToken.CurrentAccessToken.TokenString);

            if (awsManager.allUserInfo.Exists(x => x.facebook_token == facebookUserId))
            {
                PlayerPrefs.SetString("Login", "facebook");
                PlayerPrefs.SetString("nickname", "pingpengboong");
                jsonAdapter.GetAllUserData(PlayerPrefs.GetString("nickname", "pingpengboong"), GetWebRequestCallback);
            }
            else
            {
                accountPanel.SetActive(true);
            }
            
            //awsManager.Create_FacebookToken(AccessToken.CurrentAccessToken.UserId, nickname.text);
        }
        else//error...
        {
            Debug.Log("FB Login error");
        }
    }
    #endregion

    public void AddAccount()//Account Panel
    {

        string nickname_regex = "^[a-zA-Z가-힣0-9]{1}[a-zA-Z가-힣0-9\\s]{0,6}[a-zA-Z가-힣0-9]{1}$";
        Regex regex = new Regex(nickname_regex);

        for (int i = 0; i < awsManager.allUserInfo.Count; i++)
        {
            if (nickname.text == awsManager.allUserInfo[i].nickname)
            {
                addAccountText.text = "이미 존재하는 닉네임입니다";
                return;
            }
        }

        if (regex.IsMatch(nickname.text))
        {
            
            //jsonAdapter.CreateUserData(nickname.text, facebookUserId, CreateWebRequestCallback);
            //jsonAdapter.CreateUserHistory(nickname.text, CreateWebRequestCallback);
            jsonAdapter.AddAccount(nickname.text, facebookUserId, CreateWebRequestCallback);
            //xmlManager.database.InitializeInfo(facebookUserId, nickname.text);
            
        }
        else
        {
            addAccountText.text = "한글,영어,숫자 포함 최소 2자, 최대 8자입니다";
        }
    }

   
    
    

    public void GameStart()
    {
        StartCoroutine(Fader());
    }
    
    IEnumerator Interpolation()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime *1.2f;
            float interpolation = Mathf.Abs(Mathf.Sin(t));

            float size = Mathf.Lerp(minSize, maxSize, interpolation);
            title.transform.localScale = new Vector3(size, size, 1);
            yield return null;
        }

    }
    IEnumerator Fader()
    {
        
        float t = 0;
        Color c = fade.color;

        while(t < 1)
        {
            t += Time.deltaTime * 0.7f;
            c.a = t;
            fade.color = c;
            yield return null;
        }
        if(PlayerPrefs.GetInt("tutorial",0) == 0)
        {
            SceneManager.LoadScene("IntroScene");
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
        

        yield break;
        
    }

    /*
    void LoadUserData()
    {
        string nickname = PlayerPrefs.GetString("nickname", "pingpengboong");

        jsonAdapter.GetUserInfo(nickname, GetWebRequestCallback);
        jsonAdapter.GetAllUserInfo(GetWebRequestCallback);
        jsonAdapter.GetUserFriend(nickname, GetWebRequestCallback);
        jsonAdapter.GetUserHistory(nickname, GetWebRequestCallback);
        jsonAdapter.GetUserInventory(nickname, GetWebRequestCallback);
        jsonAdapter.GetUserReward(nickname, GetWebRequestCallback);
        jsonAdapter.GetUserStage(nickname, GetWebRequestCallback);

        
    }
    */

    void WebRequestCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                Debug.Log("success get all user data");
                LoginWithUserState();//call back GetWebRequestCallback
            }

        }
        else
        {
            Debug.LogError("fail create user");
        }
    }

    void CreateWebRequestCallback(bool success)
    {
        if(success)
        {
            if(jsonAdapter.EndLoading())
            {
                PlayerPrefs.SetString("nickname", nickname.text);

                if (isAuth)
                    PlayerPrefs.SetString("Login", "facebook");
                else
                    PlayerPrefs.SetString("Login", "guest");


                jsonAdapter.GetAllUserData(PlayerPrefs.GetString("nickname", "pingpengboong"), GetWebRequestCallback);
                //LoadUserData();
            }

        }
        else
        {
            Debug.LogError("fail create user");
        }
    }

    void GetWebRequestCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                Debug.Log("get all user data");

                accountPanel.SetActive(false);
                facebook_login_button.gameObject.SetActive(false);
                guest_login_button.gameObject.SetActive(false);

                awsManager.Count_LogOut_Time();
                awsManager.StartCoroutine(awsManager.StartTimer());
                play_button.interactable = true;


            }

        }
        else
        {
            Debug.LogError("fail load user");
        }
    }
}
