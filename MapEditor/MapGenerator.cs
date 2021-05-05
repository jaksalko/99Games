using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using System.Linq;

using UnityEngine.SceneManagement;


public class MapGenerator : MonoBehaviour
{
    public Button completeButton;
    public Image eraseButtonImage;
    [Header("Prefabs")]
    public Indexer blockPositionPrefab;

    [Header("POPUP")]
    public CheckListPopup checkListPopup;

    [Header("Camera")]
    public Camera cam;
    public Slider fovSlider;
    public Vector2 fovMinMax;
    public float cameraHeight;
    Vector3 center;
    float angle;
    float distance;


    [Header("Holder")]
    public Holder[] holders;
    public Holder positionHolder;//0
    public Holder firstFloorHolder;//1
    public Holder secondFloorHolder;//2
    public Holder thirdFloorHolder;//3

    [Header("Condition")]
    public int character_count;
    public int parfait_count;
    Vector3 positionA;
    Vector3 positionB;

    [Header("MapGenerating")]
    public Map newMap;
    public Indexer[,] indexer;
    public Vector2 maxSize;
    RaycastHit hit;

    Button selectedButton;
    int blockNumber = 999;
    int styleNumber = 999;
    int temp_blockNumber;
    int temp_styleNumber;
    public List<int> placeableFloor;


    
    public Text warning;

    public GameObject gameResource;//Simulator object
    public GameObject generatorResource;//MapGenerator Object

    public RectTransform bottomUI;
    

    bool editMode = false;
    BlockFactory blockFactory;

    Indexer selected_indexer;
    public Button eraseButton;
    public Button moveButton;
    public Button rotateButton;

    public Button[] characterButtons; //0 1
    public Button[] parfaitButtons; // 0 1 2 3

    public GameObject editPopup;
    public GameObject backPopup;


    public Material make_skybox;
    public Material play_skybox;

    public bool activeClick = true;
    public bool moveBlock = false;
    private void Awake()
    {
        //maxSize = GameManager.instance.maxSize; //고정되었습니다
        BlockPositionEditor();
        blockFactory = BlockFactory.instance;
        
        checkListPopup.SetCheckList(character_count, parfait_count);
        
        //Camera Setting
        /*
        
        center = new Vector3(maxSize.x / 2 -0.5f , 0f, maxSize.y / 2 - 0.5f);
        angle = -90f;
        distance = maxSize.x;
        TopView();
        */


#if UNITY_EDITOR
        this.UpdateAsObservable()
            .Where(_ => !editMode)
               .Where(_ => Input.GetMouseButton(0))
               .Select(ray => cam.ScreenPointToRay(Input.mousePosition))
               .Subscribe(ray => MakeBlock(ray));

        this.UpdateAsObservable()
            .Where(_ => activeClick)
            .Where(_ => editMode)
            
               .Where(_ => Input.GetMouseButtonUp(0))
               .Select(mouse => Input.mousePosition)
               .Subscribe(mouse => SelectBlock(mouse));

        
        this.UpdateAsObservable()
            .Where(_ => !activeClick)
            .Where(_ => editMode)
            .Where(_ => moveBlock)
              .Where(_ => Input.GetMouseButtonDown(0))
               .Select(ray => cam.ScreenPointToRay(Input.mousePosition))
               .Subscribe(ray => MakeBlock(ray));



        /*
        this.UpdateAsObservable()
           .Where(_ => editMode)
              .Where(_ => Input.GetMouseButtonDown(0))
              .Select(ray => cam.ScreenPointToRay(Input.mousePosition))
              .Subscribe(ray => EraseBlock(ray));
        */
#else

        this.UpdateAsObservable()
                .Where(_ => !editMode)
                .Where(_ => Input.touchCount > 0)
                .Where(_ => Input.GetTouch(0).phase == TouchPhase.Moved)
               .Select(ray => cam.ScreenPointToRay(Input.GetTouch(0).position))
               .Subscribe(ray => MakeBlock(ray));

        this.UpdateAsObservable()
                .Where(_ => activeClick)
                .Where(_ => editMode)
                .Where(_ => Input.touchCount > 0)
                .Where(_ => Input.GetTouch(0).phase == TouchPhase.Ended)
              .Select(mouse => Input.GetTouch(0).position)
              .Subscribe(mouse => SelectBlock(mouse));

        this.UpdateAsObservable()
                .Where(_ => !activeClick)
                .Where(_ => editMode)
                .Where(_ => moveBlock)
                .Where(_ => Input.touchCount > 0)
                    .Where(_ => Input.GetTouch(0).phase == TouchPhase.Began)
                    .Select(ray => cam.ScreenPointToRay(Input.GetTouch(0).position))
                    .Subscribe(ray => MakeBlock(ray));
#endif


    }

