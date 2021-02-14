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
            if(gameManager.nowLevel <= IslandData.tutorial)
            {
                homeBtn.gameObject.SetActive(false);
            }

            successEffect.SetActive(true);
            successPopup.SetActive(true);
            for(int i = 0 ; i < IslandData.island_last.Length ; i++)
            {
                Debug.Log(gameManager);
                Debug.Log(IslandData.island_last.Length);
                Debug.Log(IslandData.island_last[i]);
                if(gameManager.nowLevel == IslandData.island_last[i])
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
        
        if(gameManager.userInfo.heart > 0)
        {
            gameManager.userInfo.heart -= 1;
            Load_Island(GameManager.instance.nowLevel);
        }
    }
    public void NextStageButtonClicked()
    {
        gameManager.nowLevel++;
        Load_Island(GameManager.instance.nowLevel);
    }
}
