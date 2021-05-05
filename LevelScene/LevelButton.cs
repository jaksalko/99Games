using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class LevelButton : UIScript
{
    public int stage;
    public int island;
    public TextMeshProUGUI stageText;
    public Text moveCount;
    public Image myStar;
    public Sprite[] starImages;//in resource directory

    public StagePopup stagePopup;


    // Start is called before the first frame update
 
    public void SetStarImage(int star_num)
    {
        myStar.sprite = starImages[star_num];
    }

    public void StageText()
    {
        string stageString = "STAGE ";

        for(int i = 0 ; i < csvManager.islandData.island_last.Length ; i++)
        {
            if(stage <= csvManager.islandData.island_last[i])
            {
                island = i;
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
        stageText.text = stageString;


        
    }

    public void LevelButtonClicked()
    {
        
        stagePopup.SetPopup(stageText.text , stage , myStar.sprite);
        stagePopup.gameObject.SetActive(true);
    }

    
}
