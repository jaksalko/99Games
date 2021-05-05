using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StageSceneResultPopup : UIScript
{
    public TextMeshProUGUI moveCount;
    public TextMeshProUGUI snowCount;
    public TextMeshProUGUI stageText;

    public Image[] starImage;

    public GameObject successPopup;
    public GameObject failPopup;
    public GameObject successEffect;
    public Button homeBtn;
    public Button retryButton;
    public Button nextButton;
    public void ShowResultPopup(bool isSuccess,int remain_snow,int move_count , int star_count)
    {
        stageText.text = StageText(gameManager.nowLevel);

        if(isSuccess)
        {
            if(gameManager.nowLevel < csvManager.islandData.tutorial)
            {
                homeBtn.gameObject.SetActive(false);
            }

            successEffect.SetActive(true);
            successPopup.SetActive(true);
            for(int i = 0 ; i < csvManager.islandData.island_last.Length ; i++)
            {
                Debug.Log(gameManager);
                Debug.Log(csvManager.islandData.island_last.Length);
                Debug.Log(csvManager.islandData.island_last[i]);

                if(gameManager.nowLevel == csvManager.islandData.island_last[i])
                {
                    nextButton.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            failPopup.SetActive(true);
            nextButton.gameObject.SetActive(false);
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
    public void ReplayButtonClicked()
    {
        
        if(awsManager.userInfo.heart > 0)
        {
            awsManager.userInfo.heart -= 1;
            awsManager.userHistory.heart_use++;

            JsonAdapter.instance.UpdateData(awsManager.userInfo, "userInfo", ReplayCallback);
            JsonAdapter.instance.UpdateData(awsManager.userHistory, "userInfo", ReplayCallback);
            
        }
    }
    public void NextStageButtonClicked()
    {
        gameManager.nowLevel++;
        Load_Island(GameManager.instance.nowLevel);
    }

    void ReplayCallback(bool success)
    {
        if(success)
        {
            if(JsonAdapter.instance.EndLoading())
            {
                Load_Island(GameManager.instance.nowLevel);
            }
            else
            {
                Debug.Log("loading..");
            }
        }
        else
        {
            Debug.LogError("Error");
        
        }
    }
}
