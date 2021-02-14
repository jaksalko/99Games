using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class GameManager : MonoBehaviour
{
    public Text debugTxt;

    public static GameManager instance = null;
    public string id; // id == user.id

    public int infiniteLevel;
    public Vector2 maxSize;

    public int nowLevel;

    
    
    public UserInfo userInfo; // XML DATA
    public User user_aws; // AWS USER DATA 
    public List<StageData> stages;
    public List<CustomStagePlayerData> customStagePlayerDatas;

    public GameObject canvas;

    public List<JsonData> customMapdatas = new List<JsonData>();
    public CustomMapItem playCustomData;

    AWSManager awsManager = AWSManager.instance;
    XMLManager xmlManager = XMLManager.ins;

    private void Awake()
    {
        
        Application.targetFrameRate = 60;
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
    void Start()
    {

        awsManager = AWSManager.instance;
        xmlManager = XMLManager.ins;

        userInfo = xmlManager.itemDB.user;
        user_aws = awsManager.user;

        StartCoroutine(xmlManager.itemDB.StartTimer());
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_ANDROID
                Application.Quit();
            #endif
        }
    }








    /// <summary>
    /// Web Request ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>



    public void UpdateUserData(string cloud_id)
    {
        id = cloud_id;
        JsonAdapter jsonAdapter = new JsonAdapter();

        StartCoroutine(jsonAdapter.API_GET("account/info?id=" + id, callback => {

            string fixdata = JsonHelper.fixJson(callback);
            UserData[] datas = JsonHelper.FromJson<UserData>(fixdata);
            UserData selectedData = datas[0];

            //user = selectedData;

        }));

        StartCoroutine(jsonAdapter.API_GET("stage/info?id=" + id, callback => {

            string fixdata = JsonHelper.fixJson(callback);
            StageData[] datas = JsonHelper.FromJson<StageData>(fixdata);
            Debug.Log("stage clear : " + datas.Length);

            for (int i = 0; i < datas.Length; i++)
            {
                stages[datas[i].stage_num].stage_step = datas[i].stage_step;
            }

        }));

        StartCoroutine(jsonAdapter.API_GET("editorPlay/all?player_id=" + id, callback =>
        {
            if (callback == null)
            {
                
                //retry?
            }
            else
            {
                //successfully loaded the map
                customStagePlayerDatas.Clear();

                string fixdata = JsonHelper.fixJson(callback);
                customStagePlayerDatas.AddRange(JsonHelper.FromJson<CustomStagePlayerData>(fixdata));//all map data



            }
        }));




    }
    public void SetText(string txt)
    {
        debugTxt.text = debugTxt.text + "\n" + txt;

    }

    public IEnumerator LoadCustomMapList(System.Action<bool> load)//call by editor play popup open...(ButtonManager_Main.cs)
    {


        JsonAdapter adapter = new JsonAdapter();
        yield return StartCoroutine(adapter.API_GET(/*"map/all?nickname=" + user.nickname*/"test/", callback =>
         {
             if (callback == null)
             {
                //StartCoroutine(LoadCustomMapList(repeat + 1));
                load(false);
             }
             else
             {
                //successfully loaded the map
                customMapdatas.Clear();

                 string fixdata = JsonHelper.fixJson(callback);
                 customMapdatas.AddRange(JsonHelper.FromJson<JsonData>(fixdata));//all map data

                load(true);

             }
         }));


        yield break;


    }


}
