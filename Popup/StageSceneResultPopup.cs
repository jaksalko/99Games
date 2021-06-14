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

    public GameObject failEffect;
    public GameObject successEffect;
    public GameObject successPopup; //star image //move text //3 buttons //stage text 
    public GameObject failPopup;    //remain snow text //2 buttons // stage text

    public Transform buttons;
    public Button homeBtn;
    public Button retryButton;
    public Button nextButton;

    public AudioSource successAudio;
    public void ShowResultPopup(bool isSuccess,int remain_snow,int move_count , int star_count)
    {
        stageText.text = StageText(gameManager.nowLevel);

        if(isSuccess)
        {
            stageText.transform.SetParent(successPopup.transform, false);
            buttons.SetParent(successPopup.transform, false);

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
            stageText.transform.SetParent(failPopup.transform, false);
            buttons.SetParent(failPopup.transform, false);

            failEffect.SetActive(true);
            failPopup.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }

        moveCount.text = move_count.ToString();
        snowCount.text = remain_snow.ToString();

        for(int i = 0; i < starImage.Length; i++)
        {
            if(i < star_count)
            {
                starImage[i].sprite = Resources.Load<Sprite>("Popup/star_clear_" + (i + 1));
            }
            else
            {
                starImage[i].sprite = Resources.Load<Sprite>("Popup/star_fail_" + (i + 1));
            }
        }
       
        gameObject.SetActive(true);

    }

    private void Start()
    {
        successAudio.Play();
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
            JsonAdapter.instance.UpdateData(awsManager.userHistory, "userHistory", ReplayCallback);
            
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