    private void Start()
    {
        //SoundManager.instance.ChangeBGM(SceneManager.GetActiveScene().buildIndex);//섬 7-11
    }
    public void BackPopupClicked()
    {
        backPopup.SetActive(true);
    }
    public void Back_OKButton()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void Back_NoButton()
    {
        backPopup.SetActive(false);
    }


    
    public void Exitbutton()
    {
        editPopup.SetActive(false);
        StartCoroutine(ActiveClickTimer());
    }
    public void ChangeMode()
    {
        editMode = editMode ? false : true;
        if(editMode)
        {
            eraseButtonImage.sprite = Resources.Load<Sprite>("Editor/Frame/eraser_on");
            bottomUI.anchoredPosition += Vector2.down * 470f;
            characterButtons[0].gameObject.SetActive(false);
            characterButtons[1].gameObject.SetActive(false);
        }
        else
        {
            eraseButtonImage.sprite = Resources.Load<Sprite>("Editor/Frame/eraser_off");
            //bottomUI.position += Vector3.up * 470f;
            bottomUI.anchoredPosition += Vector2.up * 470f;
            characterButtons[0].gameObject.SetActive(true);
            characterButtons[1].gameObject.SetActive(true);
        }
    }

    void MakeBlock(Ray ray)
    {
        if (Physics.Raycast(ray, out hit, 1000))
        {
            if (hit.transform.CompareTag("Indexer"))
            {

                Indexer indexer = hit.transform.GetComponent<Indexer>();

                if (indexer.placeable)
                {

                    int blockNumber_modification = blockNumber;
                    //blocknumber 조정 : cracker , parfait , cloud , obstacle , character
                    if (indexer.Floor == 1)
                    {
                        if (blockNumber >= BlockNumber.cloudUp && blockNumber <= BlockNumber.cracker_2)
                        {
                            blockNumber_modification += 10;
                        }
                    }
                    else if (indexer.Floor == 2)
                    {
                        blockNumber_modification += 10;

                    }
                    Debug.Log("make block number : " + blockNumber_modification);
                    Block newBlock = blockFactory.EditorCreateBlock(blockNumber_modification, styleNumber, new Vector3(indexer.X, indexer.Z));
                    indexer.AddBlock(newBlock);
                    indexer.CheckPlaceableIndex(block_floor: placeableFloor);
                    newBlock.transform.SetParent(holders[indexer.Floor].transform);

                    if (blockNumber >= BlockNumber.parfaitA && blockNumber <= BlockNumber.parfaitD)
                    {
                        parfait_count++;
                        selectedButton.interactable = false;
                        blockNumber = 999;//더 이상 배치할 수 없음
                    }
                    if (blockNumber == BlockNumber.character)
                    {
                        character_count++;
                        selectedButton.interactable = false;
                        blockNumber = 999;//더 이상 배치할 수 없음

                        if (styleNumber == 0)
                            positionA = new Vector3(indexer.X + 1, indexer.Floor, indexer.Z + 1);
                        else
                            positionB = new Vector3(indexer.X + 1, indexer.Floor, indexer.Z + 1);
                    }

                    if(moveBlock)//move block
                    {
                        moveBlock = false;

                        blockNumber = temp_blockNumber;
                        styleNumber = temp_styleNumber;

                        Update_Placeable_Floor(blockNumber);
                        StartCoroutine(ActiveClickTimer());
                    }


                    if (CheckCondition().Item1 && CheckCondition().Item2)
                    {
                        completeButton.interactable = true;
                    }
                    else
                    {
                        completeButton.interactable = false;
                    }
                    checkListPopup.SetCheckList(character_count, parfait_count);


                }

            }
        }


    }

