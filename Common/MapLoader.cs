using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;




public class MapLoader : MonoBehaviour
{

    public BlockFactory blockFactory;
    int style;

    public ParfaitBlock[] parfaitBlock;

   

    public Map editorMap;//made by editor scene
    public List<Map> sample;//island map datas

    public Vector3 centerOfMap;
    public Transform minimapTarget;

    [Header("DESIGN OBJECT")]
    public Transform[] design;
    public Transform waterQuad;


    public Map liveMap;//on stage...

    
    CSVManager csvManager = CSVManager.instance;
    public CSVManager CSVManager_
    {
        get
        {
            if (csvManager == null)
            {
                csvManager = (CSVManager)FindObjectOfType(typeof(CSVManager));
            }

            return csvManager;
        }
        
    }
    void Start()
    {
        csvManager = CSVManager.instance;
    }
   
    void MakeMap(int mapsizeH , int mapsizeW , bool parfait)
    {
       
        
        centerOfMap = new Vector3((float)(mapsizeW - 1) / 2, -0.5f, (float)(mapsizeH - 1) / 2);
        minimapTarget.position = centerOfMap;

        waterQuad.position = centerOfMap;
        waterQuad.localScale = new Vector3(mapsizeW-2, mapsizeH-2, 1);

        //design[0].position = centerOfMap + new Vector3(0, -0.5f, 0);
        //design[1].localPosition = design[1].localPosition + new Vector3(0, 0, centerOfMap.z + 8);
        //design[2].localPosition = design[2].localPosition + new Vector3(0, 0, -(centerOfMap.z + 8));


       

        MakeGround(mapsizeH,mapsizeW);//block into map object block list

        /*
        if (parfait)
            MakeParfait(mapsizeH, mapsizeW);
        */
        
       
        
    }

   
    
    void MakeGround(int mapsizeH, int mapsizeW)
    {
        


        for (int i = 0; i < mapsizeH; i++)
        {
            for (int j = 0; j < mapsizeW; j++)
            {
                

                int data = liveMap.datas[i][j];

                if (i ==0 || i == mapsizeH - 1)
                {
                    data = BlockNumber.broken;
                }
                else if(j == 0 || j == mapsizeW-1)
                {
                    data = BlockNumber.broken;
                }

                int style_num = i * mapsizeW + j;
                Block newBlock = blockFactory.CreateBlock(data, liveMap.styles[style_num], new Vector2(j, i));
                //Debug.Log(j + "," + i + " : " + newBlock.data);
                //newBlock.transform.SetParent(groundParent);

                liveMap.SetBlocks(j, i, newBlock);
                if (newBlock.data == BlockNumber.normal || newBlock.data == BlockNumber.upperNormal)
                {
                    liveMap.UpdateCheckArray(j, i, false);
                }
                else if(newBlock.data >= BlockNumber.parfaitA && newBlock.data <= BlockNumber.parfaitD)
                {
                    liveMap.UpdateCheckArray(j, i, false);
                }
                else if (newBlock.data >= BlockNumber.upperParfaitA && newBlock.data <= BlockNumber.upperParfaitD)
                {
                    liveMap.UpdateCheckArray(j, i, false);
                }
                else if(newBlock.data == BlockNumber.character || newBlock.data == BlockNumber.upperCharacter)
                {
                    liveMap.UpdateCheckArray(j, i, false);
                }
                else
                {
                    liveMap.UpdateCheckArray(j, i, true);
                }
            }
        }
    }

//스테이지 모드
    public Map GenerateMap(int index)//index == gameManager.nowLevel
    {
        
        int len = 0;
        //int islandNum = csvManager.islandData.Island_Num(index);
        //liveMap = csvManager.islands[islandNum].maps[index];
        
        if(index <= csvManager.islandData.tutorial)
        {
            liveMap = csvManager.islands[0].maps[index];
        }
        else if(index <= csvManager.islandData.icecream)
        {
            len = csvManager.islandData.tutorial + 1;
            liveMap = csvManager.islands[1].maps[index-len];
        }
        else if(index <= csvManager.islandData.beach)
        {
            len = csvManager.islandData.icecream + 1;
            liveMap = csvManager.islands[2].maps[index-len];
        }
        else if(index <= csvManager.islandData.cracker)
        {
            len = csvManager.islandData.beach + 1;
            liveMap = csvManager.islands[3].maps[index-len];
        }
        else if(index <= csvManager.islandData.cottoncandy)
        {
            len = csvManager.islandData.cracker + 1;
            liveMap = csvManager.islands[4].maps[index-len];
        }

        liveMap.gameObject.SetActive(true);
        
        MakeMap(liveMap.mapsizeH, liveMap.mapsizeW, liveMap.parfait);

        return liveMap;

        
    }
    //에디터 모드
    public Map EditorMap()//유저가 만든 맵
    {
        liveMap = editorMap;
        liveMap.gameObject.SetActive(true);

        MakeMap(liveMap.mapsizeH, liveMap.mapsizeW, liveMap.parfait);
        return liveMap;
    }

    public Map InfiniteMap()
    {
        return liveMap;
    }
    //에디터 프레이모드
    public Map CustomPlayMap()//다른 유저가 만든 
    {
        //editorMap.Initialize(GameManager.instance.playCustomData.itemdata);
        liveMap = GameManager.instance.customMap;
        liveMap.gameObject.SetActive(true);

        MakeMap(liveMap.mapsizeH, liveMap.mapsizeW, liveMap.parfait);
        return liveMap;
    }
    


   

}




