using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Linq;

public class JsonAdapter : MonoBehaviour
{
    class UserAccountCreate
    {
        public UserInfo userInfo;
        public UserHistory userHistory;
        public UserInventory item1;
        public UserInventory item2;
        public UserAccountCreate(UserInfo i , UserHistory h, UserInventory item1, UserInventory item2)
        {
            userInfo = i;
            userHistory = h;
            this.item1 = item1;
            this.item2 = item2;
        }
    }

    public class FriendRequest
    {
        public UserFriend myRequest;
        public UserFriend friendRequest;

        public FriendRequest(UserFriend my , UserFriend friend)
        {
            myRequest = my;
            friendRequest = friend;
        }
    }

    public class RewardRequest
    {
        public UserInfo info;
        public UserHistory history;
        public UserReward reward;
        public UserInventory inventory;

        public RewardRequest(UserInfo info , UserHistory history , UserReward reward , UserInventory inven)
        {
            this.info = info;
            this.history = history;
            this.reward = reward;
            inventory = inven;

        }
    }

    public class HeartRequest
    {
        public Mailbox mailbox;
        public UserFriend myFriend;

        public HeartRequest(Mailbox mailbox_ , UserFriend friend)
        {
            mailbox = mailbox_;
            myFriend = friend;
        }


    }

   

    public static JsonAdapter instance = null;

    public GameObject loadingCanvas;

    public delegate void BooleanCallback(bool callback);
    public int wait_ = 0;

    public CustomMapItem customMapPrefab;

