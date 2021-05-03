using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class CustomsSceneResultPopup : UIScript
{
    public TextMeshProUGUI moveCount;
    public TextMeshProUGUI snowCount;
    public TextMeshProUGUI stageText;

    public Image[] starImage;

    public GameObject successPopup;
    public GameObject failPopup;
    public GameObject successEffect;



    public void ShowResultPopup(bool isSuccess, int remain_snow, int move_count, int star_count)
    {

        stageText.text = gameManager.customMap.map_title;

        if (isSuccess)
        {
            
            successEffect.SetActive(true);
            successPopup.SetActive(true);
            
        }
        else
        {
            failPopup.SetActive(true);
            
        }

        moveCount.text = move_count.ToString();
        snowCount.text = remain_snow.ToString();
        starImage[star_count].gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void GoLobbyButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void RetryButtonClicked()
    {
        SceneManager.LoadScene("CustomMapPlayScene");//customMode Scene
    }



    public void PushButtonClicked()
    {
        /*
        JsonData jsonData = GameManager.instance.playCustomData.itemdata;

        var json = JsonUtility.ToJson(jsonData);
        StartCoroutine(jsonAdapter.API_POST("map/push", json , callback => { }));
        StartCoroutine(jsonAdapter.API_POST("editorPlay/push", json , callback => { }));
        */
        //map push++
        //candy++
        //show ad...
    }
}
