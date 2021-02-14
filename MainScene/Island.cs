using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Island : MonoBehaviour
{
    public TextMeshProUGUI nowStage;
    public RewardSlider rewardSlider;

    XMLManager xmlManager = XMLManager.ins;
    CSVManager csvManager = CSVManager.instance;
    int stage;
    public int island;

    public GameObject medal;
    public Image island_image;

    

    private void Start()
    {

        xmlManager = XMLManager.ins;
        csvManager = CSVManager.instance;
        stage = xmlManager.itemDB.user.current_stage;

        if(nowStage != null)//stage text 메인 씬에만 존재
        {
            SetStageText();
            SetSlider();
            
        }
        else
        {
            SetLevelSceneIsland();
           
        }



        //slider 숫자 표기하기 게이지 나타내기
    }

    void SetIslandState()
    {

    }

    public void GetReward()
    {

    }

    void SetStageText() // only mainscene button
    {
        string stageString = "STAGE ";
        for(int i = 0 ; i < IslandData.island_last.Length ; i++)
        {
            
            if(stage <= IslandData.island_last[i])//stage == current stage
            {
                island = i;
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
            nowStage.text = stageString;
        


        
    }

    void SetSlider()
    {
        int stage_num = csvManager.islands[island].maps.Count;

        int maxValue = stage_num * 3;
        int userValue = 0;

        int island_start_num = 0;

        if (island != 0)
            island_start_num = IslandData.island_last[island - 1] + 1;//전 섬의 마지막 스테이지 번호 다음부터 시작을 의미

        for (int i = island_start_num; i <= stage; i++)
        {
            userValue += xmlManager.itemDB.user.star_list[i];
        }

        rewardSlider.SetSlider(island,maxValue, userValue, csvManager.GetRewardList(island));
    }

    void SetLevelSceneIsland()
    {
        int stage_num = csvManager.islands[island].maps.Count;
        

        int island_start_num = 0;

        if(island != 0 )
            island_start_num = IslandData.island_last[island-1]+1;//전 섬의 마지막 스테이지 번호 다음부터 시작을 의미

        int maxValue = stage_num * 3;
        int userValue = 0;

        if(stage <= IslandData.island_last[island])//아직 그 섬을 깨지 못함
        {
            medal.SetActive(false);
            island_image.sprite = Resources.Load<Sprite>("LevelScene/island_" + island + "_none");

            for (int i = island_start_num ; i <= stage ; i++)
            {
                userValue += xmlManager.itemDB.user.star_list[i];
            }
        }
        else//이미 클리어한 섬
        {
            medal.SetActive(true);
            island_image.sprite = Resources.Load<Sprite>("LevelScene/island_"+island+"_clear");


            for (int i = island_start_num ; i <= IslandData.island_last[island] ; i++)
            {
                userValue += xmlManager.itemDB.user.star_list[i];
            }
        }

        rewardSlider.SetSlider(island,maxValue, userValue, csvManager.GetRewardList(island));
    } 

   
        
}
