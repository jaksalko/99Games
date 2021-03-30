using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

using Amazon;
using Amazon.CognitoIdentity;


using Facebook.Unity;

using System.Text;
using System.Security.Cryptography;



public class LoadingScene : MonoBehaviour
{
    bool isAuth;
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
    private void Awake()
    {
        
        StartCoroutine(Interpolation());//Animation Effect
        
        
    }
    void XML_Load_Callback(bool isExist)
    {
        Debug.Log("xml exist : " + isExist);
    }
    void Start()
    {

        //PlayerPrefs.SetInt("tutorial",1);
        PlayerPrefs.SetInt("tutorial", IslandData.tutorial + 1);//튜토리얼 건너 뛰기

        awsManager = AWSManager.instance;
        xmlManager = XMLManager.ins;


        
        xmlManager.LoadItems(XML_Load_Callback);
        

        if(PlayerPrefs.GetString("Login", "none") == "none")//계정 생성하지 않았으면
        {
            FB.Init();
            facebook_login_button.gameObject.SetActive(true);
            guest_login_button.gameObject.SetActive(true);
        }
        else//이미 계정이 있는 유저
        {
            xmlManager.Count_LogOut_Time();

            if(xmlManager.itemDB.user.facebook)
            {

                FB.Init(delegate () {

                    if (FB.IsLoggedIn)
                    { //User already logged in from a previous session
                       
                        awsManager.AddLogin_To_Credentials(AccessToken.CurrentAccessToken.TokenString);
                        awsManager.Load_UserInfo(Callback_Load_UserInfo);
                    }
                    else
                    {
                        Debug.Log("critical error");
                        var perms = new List<string>() { "email" };
                        FB.LogInWithReadPermissions(perms, FacebookNotLoggedInCallback);
                        //Critical Error!
                    }
                });

            }
            else
            {
                
                awsManager.Load_UserInfo(Callback_Load_UserInfo);
            }


           
            play_button.interactable = true;
        }



        

    }
    void FacebookNotLoggedInCallback(ILoginResult result)//처음 앱을 사용할 경우 또는 다시 앱을 설치했을 경우
    {
        if (FB.IsLoggedIn)//success get token
        {

            Debug.Log("You get Access Token : " + AccessToken.CurrentAccessToken.UserId);
            awsManager.AddLogin_To_Credentials(AccessToken.CurrentAccessToken.TokenString);
            awsManager.Load_UserInfo(Callback_Load_UserInfo);
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
    

    public void SignUpGuest()
    {
        isAuth = false;
        accountPanel.SetActive(true);
        
    }
    public void SignUpWithFacebook()//SignUp Button Click Event
    {
        isAuth = true;
        accountPanel.SetActive(true);
        

    }
    void FacebookLoginCallback(ILoginResult result)//처음 앱을 사용할 경우 또는 다시 앱을 설치했을 경우
    {
        if (FB.IsLoggedIn)//success get token
        {
            
            Debug.Log("You get Access Token : " + AccessToken.CurrentAccessToken.UserId);
            awsManager.AddLogin_To_Credentials(AccessToken.CurrentAccessToken.TokenString);
            
            awsManager.Create_FacebookToken(AccessToken.CurrentAccessToken.UserId, nickname.text);
        }
        else//error...
        {
            Debug.Log("FB Login error");
        }
    }
    
    public void AddAccount()//Account Panel
    {

        string nickname_regex = "^[a-zA-Z가-힣0-9]{1}[a-zA-Z가-힣0-9]{1,7}$";
        Regex regex = new Regex(nickname_regex);

        if(regex.IsMatch(nickname.text))
        {
            awsManager.Create_UserInfo(isAuth, nickname.text, Callback_Create_UserInfo);      
        }
        else
        {
            addAccountText.text = "한글,영어,숫자 포함 최소 2자, 최대 8자입니다";
        }
    }
    void Callback_Create_UserInfo(bool success)//Create_UserInfo Result..
    {
        if(success)
        {
            if(isAuth)
            {
                PlayerPrefs.SetString("Login", "facebook");
                var perms = new List<string>() { "email" };
                FB.LogInWithReadPermissions(perms, FacebookLoginCallback);
                //create facebookuser 
            }
            else
            {
                PlayerPrefs.SetString("Login", "guest");
                
                //do nothing
            }

            xmlManager.itemDB.Initialize(isAuth, nickname.text);
            xmlManager.SaveItems();

            facebook_login_button.gameObject.SetActive(false);
            guest_login_button.gameObject.SetActive(false);
            accountPanel.SetActive(false);
            play_button.interactable = true;
        }
        else
        {
            //DB Exception // 아마도 중복?
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
}