    void SelectBlock(Vector3 mousePosition)//수정 상태일때 블럭 클릭.
    {
        
        Ray ray = cam.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out hit, 1000))
        {
            
            if (hit.transform.CompareTag("Indexer"))
            {

                selected_indexer = hit.transform.GetComponent<Indexer>();
                //indexer에 위치한 최상단 블럭을 선택해서 UI 표시
                //UI에서 지우개 , 위치변경 , 회전을 선택
                
                if(selected_indexer.blocks.Count != 0 && !editPopup.activeSelf)
                {
                    activeClick = false;
                    if ((selected_indexer.data >= BlockNumber.cloudUp && selected_indexer.data <= BlockNumber.cloudLeft) ||
                        (selected_indexer.data >= BlockNumber.upperCloudUp && selected_indexer.data <= BlockNumber.upperCloudLeft) ||
                        (selected_indexer.data >= BlockNumber.slopeUp && selected_indexer.data <= BlockNumber.slopeLeft))
                    {
                        rotateButton.interactable = true;
                    }
                    else
                    {
                        rotateButton.interactable = false;
                    }
                    Vector3 index_position = cam.WorldToScreenPoint(selected_indexer.transform.position);
                    editPopup.transform.position = new Vector3(index_position.x,index_position.y + 50, 0);
                    editPopup.SetActive(true);
                }

                
                
                

                //지우개 : 삭제
                //위치변경 : 위치할 수 있는 인덱스와 할 수 없는 인덱스 표시 후 유저가 클릭할 경우 해당 위치로 옮김
                //회전 : 수정 UI가 꺼지지 않고, 오른쪽으로 회전함.



            }
        }
    }


    public void EraseBlock()
    {
        int selected_indexer_data = selected_indexer.data;
        if (selected_indexer_data == BlockNumber.character || selected_indexer_data == BlockNumber.upperCharacter)
        {
            character_count--;
            
           
            int style = selected_indexer.blocks[selected_indexer.blocks.Count - 1].style;
            characterButtons[style].interactable = true;
            
        }
        else if((selected_indexer_data >= BlockNumber.parfaitA && selected_indexer_data <= BlockNumber.parfaitD)
            ||(selected_indexer_data >= BlockNumber.upperParfaitA && selected_indexer_data <= BlockNumber.upperParfaitD))
        {
            parfait_count--;

            int parfait_num = selected_indexer_data % 10 - 1;
            parfaitButtons[parfait_num].interactable = true;
        }

        selected_indexer.EraseBlock();
        selected_indexer.CheckPlaceableIndex(block_floor: placeableFloor);
        editPopup.SetActive(false);
        StartCoroutine(ActiveClickTimer());

        

        if (CheckCondition().Item1 && CheckCondition().Item2)
        {
            completeButton.interactable = true;
        }
        else
        {
            completeButton.interactable = false;
        }
        checkListPopup.SetCheckList(character_count, parfait_count);

    }
    void Update_Placeable_Floor(int block_num)
    {
        List<int> placeableList = new List<int>();

        if (BlockNumber.first_floor.Contains(block_num))
        {
            placeableList.Add(1);

            if (block_num != BlockNumber.normal)
                placeableList.Add(2);
        }
        else if (BlockNumber.second_floor.Contains(block_num))
        {
            placeableList.Add(2);

            if (block_num == BlockNumber.character || block_num == BlockNumber.obstacle || (block_num >= BlockNumber.parfaitA && block_num <= BlockNumber.parfaitD))
            {
                placeableList.Add(3);
            }
        }
        else if (BlockNumber.third_floor.Contains(block_num))
        {
            placeableList.Add(3);
        }

        placeableFloor = placeableList;

        for (int i = 0; i < indexer.GetLength(0); i++)
        {
            for (int j = 0; j < indexer.GetLength(1); j++)
            {
                indexer[i, j].CheckPlaceableIndex(placeableFloor);
            }
        }

    }
    public void MoveBlock()
    {
        (int, int) block_data = selected_indexer.MoveBlock();

        temp_blockNumber = blockNumber;
        temp_styleNumber = styleNumber;

        blockNumber = block_data.Item1;
        styleNumber = block_data.Item2;

        Update_Placeable_Floor(blockNumber);
        

        editPopup.SetActive(false);
        moveBlock = true;
        //StartCoroutine(ActiveClickTimer());


        if (CheckCondition().Item1 && CheckCondition().Item2)
        {
            completeButton.interactable = true;
        }
        else
        {
            completeButton.interactable = false;
        }
        checkListPopup.SetCheckList(character_count, parfait_count);

    }
    public void RotateBlock()
    {
        selected_indexer.RotateBlock();
        editPopup.SetActive(false);
        StartCoroutine(ActiveClickTimer());

    }

    IEnumerator ActiveClickTimer()
    {
        
        yield return new WaitForSeconds(1f);


        activeClick = true;

        yield break;
    }

    //Reset Button
    public void ResetMapEditorButtonClicked()
    {
        BlockPositionEditor();
        parfait_count = 0;

        firstFloorHolder.ClearHolder();
        secondFloorHolder.ClearHolder();
        thirdFloorHolder.ClearHolder();

       
    }


    

    (bool,bool) CheckCondition()
    {
        bool check_character;
        bool check_parfait;

        if(character_count == 2)
        {
            check_character = true;
        }
        else
        {
            check_character = false;
        }

        if(parfait_count == 0 || parfait_count == 4)
        {
            check_parfait = true;
        }
        else
        {
            check_parfait = false;
        }

        return (check_character, check_parfait);
    }

   

