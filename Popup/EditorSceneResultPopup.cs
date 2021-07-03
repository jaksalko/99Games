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
    public Text warningText;
    public Text moveCount;
    public Image levelImage;

    public InputField titleInputField;

    Coroutine warningCoroutine;
   
    int move;
    int dif;

    int try_count = 0;

    public void ShowResultPopup(int count , int level)
    {
        move = count;
        dif = level;

        moveCount.text = "x" + count;
        levelImage.sprite = Resources.Load<Sprite>("Icon/Level/" + dif);

        
        gameObject.SetActive(true);
    }

    public void ModifyButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MakeCustomStageClicked(int try_count)
    {
        Debug.Log("connect try : " + try_count);

        if(try_count >= 5)
        {
            //AWSManager.instance.ConnectWithAWS(true);
            return;
        }

        string title_regex = "^[a-zA-Z가-힣0-9]{1}[a-zA-Z가-힣0-9\\s]{0,6}[a-zA-Z가-힣0-9]{1}$";
        Regex regex = new Regex(title_regex);

        if(regex.IsMatch(titleInputField.text))
        {
            //AWSManager.instance.ConnectWithAWS(false);

            Debug.Log("match");
            
            Map newMap = GameController.instance.GetMap();
            newMap.map_title = titleInputField.text.ToString();

            EditorMap editorMap = new EditorMap(map : newMap, nick : AWSManager.instance.userInfo.nickname , moveCount : move );
            JsonAdapter.instance.CreateEditorMap(editorMap, CreateEditorMapCallback);
            //AWSManager.instance.CreateEditorMap(newMap, move, dif, CreateEditorMapCallback );


            //성공 시 로비로 이동

            //실패 시 --> 연결 문제 --> 재시도
            //재시도 횟수 ?번이상일 시 통신 문제 출력


        }
        else
        {
            Debug.Log("no match");
            titleInputField.text = "";
            
            StopAllCoroutines();
            StartCoroutine(WarningText());
            

        }

        
        

        //Web Request

    }

    void CreateEditorMapCallback(bool success)
    {
        //AWSManager.instance.ConnectWithAWS(success);
        if (success)
        {
            if(JsonAdapter.instance.EndLoading())
            {
                SceneManager.LoadScene("MainScene");
            }
            
        }
        else
        {
            
           
        }
    }

    IEnumerator WarningText()
    {
        warningText.gameObject.SetActive(true);
        float time = 0;

        Color warningTextColor = warningText.color;

        while(time <= 3)
        {
            time += Time.deltaTime;
            warningTextColor.a = time * 2;
            warningText.color = warningTextColor;
            yield return null;
        }
        warningText.gameObject.SetActive(false);
        yield break;
        /*JsonAdapter jsonAdapter = new JsonAdapter();
        
        JsonData customStage =
            new JsonData(GameManager.instance.user.nickname, stageTitle.text, newMap, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), move, dif);

        var json = JsonUtility.ToJson(customStage);

        yield return StartCoroutine(jsonAdapter.API_POST("editor/generate" , json , callback => { }));
        SceneManager.LoadScene("MainScene");*/

        yield break;
    }
}
