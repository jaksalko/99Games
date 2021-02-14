using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public static class IslandData
{
    public const int
        tutorial = 4,   // 0 - 4
        iceCream = 16,  // 5-6
        beach = 26,     // 7-8
        cracker = 37,   //9-11
        cottoncandy = 47;//37
    
    public const string
        stage1 = "Tutorial_Island",
        stage2 = "Icecream_Island",
        stage3 = "Beach_Island",
        stage4 = "Cracker_Island",
        stage5 = "Cottoncandy_Island";

    public const int lastLevel = 47;//13
    public static int[] island_last = {tutorial,iceCream,beach,cracker,cottoncandy};

    public static int Island_Num(int stage)
	{
        for(int i = 0 ; i < island_last.Length ; i++)
        {
            if(stage <= island_last[i])
            {
                Debug.Log("island num : " + island_last[i]);
                return i;
            }
        }

        return -1;
        
    }
}


public class MapLoader : MonoBehaviour
{


    //public int[,] map; // sampleMap에 있음
    //public bool[,] check; // 생성하며 구현
    

    [Header("Block Type")]
    public GroundBlock groundBlock;
  
    public GroundBlock groundBlock_second;
    public ObstacleBlock obstacleBlock;
    public ParfaitBlock[] parfaitBlock;
    public SlopeBlock slopeBlock;
    public CloudBlock cloudBlock;
    public CrackedBlock crackedBlock;
   

    [Header("Block Storage")]
    public Transform groundParent;
    public Transform obstacleParent;

    public Map editorMap;//made by editor scene

    public List<Map> sample;//island map datas

    //public Vector3 parfaitEndPoint;
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
        