#region Editor
    public void BlockPositionEditor()
    {
        positionHolder.ClearHolder();
        indexer = new Indexer[(int)maxSize.y,(int)maxSize.x];



        for(int i = 0; i < maxSize.y; i++)//세로
        {
            for(int j = 0; j < maxSize.x; j++)//가로
            {
                
                Indexer newPositionBlock = Instantiate(blockPositionPrefab, new Vector3(j, 0, i), blockPositionPrefab.transform.rotation);
                newPositionBlock.Initialize(j, i);
                newPositionBlock.name = "Quad(" + j + "," + i + ")";
                indexer[i,j] = newPositionBlock;
                positionHolder.SetParent(newPositionBlock.gameObject);
            }
        }

        
    }
#endregion


#region CAMERA Button Function
    public void FovControl()
    {
        float fov = Mathf.Lerp(fovMinMax.y, fovMinMax.x, fovSlider.value);
        cam.fieldOfView = fov;
    }
    public void TopView()
    {
        cam.transform.position = center + Vector3.up*cameraHeight;
        cam.transform.LookAt(center);
    }
    public void SideView()//default front view
    {
        angle = -90f;
        float x = center.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = center.z + distance * Mathf.Sin(angle * Mathf.Deg2Rad);

        cam.transform.position = new Vector3(x, cameraHeight, z);
        cam.transform.LookAt(center);
    }

    public void RightSlide()
    {
        angle += 90;
        float x = center.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = center.z + distance * Mathf.Sin(angle * Mathf.Deg2Rad);

        cam.transform.position = new Vector3(x, cameraHeight, z);
        cam.transform.LookAt(center);

    }
    public void LeftSlide()
    {
        angle -= 90;
        float x = center.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = center.z + distance * Mathf.Sin(angle * Mathf.Deg2Rad);

        cam.transform.position = new Vector3(x, cameraHeight, z);
        cam.transform.LookAt(center);
    }
