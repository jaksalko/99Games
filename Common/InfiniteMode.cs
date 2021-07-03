using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Random = UnityEngine.Random;


public class InfiniteMode : MonoBehaviour
{
    [SerializeField] private Map infiniteMap;
    [SerializeField] private BlockFactory blockFactory;
    [SerializeField] private Camera mainCamera;
    
    [Tooltip("recommended 5-17")][SerializeField] Vector2Int mapSizeMinMax;
    [Tooltip("recommended 0-0.7")][SerializeField] private Vector2 obstaclePercentMinMax;
    
    
    private Vector2Int _mapSize;
    private float _obstaclePercent;

    private PlayerProperty _propertyA;
    private PlayerProperty _propertyB;
        
        
    //Map Data
    private List<List<int>> _datas;
    private List<List<bool>> _check;
    private int _parfaitOrder = 0;

    void SetMapProperty()
    {
        _mapSize = new Vector2Int(Random.Range(mapSizeMinMax.x,mapSizeMinMax.y+1),Random.Range(mapSizeMinMax.x,mapSizeMinMax.y+1));
        _obstaclePercent = Random.Range(obstaclePercentMinMax.x, obstaclePercentMinMax.y);

        mainCamera.transform.position = new Vector3((float)(_mapSize.y-1)/2,25,(float)(_mapSize.x-1)/2);
    }

    void MakeNewMap()
    {
        //Parfait Exist?
        bool isParfait = Random.Range(0,2) == 0 ? true : false;
        //Player Data
        
        _propertyA = new PlayerProperty(default,0,State.Idle,false,false,0);
        _propertyB = new PlayerProperty(default,0,State.Idle,false,false,0);
        
        
        
        //Map Data
        _datas = new List<List<int>>(_mapSize.x);
        _check = new List<List<bool>>(_mapSize.x);
        List<List<int>> styles = new List<List<int>>();
        List<int> star_limit = new List<int>();

        List<Tuple<int, int>> exceptOutlineIndex = new List<Tuple<int, int>>();
        
        for (int i = 0; i < _mapSize.x; i++)
        {
            List<int> horizontalData = new List<int>();
            List<bool> checkData = new List<bool>();
            for (int j = 0; j < _mapSize.y; j++)
            {
                if (i == 0 || j == 0 || i == _mapSize.x - 1 || j == _mapSize.y - 1)
                {
                    horizontalData.Add(BlockNumber.broken);
                    checkData.Add(true);
                }
                    
                else
                {
                    horizontalData.Add(BlockNumber.normal);
                    exceptOutlineIndex.Add(new Tuple<int, int>(i,j));
                    checkData.Add(false);
                }
                   
            }
            _datas.Add(horizontalData);
            _check.Add(checkData);
        }

        int playerCount = 0;
        while (playerCount != 2)
        {
            var playerIndex = Random.Range(0, exceptOutlineIndex.Count);
            var randomIndexHeight = exceptOutlineIndex[playerIndex].Item1;
            var randomIndexWidth =  exceptOutlineIndex[playerIndex].Item2;


            if (_datas[randomIndexHeight][randomIndexWidth] == BlockNumber.normal)
            {
                if (playerCount == 0)
                {
                    _propertyA.Position = new Vector3Int(randomIndexWidth, 1, randomIndexHeight);
                    _propertyA.Floor = 1;
                }
                else
                {
                    _propertyB.Position = new Vector3Int(randomIndexWidth, 1, randomIndexHeight);
                    _propertyB.Floor = 1;
                }
                
                _propertyA.BlockTemp = BlockNumber.normal;
                _datas[randomIndexHeight][randomIndexWidth] = BlockNumber.character;
                    
            }
            else
            {
                if (playerCount == 0)
                {
                    _propertyA.Position = new Vector3Int(randomIndexWidth, 2, randomIndexHeight);
                    _propertyA.Floor = 2;
                }
                else
                {
                    _propertyB.Position = new Vector3Int(randomIndexWidth, 2, randomIndexHeight);
                    _propertyB.Floor = 2;
                }
                _propertyB.BlockTemp = BlockNumber.upperNormal;    
                _datas[randomIndexHeight][randomIndexWidth] = BlockNumber.upperCharacter;

                
            }
            
            exceptOutlineIndex.RemoveAt(playerIndex);
            playerCount++;
        }
       
        
        #region parfaitBlock arrange
        
        
        if (isParfait)
        {
            int parfaitCount = 0;
            
            while (parfaitCount != 4)
            {
               
                var randomIndexHeight = Random.Range(1, _mapSize.x-1);
                var randomIndexWidth =  Random.Range(1, _mapSize.y-1);
                Tuple<int, int> indexTuple = new Tuple<int, int>(randomIndexHeight, randomIndexWidth);
                
                if(!exceptOutlineIndex.Contains(indexTuple)) continue;
                exceptOutlineIndex.Remove(indexTuple);

                var parfaitBlockNumber = Random.Range(0, 2) == 0
                    ? BlockNumber.parfaitA + parfaitCount
                    : BlockNumber.upperParfaitA + parfaitCount;
                
                _datas[randomIndexHeight][randomIndexWidth] = parfaitBlockNumber;
                _check[randomIndexHeight][randomIndexWidth] = true;
                parfaitCount++;
            }
        }
        
        #endregion


        #region obstacleBlock arrange
        
        var normalMaxCount = (_mapSize.x - 2) * (_mapSize.y - 2) * (1 - _obstaclePercent);
        var obstacleBlockLength = BlockNumber.allObstacleBlocks.Length;

       
        while (normalMaxCount < exceptOutlineIndex.Count)
        {

            var randomIndexHeight = Random.Range(1, _mapSize.x-1);
            var randomIndexWidth =  Random.Range(1, _mapSize.y-1);

            Tuple<int, int> indexTuple = new Tuple<int, int>(randomIndexHeight, randomIndexWidth);
            
            if(!exceptOutlineIndex.Contains(indexTuple)) continue;

            exceptOutlineIndex.Remove(indexTuple);
            var randomObstacleBlockNumber = BlockNumber.allObstacleBlocks[Random.Range(0,obstacleBlockLength)];
            _datas[randomIndexHeight][randomIndexWidth] = randomObstacleBlockNumber;
            
        }
        
        #endregion

        
        
       
        for (int h = 0; h < _mapSize.x; h++)
        {
            string s = "";
            for (int w = 0; w < _mapSize.y; w++)
            {
                s += _datas[h][w] + " ";
                blockFactory.CreateBlock(_datas[h][w],new List<int>{0,0,0}, new Vector2(w, h));
            }
            Debug.Log(s);
        }

        infiniteMap.Initialize(_mapSize,isParfait,_propertyA.Position,_propertyB.Position,_datas,styles,star_limit);
    }