        if (parfait)
            MakeParfait(mapsizeH, mapsizeW);

        
       
        
    }

    Block MakeBlock(Block.Type type ,int blockData , Vector3 pos_ , bool visible , bool isCheck)
    {
        Block newBlock = null;

        switch(type)
        {
            case Block.Type.Outline:
                GroundBlock outline = Instantiate(groundBlock , pos_ , groundBlock.transform.rotation);
                outline.gameObject.SetActive(visible);                
                outline.Init(blockData);
                outline.transform.parent = obstacleParent;
                newBlock = outline;

                break;

            case Block.Type.Ground:
                GroundBlock ground = Instantiate(groundBlock, pos_, groundBlock.transform.rotation);
                ground.gameObject.SetActive(visible);                
                ground.Init(blockData);
                ground.IsNormal(ground.Snow);//active snow object
                ground.transform.parent = groundParent;

                newBlock = ground;
                break;

            case Block.Type.SecondGround:
                GroundBlock second = Instantiate(groundBlock_second, pos_, groundBlock.transform.rotation);
                second.gameObject.SetActive(visible);
                second.Init(blockData);
                second.IsNormal(second.Snow);//active snow object
                second.transform.parent = groundParent;

                newBlock = second;
                break;

            case Block.Type.Obstacle:
                ObstacleBlock obstacle = Instantiate(obstacleBlock,obstacleBlock.transform.position +  pos_, obstacleBlock.transform.rotation);
                obstacle.gameObject.SetActive(visible);
                obstacle.Init(blockData);
                obstacle.transform.parent = obstacleParent;

                newBlock = obstacle;
                break;

            case Block.Type.Slope:
                SlopeBlock slope = Instantiate(slopeBlock, pos_, Quaternion.Euler(new Vector3(0, 90 * (blockData - BlockNumber.slopeUp), 0)));
                slope.gameObject.SetActive(visible);
                slope.Init(blockData);
                slope.transform.parent = groundParent;

                newBlock = slope;
                break;

            case Block.Type.Parfait:
                parfaitBlock[0].gameObject.SetActive(true);
                parfaitBlock[0].gameObject.transform.position = pos_;

                parfaitBlock[0].sequence = 0;
                parfaitBlock[0].Activate();

                newBlock = parfaitBlock[0];
                break;

            case Block.Type.Cloud:
                CloudBlock cloud = Instantiate(cloudBlock, pos_, Quaternion.Euler(new Vector3(0, 90*(blockData%10), 90)));
                cloud.gameObject.SetActive(visible);
                cloud.Init(blockData);
                cloud.transform.parent = groundParent;

                newBlock = cloud;
                break;

            case Block.Type.Cracked:
                CrackedBlock cracked = Instantiate(crackedBlock, pos_, Quaternion.identity);
                cracked.gameObject.SetActive(visible);
                cracked.Init(blockData);
                
                cracked.transform.parent = groundParent;

                newBlock = cracked;
                break;

            case Block.Type.broken:
                CrackedBlock broken = Instantiate(crackedBlock, pos_, Quaternion.identity);
                broken.gameObject.SetActive(visible);
                broken.Init(blockData);
                broken.transform.parent = obstacleParent;

                newBlock = broken;
                break;

        }
        liveMap.UpdateCheckArray((int)pos_.x, (int)pos_.z, isCheck);

        return newBlock;
    }
    
    void SetOutlineBlock(int mapsizeH, int mapsizeW)
    {
        
        for (int i = 0; i < mapsizeW; i++)
        {
            if (i == 0 || i == (mapsizeW - 1))//outline setting
            {
                for (int j = 0; j < mapsizeH; j++)
                {
                    liveMap.SetBlocks(i, j, MakeBlock(Block.Type.Outline,BlockNumber.obstacle, new Vector3(i, -0.5f, j), visible: false, isCheck: true));            
                }
            }
            else
            {
                liveMap.SetBlocks(i, 0, MakeBlock(Block.Type.Outline, BlockNumber.obstacle, new Vector3(i, -0.5f, 0), visible: false, isCheck: true));
                liveMap.SetBlocks(i, mapsizeH - 1, MakeBlock(Block.Type.Outline, BlockNumber.obstacle, new Vector3(i, -0.5f, mapsizeH -1), visible: false, isCheck: true));         
            }
        }


    }
    
    void MakeGround(int mapsizeH, int mapsizeW)
    {
        
        SetOutlineBlock(mapsizeH, mapsizeW);
        
        for (int i = 1; i < mapsizeH-1; i++)//z
        {
            for (int j = 1; j < mapsizeW-1; j++)//x
            {
                //instantiate ground object at all of area . parent is spawnGround object

                int blockData = liveMap.lines[i].line[j];
                //0층 빌드
                if(blockData == 0 || blockData > BlockNumber.broken)// all floor block except cloudBlock , crackedBlock , broken
                {
                    
                    liveMap.SetBlocks(j,i, MakeBlock(Block.Type.Ground, blockData, new Vector3(j, -0.5f, i), visible: true, isCheck: false));
             
                }
                else//cloudBlock , crackedBlock or broken (0 floor)
                {

                    if(blockData >= BlockNumber.cracker_0 && blockData <= BlockNumber.cracker_2)
                    {
                        liveMap.SetBlocks(j, i, MakeBlock(Block.Type.Cracked, blockData, new Vector3(j, -0.5f, i), visible: true, isCheck: true));
                    }
                    else if(blockData == BlockNumber.broken)
                    {
                        liveMap.SetBlocks(j, i, MakeBlock(Block.Type.broken, blockData, new Vector3(j, -0.5f, i), visible: true, isCheck: true));                       
                    }
                    else // cloud Block
                    {
                        liveMap.SetBlocks(j, i, MakeBlock(Block.Type.Cloud, blockData, new Vector3(j, -0.5f, i), visible: true, isCheck: true));
                    }
                    
                }
               
                
                
                //1층,2층 빌드
                if (blockData == BlockNumber.obstacle)
                {
                    //generate obstacle
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.Obstacle, blockData, new Vector3(j, 0.8f, i), visible: true, isCheck: true));
                    
                }
                else if (blockData == BlockNumber.upperObstacle)//second floor obstacle
                {
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.SecondGround, blockData, new Vector3(j, 0.5f, i), visible: true, isCheck: true));
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.Obstacle, blockData, new Vector3(j, 1.8f, i), visible: true, isCheck: true));                   
                }
              
                else if(blockData == BlockNumber.upperNormal)//second floor
                {
                    //generate second floor
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.SecondGround, blockData, new Vector3(j, 0.5f, i), visible: true, isCheck: false));                   
                }
                else if (blockData == BlockNumber.upperCharacter)//second floor with character?
                {
                    //check => true because player start here...? // Data => upperNormal? upperCharacter?
                    //character???
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.SecondGround, BlockNumber.upperCharacter, new Vector3(j, 0.5f, i), visible: true, isCheck: false));

                    /*if (liveMap.startPositionA.z == i && liveMap.startPositionA.x == j && liveMap.startUpstairA)
                    {
                        Debug.Log("char A : " + i + "," + j);
                        GroundBlock second_ground = Instantiate(groundBlock_second, new Vector3(j, 0.5f, i), groundBlock_second.transform.rotation);
                        second_ground.transform.parent = groundParent;
                    }
                    if (liveMap.startPositionB.z == i && liveMap.startPositionB.x == j && liveMap.startUpstairB)
                    {
                        Debug.Log("char B : " + i + "," + j);
                        GroundBlock second_ground = Instantiate(groundBlock_second, new Vector3(j, 0.5f, i), groundBlock_second.transform.rotation);
                        second_ground.transform.parent = groundParent;
                    }*/
                }
                else if(blockData >= BlockNumber.slopeUp && blockData <= BlockNumber.slopeLeft)
                {
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.Slope, blockData, new Vector3(j, 0.5f, i), visible: true, isCheck: true));
                    
                }
                else if(blockData >= BlockNumber.upperCracker_0 && blockData <= BlockNumber.upperCracker_2)
                {
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.Cracked, blockData, new Vector3(j, 0.5f, i), visible: true, isCheck: true));
                    
                }
                else if (blockData == BlockNumber.upperBroken)
                {
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.broken, blockData, new Vector3(j, 0.5f, i), visible: true, isCheck: true));
                    
                }
                else if (blockData >= BlockNumber.upperCloudUp && blockData <= BlockNumber.upperCloudLeft)
                {
                    liveMap.SetBlocks(j, i, MakeBlock(Block.Type.Cloud, blockData, new Vector3(j, 0.5f, i), visible: true, isCheck: true));
                    
                }
               
            }
        }
    }


    void MakeParfait(int mapsizeH, int mapsizeW)
    {
        int parfait_sequence = 0;

        for (int i = 0; i < mapsizeH; i++)
        {
            for (int j = 0; j < mapsizeW; j++)
            {
                int blockData = liveMap.lines[i].line[j];
                int sequence = (blockData % 10) - 1;
                if (blockData >= BlockNumber.parfaitA && blockData <= BlockNumber.parfaitD)
                {
                    liveMap.SetBlocks(j,i, parfaitBlock[sequence]);
                    parfaitBlock[sequence].gameObject.SetActive(true);
                    parfaitBlock[sequence].gameObject.transform.position = new Vector3(j, 0.5f, i);
                    
                    parfaitBlock[sequence].Init(blockData);
                    
                    parfait_sequence++;
                 
                }
                else if(blockData >= BlockNumber.upperParfaitA && blockData <= BlockNumber.upperParfaitD)
                {
                    MakeBlock(Block.Type.SecondGround, blockData, new Vector3(j, 0.5f, i), visible: true, isCheck: false);

                    liveMap.SetBlocks(j, i, parfaitBlock[sequence]);
                    parfaitBlock[sequence].gameObject.SetActive(true);
                    parfaitBlock[sequence].gameObject.transform.position = new Vector3(j, 1.5f, i);

                    parfaitBlock[sequence].Init(blockData);

                    
                    parfait_sequence++;
                   
                }
                    
                    
                
                



            }
        }
    }

    public Map GenerateMap(int index)//index == gameManager.nowLevel
    {
        
        int len = 0;
        if(index <= IslandData.tutorial)
        {
            Debug.Log(csvManager);
            Debug.Log(csvManager.islands[0].maps[index]);
            liveMap = csvManager.islands[0].maps[index];
            liveMap.init();


        }
        else if(index <= IslandData.iceCream)
        {
            len = IslandData.tutorial + 1;
            liveMap = csvManager.islands[1].maps[index-len];
        }
        else if(index <= IslandData.beach)
        {
            len = IslandData.iceCream + 1;
            liveMap = csvManager.islands[2].maps[index-len];
        }
        else if(index <= IslandData.cracker)
        {
            len = IslandData.beach + 1;
            liveMap = csvManager.islands[3].maps[index-len];
        }
        else if(index <= IslandData.cottoncandy)
        {
            len = IslandData.cracker + 1;
            Debug.Log("cotton :" + (index - len));
            liveMap = csvManager.islands[4].maps[index-len];
        }
        liveMap.init();
        liveMap.gameObject.SetActive(true);

        //sample[index].init();
        //liveMap = sample[index];

        MakeMap(liveMap.mapsizeH, liveMap.mapsizeW, liveMap.parfait);

        return liveMap;

        //return MakeMap(liveMap.mapsizeH, liveMap.mapsizeW, liveMap.parfait);
    }
    public Map EditorMap()
    {
        liveMap = editorMap;
        liveMap.gameObject.SetActive(true);

        MakeMap(editorMap.mapsizeH, editorMap.mapsizeW, editorMap.parfait);
        return editorMap;
    }
    public Map CustomPlayMap()
    {
        editorMap.Initialize(GameManager.instance.playCustomData.itemdata);
        liveMap = editorMap;
        liveMap.gameObject.SetActive(true);

        MakeMap(editorMap.mapsizeH, editorMap.mapsizeW, editorMap.parfait);
        return editorMap;
    }
    public IEnumerator InfiniteMAP(int level , System.Action<Map> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(PrivateData.ec2 + "map/difficulty?difficulty=" +level+"&nickname="+GameManager.instance.id);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            callback(null);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;

            //Get data and convert to samplemap list..

            
            string fixdata = JsonHelper.fixJson(www.downloadHandler.text);
            JsonData[] datas = JsonHelper.FromJson<JsonData>(fixdata);
           
            JsonData selectedData = datas[Random.Range(0, datas.Length)];

            editorMap.Initialize(selectedData);
            Debug.Log(editorMap.mapsizeH + "," + editorMap.mapsizeW + "," + editorMap.startPositionA);
            liveMap = editorMap;
            callback(liveMap);
            //selectedData.DataToString();//Debug.Log

            liveMap.gameObject.SetActive(true);

            MakeMap(liveMap.mapsizeH , liveMap.mapsizeW , liveMap.parfait);
           
        }

        yield break;
    }



   

}