#endregion
    

    //Block Select Button Function
    public void SelectBlockButtonClicked(EditorBlock block)
    {
        //editMode = false;


        blockNumber = block.blockNumber;
        styleNumber = block.styleNumber;

        placeableFloor = block.placeableFloor();


        for (int i = 0; i < indexer.GetLength(0); i++)
        {
            for (int j = 0; j < indexer.GetLength(1); j++)
            {
                indexer[i, j].CheckPlaceableIndex(placeableFloor);
            }
        }

        selectedButton = block.GetComponent<Button>();//한개만 생성가능한 블럭을 비활성화 하기 위함.
  
    }

    public void CheckListPanelClicked()
    {
        checkListPopup.gameObject.SetActive(true);
    }

    void GetEditorMap()
    {
        int width_min = (int)maxSize.x - 1;
        int height_min = (int)maxSize.y - 1;

        int width_max = 0;
        int height_max = 0;

        int indexer_count = 0;

        List<List<int>> datas = new List<List<int>>();

        List<List<int>> styles = new List<List<int>>();

        bool isParfait = parfait_count == 4 ? true : false;
        
        

        for (int i = 0; i < maxSize.y; i++)//세로
        {
            for (int j = 0; j < maxSize.x; j++)//가료
            {
                if(indexer[i,j].blocks.Count !=0)
                {
                    if (width_min > j) width_min = j;
                    if (height_min > i) height_min = i;

                    if (width_max < j) width_max = j;
                    if (height_max < i) height_max = i;
                }

                indexer_count++;
            }
        }
        maxSize.y = height_max - height_min + 1;
        maxSize.x = width_max - width_min + 1;

        int height = (int)maxSize.y + 2;
        int width = (int)maxSize.x + 2;

      
        //initialize data and style list
        for (int i = 0; i < height; i++)
        {
            List<int> data_line = new List<int>();
            for(int j = 0; j < width; j++)
            {
                data_line.Add(BlockNumber.broken);

                List<int> style_line = new List<int>() { 0, 0, 0 };
                styles.Add(style_line);
            }
            datas.Add(data_line);
        }

        //get data and style from indexer
        for(int i = 1; i < height-1; i++)
        {
            for(int j = 1; j < width-1; j++)
            {
                int style_count = i * width + j;
                int indexer_x = height_min + i -1;
                int indexer_z = width_min + j-1;

                (int, List<int>) indexItem = indexer[indexer_x, indexer_z].GetLastBlockData();
                datas[i][j] = indexItem.Item1;
                styles[style_count] = indexItem.Item2;
                for(int k = 0; k < styles[style_count].Count; k++)
                {
                    Debug.Log(styles[style_count][k]);
                }
                if(datas[i][j] == BlockNumber.character)
                {
                    if (styles[style_count][1] == 0)
                        positionA = new Vector3(j, 1, i);
                    else
                        positionB = new Vector3(j, 1, i);
                }
                else if (datas[i][j] == BlockNumber.upperCharacter)
                {
                    if (styles[style_count][2] == 0)
                        positionA = new Vector3(j, 2, i);
                    else
                        positionB = new Vector3(j, 2, i);
                }


                
            }
        }

        


        newMap.Initialize(new Vector2(height, width), isParfait, positionA, positionB, datas, styles, new List<int>() {100,200,300});
        Debug.Log("width : " + width_min + "," + width_max + "  height : " + height_min + "," + height_max);
        Debug.Log("width length : " + width + " height length : " + height);
    }
    //Simulating Button Function
    public void StartSimulatorButtonClicked()
    {
        RenderSettings.skybox = play_skybox;


        GetEditorMap();

        generatorResource.SetActive(false);
        gameResource.SetActive(true);

        //newMap.Initialize();
        /*
        if(parfait_count == 4)
        {
            newMap.parfait = true;
        }


        newMap.mapsizeH = (int)maxSize.y + 2;//외벽
        newMap.mapsizeW = (int)maxSize.x + 2;


        for (int i = 0; i < newMap.mapsizeH; i++)
        {
            Map.Line line = new Map.Line();
            for (int j = 0; j < newMap.mapsizeW; j++)
            {
                line.line.Add(BlockNumber.obstacle);
            }
            newMap.lines.Add(line);
        }
            
        
        for(int i = 0; i < indexer.Count; i++)
        {
            
            newMap.lines[indexer[i].Z+1].line[indexer[i].X+1] = indexer[i].data;
            if(indexer[i].data == BlockNumber.character)
            {
                Debug.Log("character index : " + indexer[i].Z + 1 + " , " + indexer[i].X + 1);
            }

        }

        //newMap.parfait = false;

        //newMap.LineToMap();
        generatorResource.SetActive(false);
        gameResource.SetActive(true);

        */
    }


}
