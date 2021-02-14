using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevData : MonoBehaviour
{
    public Player p1;
    public Player p2;

    public Text p1Text;
    public Text p2Text;

    float deltaTime = 0.0f;
    float fps;
    private void Awake()
    {
        if (GameManager.instance.id == "TestPlayerID" || GameManager.instance.id == "U:9b4f1778f9068ddc8e5f9648a916e74b")
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);

        //Application.targetFrameRate = 60;



    }
    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;

        p1Text.text = "Player1 Data\n isMoving : " + p1.Moving() + "\n isActive : " + p1.isActive
            + "\n dir : " + p1.direction + "\n onCloud : " + p1.onCloud + "\n isLock : " + p1.isLock
            + "\n stateChange : "+p1.stateChange+"\n temp : " + p1.temp + "\n fps : " + fps;

        p2Text.text = "Player2 Data\n isMoving : " + p2.Moving() + "\n isActive : " + p2.isActive
            + "\n dir : " + p2.direction + "\n onCloud : " + p2.onCloud + "\n isLock : " + p2.isLock
             + "\n stateChange : " + p2.stateChange + "\n temp : " + p2.temp;
    }
}
