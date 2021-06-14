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
    public Text stageText;

    public Image[] starImage;

    public GameObject successPopup;
    public GameObject failPopup;
    public GameObject successEffect;

    public Button likeButton;

    public void ShowResultPopup(bool isSuccess, int remain_snow, int move_count, int star_count, bool retry)
    {
        if (retry)
            likeButton.gameObject.SetActive(false);
        else
            likeButton.gameObject.SetActive(true);

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



    public void LikeButtonClicked()
    {
        
        jsonAdapter.UpdateData(gameManager.playCustomData, "editorMap/like", UpdateEditorMapCallback);
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

    void UpdateEditorMapCallback(bool success)
    {
        if (success)
        {
            if (jsonAdapter.EndLoading())
            {
                likeButton.interactable = false;
                gameManager.playCustomData.likes++;
                
                    CustomMapItem map = awsManager.editorMap.Find(x => x.itemdata.map_id == GameManager.instance.playCustomData.map_id);
                    map.itemdata = GameManager.instance.playCustomData;
                map.likes.text = map.itemdata.likes.ToString();
            }

        }
        else
        {
            Debug.LogError("fail like button");
        }
    }
}
