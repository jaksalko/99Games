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
    AWSManager awsManager = AWSManager.instance;
    int user_stage;
    public int island;

    public GameObject medal;
    public Image island_image;

    

    private void Start()
    {

        xmlManager = XMLManager.ins;
        csvManager = CSVManager.instance;
        user_stage = awsManager.userInfo.stage_current;

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

    

    void SetStageText() // only mainscene button
    {
        string stageString = "STAGE ";
        for(int i = 0 ; i < csvManager.islandData.island_last.Length ; i++)
        {
            
            if(user_stage <= csvManager.islandData.island_last[i])//stage == current stage
            {
                island = i;
                stageString += (i+1).ToString() + " - ";
                if(i == 0)
                {
                    stageString += (user_stage + 1).ToString();
                }
                else
                {
                    stageString += (user_stage - csvManager.islandData.island_last[i-1]).ToString();
                }
                break;
            }
        }
            nowStage.text = stageString;
        


        
    }

    void SetSlider()
    {
        int stage_num = csvManager.islands[island].maps.Count;

        int maxValue = stage_num * 3;//별의 최대 개수 = 맵 개수 *3
        int userValue = 0; //해당 스테이지에서 유저가 가지고 있는 

 
        for (int i = csvManager.islandData.island_start[island]; i < user_stage; i++)//섬의 시작 번호부터 유저 스테이지의 전까지 맵에서 유저가 얻은 별의 갯수를 합침
        {
            userValue += awsManager.userStage[i].stage_star;
        }

        //섬 번호 //해당 섬에서의 별 최대 값 //유저의 보유 별의 개수 // 해당 섬에서 얻을 수 있는 보상(3개)
        rewardSlider.SetSlider(island,maxValue, userValue, csvManager.GetRewardList(island));
    }

    void SetLevelSceneIsland()
    {
        int stage_num = csvManager.islands[island].maps.Count;

        int maxValue = stage_num * 3;
        int userValue = 0;

        if(user_stage <= csvManager.islandData.island_last[island])//아직 그 섬을 깨지 못함
        {
            medal.SetActive(false);
            island_image.sprite = Resources.Load<Sprite>("LevelScene/island_" + island + "_none");

            for (int i = csvManager.islandData.island_start[island]; i < user_stage; i++)
            {
                userValue += awsManager.userStage[i].stage_star;
            }
        }
        else//이미 클리어한 섬
        {
            medal.SetActive(true);
            island_image.sprite = Resources.Load<Sprite>("LevelScene/island_"+island+"_clear");


            for (int i = csvManager.islandData.island_start[island]; i <= csvManager.islandData.island_last[island] ; i++)
            {
                userValue += awsManager.userStage[i].stage_star;
            }
        }

        rewardSlider.SetSlider(island,maxValue, userValue, csvManager.GetRewardList(island));
    } 

   
        
}