    bool ValidateMap() // BFS 경로탐색
    {
        Queue<Validation> validations= new Queue<Validation>();
        Validation validation = new Validation(0,_propertyA, _propertyB, _datas,_check);

        validations.Enqueue(validation);
        int validationCount = 0;
        while (validations.Count != 0)
        {
            validationCount++;
            if (validationCount == 1234)
            {
                Debug.Log("can't validate");
                return false;
                
            }
            Validation v = validations.Dequeue();

            for (int i = 0; i < 8; i++)
            {
                int dir = i % 4;
                PlayerProperty movePlayer = i < 4 ? v.PropertyA : v.PropertyB;
                PlayerProperty otherPlayer = i < 4 ? v.PropertyB : v.PropertyA;

                var result = Move(0, dir, ref movePlayer, ref otherPlayer, v.Datas.ToList(), v.Check.ToList());
                if (result.Item1 != 0)
                {
                    if (CheckEndGame(result.Item3))
                        return true;
                    
                    Debug.Log("Move " + dir +" : ");
                    int step = v.Step + 1;
                    //움직인 이후 둘 중에 하나라도 포지션이 달라졌다면
                    validations.Enqueue(new Validation(step,movePlayer,otherPlayer,result.Item2.ToList(),result.Item3.ToList()));
                }
                
                
            }
            
           
        }

       
        
        return false;
        //한번 움직일 때 마다 변경되는 data list,PlayerProperty 는 공유되어서는 안된다.
       
    }

