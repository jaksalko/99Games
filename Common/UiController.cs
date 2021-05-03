using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;



public class UiController : UIScript
{
    public GameObject inGame;

    public GameObject pausePopup;
    public GameObject settingPopup;
    public TextMeshProUGUI pauseStageText;
    public Button pausePopup_retryButton;

    int order = 0;
    public GameObject[] parfaitOrder;
    public GameObject[] parfaitOrder_done;

    public GameObject mission_default;
    public GameObject mission_parfait;

    public StageSceneResultPopup stageSceneResultPopup;
    public CustomsSceneResultPopup customSceneResultPopup;
    public EditorSceneResultPopup editorSceneResultPopup;

	public Button nextLevelBtn;
    
    public Text devtext;
    public Text remainText;
    public Text moveText;

    bool mini = false;

    public Button player1;
    public Button player2;


    public Button revertButton;


    public StarSlider starSlider;
    List<int> star_limit = new List<int>();
    

    private void Start()
    {
        devtext.text = "platform : " + Application.platform + "\n" + "level : " + PlayerPrefs.GetInt("level", 0);

        player1.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/Skin/" + awsManager.userInfo.skin_a);
        player2.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icon/Skin/" + awsManager.userInfo.skin_b);

        SpriteState spriteState = new SpriteState();

        spriteState = player1.spriteState;
        spriteState.disabledSprite = Resources.Load<Sprite>("Icon/Skin/" + awsManager.userInfo.skin_a + "_on");
        player1.spriteState = spriteState;

        spriteState = player2.spriteState;
        spriteState.disabledSprite = Resources.Load<Sprite>("Icon/Skin/" + awsManager.userInfo.skin_b + "_on");
        player2.spriteState = spriteState;

        player1.interactable = false;
        player2.interactable = true;


    }

    public void GameEnd(bool isSuccess, int star,int remain_snow, int moveCount, bool custom , bool editor)
    {
        //inGame.SetActive(false);
        //SetMoveCountText(moveCount);

        //infinite --> 종료 팝업 선택 버튼 : 다음 맵 / 로비로?
        //editor --> 종료 팝업 선택 버튼 : 생성할지 말지
        //Default --> 종료 팝업 선택 버튼 : 다음 스테이지 / 로비로
        if (custom)
        {
            customSceneResultPopup.ShowResultPopup(isSuccess, remain_snow, moveCount, star_count: star);
        }
        else if(editor)
        {
            int level = (moveCount / 5) + 1;
            if (level > 5) level = 5;

            

            editorSceneResultPopup.ShowResultPopup(moveCount, level);
        }
        else
        {
            stageSceneResultPopup.ShowResultPopup(isSuccess,remain_snow, moveCount , star_count : star);
        }
       
    }

    #region 인게임 UI
    public void SetRemainText(int remain, int total)//inGame UI
    {
        remainText.text = remain + "/" + total;
    }
    public void SetMoveCountText(int count , int max)//Result UI
    {
        Debug.Log(count +"," + max);
        Debug.Log(starSlider);
        starSlider.SetSliderValue(count);
        int remain_move = max - count;
        moveText.text = remain_move + "/" + max;
    }
    public void SetSlider(List<int> star_list , int maxValue)
    {
        starSlider.SetSlider(star_list, maxValue);
        
    }
    public void ParfaitDone()
    {
        parfaitOrder[order].SetActive(false);
        parfaitOrder_done[order].SetActive(true);
        order++;
    }
    
    #endregion

    #region 결과 창 UI
    
    public void Pause()
    {
        GameController.instance.SetPlaying(false);
        pauseStageText.text = StageText(gameManager.nowLevel);

        if(AWSManager.instance.userInfo.heart == 0 )
        {
            pausePopup_retryButton.interactable = false;
        }

        pausePopup.SetActive(true);
    }

    public void SettingButtonClicked()
    {
        GameController.instance.SetPlaying(false);
        settingPopup.SetActive(true);
    }

    public void Reset()
    {

        AWSManager.instance.userInfo.heart--;
        AWSManager.instance.userHistory.heart_use++;
        JsonAdapter.instance.UpdateData(AWSManager.instance.userInfo,"userInfo", ResetCallback);
        JsonAdapter.instance.UpdateData(AWSManager.instance.userHistory,"userHistory", ResetCallback);
        
    }
    void ResetCallback(bool success)
    {
        if(success)
        {
            if(JsonAdapter.instance.EndLoading())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {

        }
    }
    public void GoLobby()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Resume()
    {
        GameController.instance.SetPlaying(true);
        pausePopup.SetActive(false);
    }
    #endregion


    public void MiniMapButton()
    {
        Player now = GameController.instance.nowPlayer;

        if (!now.Moving() && !now.other.Moving())
        {
            mini = GameController.instance.cameraController.MiniMapView(mini);
            GameController.instance.SetPlaying(!mini);
        }
            
    }

    public void ReturnButton()
    {
        Player now = GameController.instance.nowPlayer;

        if (!now.Moving() && !now.other.Moving())
        {
            GameController.instance.moveCommand.Undo();
        }
    }


    public void ChangeCharacter(int player_num)
    {
        GameController instance = GameController.instance;

        Player now = instance.nowPlayer;

        Player player;
        if(player_num == 1)
        {
            player = instance.player1;
        }
        else
        {
            player = instance.player2;
        }

        if(!now.Moving() && !now.other.Moving())
        {
            now.isActive = false;
            instance.nowPlayer = player;
            player.isActive = true;

            if(player == GameController.instance.player1)
            {
                player1.interactable = false;
                

                player2.interactable = true;
                
            }
            else
            {
                player1.interactable = true;
                

                player2.interactable = false;
            }
            
        }

		
        
    }
    public void MasterFocus(Player master)
    {
        GameController.instance.nowPlayer = master;
        GameController.instance.nowPlayer.isActive = true;
        Debug.Log("master : " + master.name);
    }
    public void NextLevel()
    {
        //GameController googleinstance level++....
        if(GameController.instance.customMode)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
            Load_Island(GameManager.instance.nowLevel);
        
    }

   


    
}
