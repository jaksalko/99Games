using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class EditorSceneResultPopup : MonoBehaviour
{
    public Text moveCount;
    public GameObject[] difficulty;//0-4 : maximum 5 difficulty
    public InputField stageTitle;

    Map newMap;
    int move;
    int dif;

    public void ShowResultPopup(int count , int level)
    {
        move = count;
        dif = level;

        moveCount.text = count.ToString();
       
        /*for(int i = 1 ; i <= level; i++)
        {
            difficulty[i-1].SetActive(true);
        }*/

        newMap = GameController.instance.mapLoader.editorMap;

        gameObject.SetActive(true);
    }

    public void GoLobbyButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MakeCustomStageClicked()
    {
        string title_regex = "^[a-zA-Z가-힣0-9]{1}[a-zA-Z가-힣0-9]{1,7}$";
        Regex regex = new Regex(title_regex);




        //Web Request
        StartCoroutine(InsertCustomStage());
    }

    IEnumerator InsertCustomStage()
    {
        /*JsonAdapter jsonAdapter = new JsonAdapter();
        
        JsonData customStage =
            new JsonData(GameManager.instance.user.nickname, stageTitle.text, newMap, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), move, dif);

        var json = JsonUtility.ToJson(customStage);

        yield return StartCoroutine(jsonAdapter.API_POST("editor/generate" , json , callback => { }));
        SceneManager.LoadScene("MainScene");*/

        yield break;
    }
}
