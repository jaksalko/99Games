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
    protected CSVManager csvManager = CSVManager.instance;
    
    void Start()
    {
        gameManager = GameManager.instance;
        awsManager = AWSManager.instance;
        xmlManager = XMLManager.ins;
        jsonAdapter = JsonAdapter.instance;
        csvManager = CSVManager.instance;

    }
    public void Load_Island(int stage)
    {
        if(stage <= csvManager.islandData.tutorial)
        {
            SceneManager.LoadScene("Tutorial_Island");
        }
        else if(stage <= csvManager.islandData.icecream)
        {
            SceneManager.LoadScene("Icecream_Island");
        }
        else if (stage <= csvManager.islandData.beach)
        {
            SceneManager.LoadScene("Beach_Island");
        }
        else if (stage <= csvManager.islandData.cracker)
        {
            SceneManager.LoadScene("Cracker_Island");
        }
        else if (stage <= csvManager.islandData.cottoncandy)
        {
            SceneManager.LoadScene("Cottoncandy_Island");
        }
    }

	public int Island_Name(int stage)
	{
        for(int i = 0 ; i < csvManager.islandData.island_last.Length ; i++)
        {
            if(stage <= csvManager.islandData.island_last[i])
            {
                Debug.Log("island num : " + csvManager.islandData.island_last[i]);
                return i;
            }
        }

        return csvManager.islandData.island_last.Length-1;
        
    }

    public string StageText(int stage)
    {
        string stageString = "STAGE ";
        for(int i = 0 ; i < csvManager.islandData.island_last.Length ; i++)
        {
            if(stage <= csvManager.islandData.island_last[i])
            {
                
                stageString += (i+1).ToString() + " - ";
                if(i == 0)
                {
                    stageString += (stage+1).ToString();
                }
                else
                {
                    stageString += (stage - csvManager.islandData.island_last[i-1]).ToString();
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
