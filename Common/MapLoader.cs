using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;




public class MapLoader : MonoBehaviour
{


    //public int[,] map; // sampleMap에 있음
    //public bool[,] check; // 생성하며 구현
    BlockFactory blockFactory;
    int style;

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

        /*
        if (parfait)
            MakeParfait(mapsizeH, mapsizeW);
        */
        
       
        
    }

   
    
    void MakeGround(int mapsizeH, int mapsizeW)
    {
        blockFactory = BlockFactory.instance;


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
        /*
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

                    if (liveMap.startPositionA.z == i && liveMap.startPositionA.x == j && liveMap.startUpstairA)
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
                    }
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
        */
    


    
//스테이지 모드
    public Map GenerateMap(int index)//index == gameManager.nowLevel
    {
        
        int len = 0;
        if(index <= csvManager.islandData.tutorial)
        {
            Debug.Log(csvManager);
            Debug.Log(csvManager.islands[0].maps[index]);
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
            Debug.Log("cotton :" + (index - len));
            liveMap = csvManager.islands[4].maps[index-len];
        }

        liveMap.gameObject.SetActive(true);

        //sample[index].init();
        //liveMap = sample[index];

        MakeMap(liveMap.mapsizeH, liveMap.mapsizeW, liveMap.parfait);

        return liveMap;

        //return MakeMap(liveMap.mapsizeH, liveMap.mapsizeW, liveMap.parfait);
    }
    //에디터 모드
    public Map EditorMap()//내가 먼든 먑
    {
        liveMap = editorMap;
        liveMap.gameObject.SetActive(true);

        MakeMap(liveMap.mapsizeH, liveMap.mapsizeW, liveMap.parfait);
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
            EditorMap[] datas = JsonHelper.FromJson<EditorMap>(fixdata);

            EditorMap selectedData = datas[Random.Range(0, datas.Length)];

            //editorMap.Initialize(selectedData);
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