    (int,List<List<int>>,List<List<bool>>) Move(int moveCount ,int direction,ref PlayerProperty movePlayer ,ref PlayerProperty otherPlayer , List<List<int>> datas , List<List<bool>> check)
    {
        int[,] step = {{0,1},{1,0},{0,-1},{-1,0} };
        var userDirection = movePlayer.Direction;

        var floor = movePlayer.Floor;
        var posX = movePlayer.Position.x;
        var posZ = movePlayer.Position.z;

        Debug.Log((posZ + step[direction, 1]) + "," + (posX + step[direction, 0]));
        var nextBlockData = datas[posZ + step[direction, 1]][posX + step[direction, 0]];
        var nextBlockOrder = nextBlockData % 10 - 1;
        
        if(CheckNextBlock(GetThroughBlockList(floor,direction,movePlayer.Cloud),nextBlockData))
        {
            var nextnextBlockData = datas[posZ + step[direction, 1]*2][posX + step[direction, 0]*2];

            
            int nextFloor = (floor == 1) ? 2 : 1;
            if (nextBlockData >= BlockNumber.slopeUp && nextBlockData <= BlockNumber.slopeLeft 
                && !CheckNextBlock(GetThroughBlockList(nextFloor, movePlayer.Direction, movePlayer.Cloud), nextnextBlockData) 
                && !CheckNextBlock(GetStopBlockList(nextFloor, movePlayer.Direction, movePlayer.Cloud), nextnextBlockData))
            {
                Debug.Log("can't use slope");
            }
            else
            {
                moveCount++;
                //character move
                movePlayer.Position += new Vector3Int(step[direction, 0],0,step[direction, 1]);
                check[posZ + step[direction, 1]][posX + step[direction, 0]] = true;
                
                if (BlockNumber.cloudBlocks.Contains(nextBlockData))
                {
                    movePlayer.Position += Vector3Int.up * BlockNumber.GetFloorGap(floor, nextBlockData);
                    movePlayer.Cloud = true;

                    if (movePlayer.Direction != nextBlockOrder)
                    {
                        movePlayer.Direction = nextBlockOrder;
                    }
                }
                else if(BlockNumber.parfaitBlocks.Contains(nextBlockData))
                {
                    if (nextBlockOrder == GameController.ParfaitOrder)
                        GameController.ParfaitOrder++;
                }
                else if (BlockNumber.slopeBlocks.Contains(nextBlockData))
                {
                    movePlayer.Floor = nextFloor;
                }

                if (!CheckEndGame(check))
                {
                    if (otherPlayer.Lock)
                    {
                        otherPlayer.Lock = false;
                        otherPlayer.Direction = otherPlayer.BlockTemp % 10 - 1;
                        Swap(ref otherPlayer);
                        Move(moveCount,otherPlayer.Direction, ref otherPlayer, ref movePlayer, datas, check);
                    }

                    Move(moveCount, direction, ref movePlayer, ref otherPlayer, datas, check);
                    //Move(moveCount);
                }
                else
                {
                    Debug.Log("End Game");
                }
                
                
                /*check is all clear
                if not clear --> recursive
                if clear --> end*/
                
            }
        }
        else if (CheckNextBlock(GetStopBlockList(floor, direction, movePlayer.Cloud), nextBlockData))
        {
            moveCount++;
            movePlayer.Cloud = false;
            movePlayer.Lock = false;

            var floorGap = BlockNumber.GetFloorGap(floor, nextBlockData);
            movePlayer.Position += new Vector3Int(step[direction, 0],0,step[direction, 1]);
            movePlayer.Floor += floorGap;
            check[posZ + step[direction, 1]][posX + step[direction, 0]] = true;

            
            if (otherPlayer.Lock)
            {
                otherPlayer.Lock = false;
                otherPlayer.Direction = otherPlayer.BlockTemp % 10 - 1;
                Swap(ref otherPlayer);
                Move(moveCount,otherPlayer.Direction, ref otherPlayer, ref movePlayer, datas, check);
            }

        }
        else
        {
            if ((movePlayer.Floor == 1 && nextBlockData == BlockNumber.character)
                || (movePlayer.Floor == 2 && nextBlockData == BlockNumber.upperCharacter))
            {
                if (movePlayer.Cloud) movePlayer.Lock = true;
            }
            else if (movePlayer.State == State.Master && movePlayer.Floor == 1 &&
                     nextBlockData >= BlockNumber.upperNormal && nextBlockData < BlockNumber.upperObstacle)
            {
                Swap(ref otherPlayer);
                Move(moveCount, movePlayer.Direction, ref otherPlayer, ref movePlayer, datas, check);
            }
        }
        
        
        Swap(ref movePlayer);
        
        
        
        
        return (moveCount,datas,check);
    }

    bool CheckEndGame(List<List<bool>> check)
    {
        foreach (var line in check)
        {
            foreach (var data in line)
            {
                if (!data) return false;
            }
           
            
        }
        return true;

        
    }
    void Swap(ref PlayerProperty pp)
    {
        int blockData = _datas[pp.Position.z][pp.Position.x];
        _datas[pp.Position.z][pp.Position.x] = pp.BlockTemp;
        pp.BlockTemp = blockData;
    }

    public void MakeMap()
    {
        blockFactory.DestroyBlock();
        SetMapProperty();
        MakeNewMap();
        
        
        if (ValidateMap())
        {
            Debug.Log("is validated");
        }
        else
        {
            Debug.Log("failed");
            //MakeMap();
        }
        
    }
    
    
    List<int> GetThroughBlockList(int floor, int getDirection, bool onCloud)
    {
        switch (floor)
        {
            case 1:
                return BlockNumber.GetDownstairThroughBlock(_parfaitOrder,getDirection, onCloud);
            case 2:
                return BlockNumber.GetUpstairThroughBlock(_parfaitOrder,getDirection, onCloud);
            case 3:
                return BlockNumber.GetThirdFloorThroughBlock(getDirection);
                

        }

        return new List<int>();
    }
    List<int> GetStopBlockList(int floor, int getDirection, bool onCloud)
    {
        switch (floor)
        {
            case 1:
                return BlockNumber.GetDownstairStopBlock(_parfaitOrder,onCloud);
            case 2:
                return BlockNumber.GetUpstairStopBlock(_parfaitOrder,onCloud);
            case 3:
                return BlockNumber.GetThirdFloorStopBlock(_parfaitOrder);

        }

        return new List<int>();
    }

    bool CheckNextBlock(List<int> checkList, int data)
    {
        for (int i = 0; i < checkList.Count; i++)
        {
            if (checkList[i] == data)
                return true;
        }
        return false;
    }

}
