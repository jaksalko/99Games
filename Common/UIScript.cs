using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


   

public class UIScript : MonoBehaviour
{
    protected GameManager gameManager = GameManager.instance;
    protected AWSManager awsManager = AWSManager.instance;
    protected XMLManager xmlManager = XMLManager.ins;
    protected JsonAdapter jsonAdapter = JsonAdapter.instance;
    
    void Awake()
    {
        gameManager = GameManager.instance;
        awsManager = AWSManager.instance;
        xmlManager = XMLManager.ins;
        jsonAdapter = JsonAdapter.instance;

    }
    void Start()
    {
        gameManager = GameManager.instance;
        awsManager = AWSManager.instance;
        xmlManager = XMLManager.ins;
        jsonAdapter = JsonAdapter.instance;

    }
    public void Load_Island(int stage)
    {
        if(stage <= IslandData.tutorial)
        {
            SceneManager.LoadScene("Tutorial_Island");
        }
        else if(stage <= IslandData.iceCream)
        {
            SceneManager.LoadScene("Icecream_Island");
        }
        else if (stage <= IslandData.beach)
        {
            SceneManager.LoadScene("Beach_Island");
        }
        else if (stage <= IslandData.cracker)
        {
            SceneManager.LoadScene("Cracker_Island");
        }
        else if (stage <= IslandData.cottoncandy)
        {
            SceneManager.LoadScene("Cottoncandy_Island");
        }
    }

	public int Island_Name(int stage)
	{
        for(int i = 0 ; i < IslandData.island_last.Length ; i++)
        {
            if(stage <= IslandData.island_last[i])
            {
                Debug.Log("island num : " + IslandData.island_last[i]);
                return i;
            }
        }

        return IslandData.island_last.Length-1;
        
    }

    public string StageText(int stage)
    {
        string stageString = "STAGE ";
        for(int i = 0 ; i < IslandData.island_last.Length ; i++)
        {
            if(stage <= IslandData.island_last[i])
            {
                
                stageString += (i+1).ToString() + " - ";
                if(i == 0)
                {
                    stageString += (stage+1).ToString();
                }
                else
                {
                    stageString += (stage - IslandData.island_last[i-1]).ToString();
                }
                break;
            }
        }
        return stageString;


        
    }

    public void ExitButton()
    {
        gameObject.SetActive(false);
    }
}
