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
    public Map customMap;
    public Vector2 maxSize;

    public int nowLevel;

    
    
    
    //public List<StageData> stages;
    //public List<CustomStagePlayerData> customStagePlayerDatas;

    public GameObject canvas;

    //public List<JsonData> customMapdatas = new List<JsonData>();
    public CustomMapItem playCustomData;

    AWSManager awsManager = AWSManager.instance;
    XMLManager xmlManager = XMLManager.ins;
    CSVManager csvManager = CSVManager.instance;
    JsonAdapter jsonAdapter = JsonAdapter.instance;

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
        csvManager = CSVManager.instance;
        jsonAdapter = JsonAdapter.instance;

        
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

    }
    public void SetText(string txt)
    {
        debugTxt.text = debugTxt.text + "\n" + txt;

    }

    public IEnumerator LoadCustomMapList(System.Action<bool> load)//call by editor play popup open...(ButtonManager_Main.cs)
    {
        yield break;

    }


}
