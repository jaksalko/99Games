using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;

using UnityEngine.SceneManagement;

using UniRx;
using UniRx.Triggers;


[Serializable]
public class MapCount
{
    public int count;
}
public class Simulator : MonoBehaviour
{
    public SimulatorCameraController cameraController;

    public static Simulator instance;
    public GameObject simulatorUI;
    public Player player1;
    public Player player2;
    public Player nowPlayer;

    public MapLoader simulatingMap;

    public SimulatorObject simulatorObject;
    public GameObject generatorObject;
    public MapGenerator mapGenerator;
    SimulatorObject subSimulatorObject;
    public GameObject successPopup;
    public Text MoveCountTxt;

    //public UiController ui;
    //public CameraController cameraController;
    Vector3 backUpPositionA;
    Vector3 backUpPositionB;


    private bool isRunning;
    public static bool Running
    {
        get { return instance.isRunning; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);



        

    }
    void ChangeOutline()
    {
        
        if (nowPlayer == player1)
        {
            player1.transform.GetChild(0).transform.GetChild(1).GetComponent<Renderer>().sharedMaterial.SetFloat("_OutlineWidth", 1.08f);
            player2.transform.GetChild(0).transform.GetChild(1).GetComponent<Renderer>().sharedMaterial.SetFloat("_OutlineWidth", 1.0f);
        }
        else
        {
            player2.transform.GetChild(0).transform.GetChild(1).GetComponent<Renderer>().sharedMaterial.SetFloat("_OutlineWidth", 1.08f);
            player1.transform.GetChild(0).transform.GetChild(1).GetComponent<Renderer>().sharedMaterial.SetFloat("_OutlineWidth", 1.0f);
        }
    }
    public void StartSimulator()
    {

        //set Player...
        generatorObject.SetActive(false);
        simulatorObject.gameObject.SetActive(true);

        simulatingMap.GenerateMap(0);

        //player1.SetSimulatorData();
        //player2.SetSimulatorData();

        backUpPositionA = player1.transform.position;
        backUpPositionB = player2.transform.position;


        //player1.SetPosition(simulatingMap.liveMap.startPositionA + new Vector3(0, -0.5f, 0), simulatingMap.liveMap.startUpstairA); //position correction fix .. 5/13
        //player2.SetPosition(simulatingMap.liveMap.startPositionB + new Vector3(0, -0.5f, 0), simulatingMap.liveMap.startUpstairB);

        
        
        
        //ui.inGame.SetActive(true);
        

        cameraController.gameObject.SetActive(true);

        this.UpdateAsObservable()
           .Subscribe(_ => ChangeOutline());

    }
    public void GameStart()
    {
        simulatorUI.SetActive(true);

//        player1.FindPlayer();
  //      player2.FindPlayer();

        nowPlayer = player1;
        nowPlayer.isActive = true;

        isRunning = true;

    }
    public void SimulatingSuccess()//Can Make MapLoader
    {
        isRunning = false;
        Debug.Log("can build!");
        //int count = player1.moveCount + player2.moveCount;
        //int level = (count / 5) + 1;
        //StartCoroutine(INSERTMAP());
        //MoveCountTxt.text = "이동횟수 : " + count + "\n" + "난이도 : " + level;
        successPopup.SetActive(true);

    }
   
    public void GoToLobbyButton()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void GenerateModeButton()
    {
        isRunning = false;
        player1.transform.position = backUpPositionA;
        player2.transform.position = backUpPositionB;

        subSimulatorObject = Instantiate(simulatorObject, transform.position, Quaternion.identity);
        mapGenerator.newMap = subSimulatorObject.simulatingMap.sample[0];

        Destroy(simulatorObject.gameObject);
        simulatorObject = subSimulatorObject;
        simulatorObject.Adapting();
        generatorObject.SetActive(true);
        simulatorObject.gameObject.SetActive(false);
    }
    public void ResetButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
 
   
    public void ChangeCharacter()
    {
        

        if (!nowPlayer.Moving())//now player is not moving
        {
            Debug.Log("change Character");
            nowPlayer.isActive = false;

            if (nowPlayer == player1)
            {
                nowPlayer = player2;
            }
            else
            {
                nowPlayer = player1;
            }
            nowPlayer.isActive = true;

            Debug.Log("player 1 : " + player1.isActive);
            Debug.Log("player 2 : " + player2.isActive);
        }
        else
        {
            Debug.Log("Can't change!");
        }

    }
    public void MasterFocus(Player master)
    {
        nowPlayer = master;
        nowPlayer.isActive = true;
        Debug.Log("master : " + nowPlayer.name);
    }

}