    private void Awake()
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

    }

    private void Start()
    {
        
    }

    IEnumerator API_GET(string url , Action<bool,string> callback)
    {
        wait_++;
        loadingCanvas.SetActive(true);
        UnityWebRequest www = UnityWebRequest.Get(PrivateData.ec2+url);
//        Debug.Log(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            callback(false,www.error);
        }
        else
        {
           

            if(www.responseCode != 200)
            {
                Debug.Log("response code : " + www.responseCode);
                callback(false,www.downloadHandler.text);
               
            }
            else
            {
                Debug.Log("GET WebRequset : " + www.downloadHandler.text);
                callback(true,www.downloadHandler.text);
               
            }
           
            

        }
        
        yield break;
    }
    
    IEnumerator API_POST(string url , string bodyJsonString , Action<bool,string> callback)
    {
        wait_++;
        loadingCanvas.SetActive(true);

        Debug.Log(bodyJsonString);
        var req = new UnityWebRequest(PrivateData.ec2 + url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJsonString);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if(req.isHttpError || req.isNetworkError )
        {
            Debug.Log(req.error);
            callback(false,req.error);
        }
        else if(req.responseCode != 200)
        {
            Debug.Log("response code : " + req.responseCode);
            callback(false, req.downloadHandler.text);
            
        }
        else
        {
            Debug.Log("GET WebRequset : " + req.downloadHandler.text);
            callback(true,req.downloadHandler.text);
        }
       
        Debug.Log("Status Code: " + req.responseCode);
        
        yield break;
    }
    public void GetReward(RewardRequest request,BooleanCallback callback)
    {
        var json = JsonUtility.ToJson(request);

        StartCoroutine(API_POST("userReward/insert", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success insert get reward");

                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }

    
    

    public void GetAllUserData(string nickname,BooleanCallback callback)
    {
        CSVManager csvManager = CSVManager.instance;
        AWSManager awsManager = AWSManager.instance;

        StartCoroutine(API_GET("getall/get?nickname="+nickname, (connect, response) =>
        {
            if (connect)
            {
                
                List<string> fixDatas = JsonHelper.FixMultipleJson(response);


                string fixData = JsonHelper.fixJson(fixDatas[0]);
                UserInfo[] userInfos = JsonHelper.FromJson<UserInfo>(fixData);
                awsManager.userInfo = userInfos[0];

                fixData = JsonHelper.fixJson(fixDatas[1]);
                UserHistory[] userHistory = JsonHelper.FromJson<UserHistory>(fixData);
                awsManager.userHistory = userHistory[0];

                fixData = JsonHelper.fixJson(fixDatas[2]);
                UserStage[] userStages = JsonHelper.FromJson<UserStage>(fixData);
                List<UserStage> islandStage = new List<UserStage>();
                List<UserStage> editorStage = new List<UserStage>();
                for(int i = 0; i < userStages.Length; i++)
                {
                    if(userStages[i].stage_num < 10000000)
                    {
                        islandStage.Add(userStages[i]);
                    }
                    else
                    {
                        editorStage.Add(userStages[i]);
                        CustomMapItem item = AWSManager.instance.editorMap.Find(x => x.itemdata.map_no == userStages[i].stage_num);
                        if(item != null)
                            item.SetMining(true);
                    }
                }

                awsManager.userStage = islandStage.OrderBy(x => x.stage_num).ToList();
                awsManager.userEditorStage = editorStage.OrderBy(x => x.stage_clear_time).ToList();

                fixData = JsonHelper.fixJson(fixDatas[3]);
                UserInventory[] userInventories = JsonHelper.FromJson<UserInventory>(fixData);
                for(int i = 0; i < userInventories.Length; i++)
                {
                    Skin skin = csvManager.skins.Find(x => x.skinName == userInventories[i].item_name);
                    skin.inPossession = true;
                    skin.skin_get_time = DateTime.ParseExact(userInventories[i].time_get, "yyyy-MM-dd HH:mm:ss", null);

                
                }
                awsManager.userInventory = userInventories.ToList();

                fixData = JsonHelper.fixJson(fixDatas[4]);
                UserFriend[] userFriends = JsonHelper.FromJson<UserFriend>(fixData);

                for(int i = 0; i < userFriends.Length; i++)
                {
                    DateTime request_time = DateTime.ParseExact(userFriends[i].time_request, "yyyy-MM-dd HH:mm:ss", null);

                    if(DateTime.Now.DayOfYear != request_time.DayOfYear)//하트 초기화
                    {
                        userFriends[i].send = false;
                    }
                    
                }

                awsManager.userFriend = userFriends.ToList();

                fixData = JsonHelper.fixJson(fixDatas[5]);
                UserReward[] userRewards = JsonHelper.FromJson<UserReward>(fixData);
                awsManager.userReward = userRewards.ToList();

                fixData = JsonHelper.fixJson(fixDatas[6]);
                Mailbox[] mailbox = JsonHelper.FromJson<Mailbox>(fixData);
                awsManager.mailbox = mailbox.ToList();




                callback(true);
            }
            else
            {
                Debug.LogError(response);

                callback(false);
                //재시도
            }
        }));
    }

    public void AddAccount(string nickname,string facebook, BooleanCallback callback)
    {

        UserInfo userInfo = new UserInfo(nickname, facebook);
        UserHistory userHistory = new UserHistory(nickname);
        UserInventory skin0 = new UserInventory(nickname,CSVManager.instance.skins[0].skinName);
        UserInventory skin1 = new UserInventory(nickname, CSVManager.instance.skins[1].skinName);
        UserAccountCreate userAccount = new UserAccountCreate(userInfo,userHistory,skin0,skin1);


        var json = JsonUtility.ToJson(userAccount);

        StartCoroutine(API_POST("newUser/create", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create account");

                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }


    public void GetAllUserInfo(BooleanCallback callback)
    {
        
        StartCoroutine(API_GET("allUser/get", (connect, response) =>
        {
            if (connect)
            {
                string fixData = JsonHelper.fixJson(response);
                UserInfo[] userInfos = JsonHelper.FromJson<UserInfo>(fixData);
                AWSManager.instance.allUserInfo = userInfos.ToList();

                callback(true);
            }
            else
            {
                Debug.LogError(response);

                callback(false);
                //재시도
            }
        }));

    }
    public void GetAllEditorMap(BooleanCallback callback)
    {
        StartCoroutine(API_GET("editorMap/get", (connect, response) =>
        {
            if (connect)
            {
                Transform mapList = AWSManager.instance.customMapList;

                string fixData = JsonHelper.fixJson(response);
                EditorMap[] editorMaps = JsonHelper.FromJson<EditorMap>(fixData);
                for(int i = 0; i < editorMaps.Length; i++)
                {
                    CustomMapItem newItem = Instantiate(customMapPrefab);
                    newItem.Initialize(editorMaps[i]);

                    AWSManager.instance.editorMap.Add(newItem);
                    newItem.transform.SetParent(mapList);
                }
                

                callback(true);
            }
            else
            {
                callback(false);
                Debug.LogError(response);
                //재시도
            }
        }));

    }

    /*
    public void GetUserInfo(string nickname, BooleanCallback callback)
    {
        StartCoroutine(API_GET("userInfo/get?nickname=" + nickname, (connect,response) =>
        {
            if(connect)
            {
                string fixData = JsonHelper.fixJson(response);
                UserInfo[] myInfo = JsonHelper.FromJson<UserInfo>(fixData);
                AWSManager.instance.userInfo = myInfo[0];

                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));


    }
    public void GetUserHistory(string nickname, BooleanCallback callback)
    {
        StartCoroutine(API_GET("userHistory/get?nickname=" + nickname, (connect, response) =>
        {
            if (connect)
            {
                string fixData = JsonHelper.fixJson(response);
                UserHistory[] myHistory = JsonHelper.FromJson<UserHistory>(fixData);
                AWSManager.instance.userHistory = myHistory[0];

                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));

    }
    public void GetUserReward(string nickname, BooleanCallback callback)
    {
        StartCoroutine(API_GET("userReward/get?nickname=" + nickname, (connect, response) =>
        {
            if (connect)
            {
                string fixData = JsonHelper.fixJson(response);
                UserReward[] userRewards = JsonHelper.FromJson<UserReward>(fixData);
                AWSManager.instance.userReward.AddRange(userRewards);

                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));

    }
    public void GetUserStage(string nickname, BooleanCallback callback)
    {
        StartCoroutine(API_GET("userStage/get?nickname=" + nickname, (connect, response) =>
        {
            if (connect)
            {
                string fixData = JsonHelper.fixJson(response);
                UserStage[] userStages = JsonHelper.FromJson<UserStage>(fixData);
                AWSManager.instance.userStage.AddRange(userStages);

                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));

    }
    public void GetUserInventory(string nickname, BooleanCallback callback)
    {
        StartCoroutine(API_GET("userInventory/get?nickname=" + nickname, (connect, response) =>
        {
            if (connect)
            {
                string fixData = JsonHelper.fixJson(response);
                UserInventory[] userInventories = JsonHelper.FromJson<UserInventory>(fixData);
                AWSManager.instance.userInventory.AddRange(userInventories);

                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));

    }
    public void GetUserFriend(string nickname, BooleanCallback callback)
    {
        StartCoroutine(API_GET("userFriend/get?nickname_mine=" + nickname, (connect, response) =>
        {
            if (connect)
            {
                string fixData = JsonHelper.fixJson(response);
                UserFriend[] userFriends = JsonHelper.FromJson<UserFriend>(fixData);
                AWSManager.instance.userFriend.AddRange(userFriends);

                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));

    }
    */

    /*
    public void CreateUserData(string nickname , string facebook, BooleanCallback callback)
    {
        UserInfo newUser = new UserInfo(nickname, facebook);
        var json = JsonUtility.ToJson(newUser);
        StartCoroutine(API_POST("userInfo/insert", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create user");
                
                
                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
        
    }
    public void CreateUserHistory(string nickname, BooleanCallback callback)
    {
        UserHistory userHistory = new UserHistory(nickname);
        var json = JsonUtility.ToJson(userHistory);
        StartCoroutine(API_POST("userHistory/insert", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create history");
                
                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }
    */
    public void CreateUserReward(UserReward reward, BooleanCallback callback)
    {
        
        var json = JsonUtility.ToJson(reward);
        StartCoroutine(API_POST("userReward/insert", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create reward");
                

                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }
    public void CreateUserStage(UserStage stage, BooleanCallback callback)
    {
        
        var json = JsonUtility.ToJson(stage);
        StartCoroutine(API_POST("userStage/insert", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create stage");
                

                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }
    public void CreateUserInventory(UserInventory item, BooleanCallback callback)
    {
        
        var json = JsonUtility.ToJson(item);
        StartCoroutine(API_POST("userInventory/insert", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create item");
                

                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }
    public void CreateUserFriend(UserFriend my,UserFriend friend, BooleanCallback callback)
    {
        FriendRequest friendRequest = new FriendRequest(my, friend);
        var json = JsonUtility.ToJson(friendRequest);
        StartCoroutine(API_POST("userFriend/insert", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create friend");
                
                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }
    public void CreateEditorMap(EditorMap editorMap, BooleanCallback callback)
    {
        var json = JsonUtility.ToJson(editorMap);
        StartCoroutine(API_POST("editorMap/insert", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create editorMap");
                //xml.database.userFriend.Add(editorMap);

                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }
   

    public void UpdateData<T>(T data, string url, BooleanCallback callback)
    {
        var json = JsonUtility.ToJson(data);
        StartCoroutine(API_POST(url+"/update", json, (connect, response) => {
            if (connect)
            {
                Debug.Log("success update user");
                Debug.Log(response);
                callback(true);
                //성공알림
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }

    public void DeleteFriend(UserFriend data, BooleanCallback callback)
    {
        var json = JsonUtility.ToJson(data);
        StartCoroutine(API_POST("userFriend/delete", json, (connect, response) => {
            if (connect)
            {
                //성공알림
                Debug.Log("success create editorMap");
                //xml.database.userFriend.Add(editorMap);

                Debug.Log(response);
                callback(true);
            }
            else
            {
                Debug.LogError(response);
                callback(false);
                //재시도
            }
        }));
    }


    public bool EndLoading()
    {
        wait_--;
        if(wait_ == 0)
        {
            loadingCanvas.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
            
    }

}





public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        Debug.Log(wrapper.Items.Length);

        for(int i = 0; i < wrapper.Items.Length; i++)
        {
            Debug.Log(wrapper.Items[i].ToString());
        }
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
    public static string fixJson(string value)//value : []
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static List<string> FixMultipleJson(string value)
    {
        value = value.Remove(0,1);
        value = value.Remove(value.Length-1,1);

        List<string> jsons = new List<string>();

        for(int i = 0; i < value.Length; i++)
        {
            string json = "";

            if(value[i] == '[')
            {
                while(true)
                {
                    json += value[i];
                    //Debug.Log(json);
                    if (value[i] == ']')
                    {
                        jsons.Add(json);
                        break;
                    }
                    i++;
                    
                    
                }
                
            }
            
        }

        Debug.Log(value);
        return jsons;

    }
   
    



}