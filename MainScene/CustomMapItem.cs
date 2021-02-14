using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomMapItem : MonoBehaviour
{
    JsonAdapter jsonAdapter = new JsonAdapter();

    public Text title;
    public Text maker;
    public Text moveCount;
    public Toggle isClear;
    // Start is called before the first frame update
    public JsonData itemdata;

    public bool isPlayed;
    public void Initialize(JsonData item)
    {
        name = item.updateTime;
        itemdata = item;
        title.text = itemdata.title;
        maker.text = itemdata.nickname;
        moveCount.text = itemdata.moveCount.ToString();

        List<CustomStagePlayerData> myPlayData = GameManager.instance.customStagePlayerDatas;
        isPlayed = false;
        for (int i = 0; i < myPlayData.Count; i++)
        {
            if(myPlayData[i].title == itemdata.title)
            {
                isPlayed = true;
                break;
            }
        }

    }

    public void PlayButton()
    {
        /*
        GameManager gameManager = GameManager.instance;
        gameManager.playCustomData = this;

        

        UserData user = new UserData(gameManager.user.id, change_cash : 0, change_heart: -1, gameManager.user.stage);
        var json = JsonUtility.ToJson(user);
        StartCoroutine(jsonAdapter.API_POST("account/update", json, callback => {

            gameManager.user.heart -= 1;

        }));


        


        //popularity++
        if (!isPlayed)
        {
            
            json = JsonUtility.ToJson(itemdata);
            StartCoroutine(jsonAdapter.API_POST("map/play", json , callback => { }));//popularity++

        }

        */
        SceneManager.LoadScene("CustomMapPlayScene");//customMode Scene
    }
}
