using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

public class MapGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] blockPrefab;// 0 normal 1 obstacle 2 second 3 slope 4 5 char 6 cracked 7 cloud
    public GameObject[] parfaitPrefab;//0 cup 1 ice 2 syrup 3 cherry
    public Indexer blockPositionPrefab;

    [Header("Camera")]
    Camera cam;
    public Slider fovSlider;
    public Vector2 fovMinMax;
    public float cameraHeight;
    Vector3 center;
    float angle;
    float distance;


    [Header("Holder")]
    public Holder positionHolder;
    public Holder firstFloorHolder;
    public Holder secondFloorHolder;
    public Holder thirdFloorHolder;
    


    [Header("MapGenerating")]
    [SerializeField] GameObject selectedPrefab;
    public Map newMap;
    public List<Indexer> indexer;
    public Vector2 maxSize;
    RaycastHit hit;
    int selected_id;
    int slope_id;
    int cloud_id;
    int parfait_id;
    public Text warning;

    public GameObject gameResource;
    public GameObject generatorResource;
    bool erase = false;

    private void Awake()
    {
        maxSize = GameManager.instance.maxSize;
        BlockPositionEditor();
        cam = Camera.main;
        center = new Vector3(maxSize.x / 2 -0.5f , 0f, maxSize.y / 2 - 0.5f);
        angle = -90f;
        distance = maxSize.x;

       
        
#if UNITY_EDITOR
        this.UpdateAsObservable()
            .Where(_ => !erase)
               .Where(_ => Input.GetMouseButton(0))
               .Select(ray => cam.ScreenPointToRay(Input.mousePosition))
               .Subscribe(ray => MakeBlock(ray));


        this.UpdateAsObservable()
           .Where(_ => erase)
              .Where(_ => Input.GetMouseButtonDown(0))
              .Select(ray => cam.ScreenPointToRay(Input.mousePosition))
              .Subscribe(ray => EraseBlock(ray));
#else

        this.UpdateAsObservable()
                .Where(_ => !erase)
                .Where(_ => Input.touchCount > 0)
                .Where(_ => Input.GetTouch(0).phase == TouchPhase.Moved)
               .Select(ray => cam.ScreenPointToRay(Input.GetTouch(0).position))
               .Subscribe(ray => MakeBlock(ray));

        this.UpdateAsObservable()
               .Where(_ => erase)
               .Where(_ => Input.touchCount > 0)
               .Where(_ => Input.GetTouch(0).phase == TouchPhase.Began)
              .Select(ray => cam.ScreenPointToRay(Input.GetTouch(0).position))
              .Subscribe(ray => EraseBlock(ray));
#endif
        TopView();
       
    }

    //Reset Button
    public void ResetMapEditorButtonClicked()
    {
        BlockPositionEditor();
        parfait_id = 0;

        firstFloorHolder.ClearHolder();
        secondFloorHolder.ClearHolder();
        thirdFloorHolder.ClearHolder();

        

        blockPrefab[BlockNumber.characterA].SetActive(false);//character
        blockPrefab[BlockNumber.characterA].transform.position = default;


        blockPrefab[BlockNumber.characterB].SetActive(false);//character
        blockPrefab[BlockNumber.characterB].transform.position = default;


       
    }


    void EraseBlock(Ray ray)
    {
        if (Physics.Raycast(ray, out hit, 1000))
        {
            if (hit.transform.CompareTag("Indexer"))
            {
                Indexer indexer = hit.transform.GetComponent<Indexer>();
                if(indexer.Floor != 0)
                {
                    switch(indexer.Floor)
                    {
                        case 1://first floor
                            for(int i = 0; i < firstFloorHolder.transform.childCount; i++)
                            {
                                Transform firstFloorObject = firstFloorHolder.transform.GetChild(i);
                                if(firstFloorObject.localPosition.x == indexer.X && firstFloorObject.localPosition.z == indexer.Z)
                                {
                                    Destroy(firstFloorObject.gameObject);
                                    indexer.Floor = 0;
                                    indexer.data = BlockNumber.broken;//obstacle
                                    indexer.isFull = false;
                                    break;
                                }
                            }
                            
                            break;
                        case 2://second floor
                            for (int i = 0; i < secondFloorHolder.transform.childCount; i++)
                            {
                                Transform secondFloorObject = secondFloorHolder.transform.GetChild(i);
                                if (secondFloorObject.localPosition.x == indexer.X && secondFloorObject.localPosition.z == indexer.Z)
                                {
                                    if(indexer.data >= BlockNumber.parfaitA && indexer.data <= BlockNumber.parfaitD)
                                    {
                                        Debug.LogWarning("Parfait erase");
                                    }
                                    Destroy(secondFloorObject.gameObject);
                                    indexer.Floor = 1;
                                    indexer.data = BlockNumber.normal;
                                    indexer.isFull = false;
                                    break;
                                }
                            }
                           
                            break;
                        case 3://third floor
                            for (int i = 0; i < thirdFloorHolder.transform.childCount; i++)
                            {
                                Transform thirdFloorObject = thirdFloorHolder.transform.GetChild(i);
                                if (thirdFloorObject.localPosition.x == indexer.X && thirdFloorObject.localPosition.z == indexer.Z)
                                {
                                    if (indexer.data >= BlockNumber.upperParfaitA && indexer.data <= BlockNumber.upperParfaitD)
                                    {
                                        Debug.LogWarning("Parfait erase");
                                    }
                                    Destroy(thirdFloorObject.gameObject);
                                    indexer.Floor = 2;
                                    indexer.data = BlockNumber.upperNormal;
                                    indexer.isFull = false;
                                    break;
                                }
                            }
                           
                            break;

                    }
                    if (indexer.data == BlockNumber.character || indexer.data == BlockNumber.upperCharacter)//character erase
                    {
                        if(blockPrefab[BlockNumber.characterA].activeSelf && blockPrefab[BlockNumber.characterA].transform.position.x == indexer.X && blockPrefab[BlockNumber.characterA].transform.position.z == indexer.Z)
                        {
                            //char1 erase
                            blockPrefab[BlockNumber.characterA].SetActive(false);
                            blockPrefab[BlockNumber.characterA].transform.position = default;
                            indexer.Floor--;
                           
                        }
                        if (blockPrefab[BlockNumber.characterB].activeSelf && blockPrefab[BlockNumber.characterB].transform.position.x == indexer.X && blockPrefab[BlockNumber.characterB].transform.position.z == indexer.Z)
                        {
                            //char2 erase
                            blockPrefab[BlockNumber.characterB].SetActive(false);
                            blockPrefab[BlockNumber.characterB].transform.position = default;
                            indexer.Floor--;
                        }

                        if (indexer.Floor == 1)
                            indexer.data = BlockNumber.normal;
                        else if (indexer.Floor == 2)
                            indexer.data = BlockNumber.upperNormal;

                        indexer.isFull = false;
                    }
                }
            }
        }
    }


    void MakeBlock(Ray ray)
    {
        if(Physics.Raycast(ray,out hit , 1000))
        {
            if(hit.transform.CompareTag("Indexer"))
            {
                if (selectedPrefab == null)
                    warning.text = "블럭을 선택해주세요";
                else
                {
                    warning.text = "";
                    Indexer indexer = hit.transform.GetComponent<Indexer>();
                    if (indexer.Floor == 0 && selected_id < 10)
                        //아무것도 깔려있지 않은 상태에서는 노말블럭 또는 깨지는 블럭 또는 구름블럭
                    {

                        if(selected_id == BlockNumber.cloudUp)
                        {
                            GameObject selected = Instantiate(selectedPrefab, new Vector3(indexer.X, 0, indexer.Z), Quaternion.Euler(new Vector3(0,90 * cloud_id, 0)));
                            firstFloorHolder.SetParent(selected);
                            indexer.data = BlockNumber.cloudUp + cloud_id;
                        }
                        else//Cracker,Normal
                        {
                            GameObject selected = Instantiate(selectedPrefab, new Vector3(indexer.X, 0, indexer.Z), selectedPrefab.transform.rotation);
                            firstFloorHolder.SetParent(selected);
                            indexer.data = selected_id;
                        }
                        
                        
                        indexer.Floor = 1;

                        if (selected_id != BlockNumber.normal)//2층 블럭이 아니면 더이상 위에 무엇을 올릴 수가 없으므로 
                            indexer.isFull = true;

                    }
                    else if (indexer.Floor == 1 && !indexer.isFull && selected_id > 0 && selected_id < 30)//바닥이 깔려있는 상태에서 선택 블럭이 노말블럭이 아니며
                    {
                        if (selected_id == BlockNumber.characterA || selected_id == BlockNumber.characterB)//if character setting
                        {
                            if (!selectedPrefab.activeSelf)
                            {
                                if (selected_id == BlockNumber.characterA)
                                {
                                    newMap.startUpstairA = false;
                                    newMap.startPositionA = new Vector3(indexer.X + 1, 0, indexer.Z + 1);
                                }
                                else if (selected_id == BlockNumber.characterB)
                                {
                                    newMap.startUpstairB = false;
                                    newMap.startPositionB = new Vector3(indexer.X + 1, 0, indexer.Z + 1);
                                }
                                
                                selectedPrefab.transform.position = new Vector3(indexer.X, 0.5f, indexer.Z);
                                selectedPrefab.SetActive(true);
                                indexer.data = BlockNumber.character;//블럭 넘버 1층 캐릭터로 바꿔야
                                Debug.Log(indexer.data +" : "+ indexer.Z + "," + indexer.X);
                            }
                            else
                            {
                                warning.text = "이미 존재합니다";
                                return;
                            }
                             //Character positioning 수정해야함



                        }
                        else if (selected_id == BlockNumber.slopeUp)
                        {
                            GameObject selected = Instantiate(selectedPrefab, new Vector3(indexer.X, 0, indexer.Z), Quaternion.Euler(new Vector3(0, 90 * slope_id, 0)));
                            secondFloorHolder.SetParent(selected);
                            indexer.data = BlockNumber.slopeUp + slope_id;

                        }
                        else if (selected_id == BlockNumber.cloudUp)
                        {
                            GameObject selected = Instantiate(selectedPrefab, new Vector3(indexer.X, 0, indexer.Z), Quaternion.Euler(new Vector3(0, 90 * cloud_id, 0)));
                            secondFloorHolder.SetParent(selected);
                            indexer.data = BlockNumber.upperCloudUp + cloud_id;
                        }
                        else if (selected_id == BlockNumber.parfaitA)
                        {
                            if(parfait_id < 4)
                            {
                                GameObject selected = Instantiate(parfaitPrefab[parfait_id], new Vector3(indexer.X, 0, indexer.Z), Quaternion.identity);
                                secondFloorHolder.SetParent(selected);
                                indexer.data = BlockNumber.parfaitA + parfait_id;
                                parfait_id++;
                            }
                            else
                            {
                                warning.text = "모두 배치했습니다";
                                return;
                            }
                           
                        }
                        else// 1 2  : 장애물 , 2층 , 2층 부서지는 블럭
                        {
                            GameObject selected = Instantiate(selectedPrefab, new Vector3(indexer.X, 0, indexer.Z), selectedPrefab.transform.rotation);
                            secondFloorHolder.SetParent(selected);

                            //indexer.data = selected_id;
                            if(selected_id == BlockNumber.cracker_0)
                            {
                                Debug.Log("uppercracker");
                                indexer.data = BlockNumber.upperCracker_0;
                            }
                            else
                            {
                                indexer.data = selected_id;
                            }
                           

                        }
                      
                        indexer.Floor = 2;

                        if (selected_id != BlockNumber.upperNormal)//2층 블럭이 아니면 더이상 위에 무엇을 올릴 수가 없으므로 
                            indexer.isFull = true;
                        
                    }
                    else if (indexer.Floor == 2 && !indexer.isFull && selected_id >= 21)
                    {
                        if (selected_id == BlockNumber.characterA || selected_id == BlockNumber.characterB)//character setting
                        {
                            if (!selectedPrefab.activeSelf)
                            {
                                if (selected_id == BlockNumber.characterA)
                                {
                                    newMap.startUpstairA = true;
                                    newMap.startPositionA = new Vector3(indexer.X+1, 1, indexer.Z+1);
                                }
                                else if (selected_id == BlockNumber.characterB)
                                {
                                    newMap.startUpstairB = true;
                                    newMap.startPositionB = new Vector3(indexer.X+1, 1, indexer.Z+1);
                                }
                               
                                selectedPrefab.transform.position = new Vector3(indexer.X, 1.5f, indexer.Z);
                                selectedPrefab.SetActive(true);
                                //thirdFloorHolder.SetParent(selectedPrefab);


                                indexer.data = BlockNumber.upperCharacter;//2층 캐릭터
                                Debug.Log(indexer.data + " : " + indexer.Z + "," + indexer.X);
                            }
                            else
                            {
                                warning.text = "이미 존재합니다";
                                return;
                            }



                        }
                        else if (selected_id == BlockNumber.parfaitA)
                        {
                            if (parfait_id < 4)
                            {
                                GameObject selected = Instantiate(parfaitPrefab[parfait_id], new Vector3(indexer.X, 0, indexer.Z), Quaternion.identity);
                                thirdFloorHolder.SetParent(selected);
                                indexer.data = BlockNumber.upperParfaitA + parfait_id;
                                parfait_id++;
                            }
                            else
                            {
                                warning.text = "모두 배치했습니다";
                                return;
                            }

                        }
                        else
                        {
                            GameObject selected = Instantiate(selectedPrefab, new Vector3(indexer.X, 0, indexer.Z), selectedPrefab.transform.rotation);
                            thirdFloorHolder.SetParent(selected);
                            indexer.data = BlockNumber.upperObstacle;
                        }
                       
                        indexer.Floor = 3;
                        indexer.isFull = true;
                    }
                   
                }
               
                
                
                
            }
        }
        
    }

#region Editor
    public void BlockPositionEditor()
    {
        positionHolder.ClearHolder();
        indexer.Clear();
        for(int i = 0; i < maxSize.x; i++)//가로
        {
            for(int j = 0; j < maxSize.y; j++)//세로
            {
                Indexer newPositionBlock = Instantiate(blockPositionPrefab, new Vector3(i, 0, j), blockPositionPrefab.transform.rotation);
                newPositionBlock.SetXZ(i, j);
                newPositionBlock.name = "Quad(" + i + "," + j + ")";
                indexer.Add(newPositionBlock);
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
    public void SelectBlockButtonClicked(int id)
    {
        erase = false;
        selected_id = id;
        selectedPrefab = blockPrefab[selected_id];
        
    }
    public void SelectParfaitButtonClicked(int id)
    {
        erase = false;
        selected_id = BlockNumber.parfaitA;
        //parfait_id = parfaitOrder;
        //selectedPrefab = blockPrefab[selected_id + parfait_id];
    }
    public void SlopeButtonClicked(int id)
    {
        erase = false;
        selected_id = BlockNumber.slopeUp;
        slope_id = id;
        selectedPrefab = blockPrefab[selected_id];
    }
    public void CloudButtonClicked(int id)
    {
        erase = false;
        selected_id = BlockNumber.cloudUp;
        cloud_id = id;// 0 up 1 right 2 down 3 left
        selectedPrefab = blockPrefab[selected_id];
    }
    public void EraseButtonClicked()
    {
        erase = true;

    }
    //Simulating Button Function
    public void StartSimulatorButtonClicked()
    {

        //실핼할수 없는 조건 넣기
        //캐릭터 미배치 // 파르페 일부 배치
        if(parfait_id != 0 && parfait_id < 4)
        {
            warning.text = "파르페를 모두 배치하거나 모두 삭제해주세요";
            return;
        }
        else if(!blockPrefab[BlockNumber.characterA].activeSelf || !blockPrefab[BlockNumber.characterB].activeSelf)
        {
            warning.text = "캐릭터를 모두 배치해주세요";
            return;
        }

        if(parfait_id == 4)
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
    }

   
}
