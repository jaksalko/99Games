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

    void SetMapProperty()
    {
        _mapSize = new Vector2Int(Random.Range(mapSizeMinMax.x,mapSizeMinMax.y+1),Random.Range(mapSizeMinMax.x,mapSizeMinMax.y+1));
        _obstaclePercent = Random.Range(obstaclePercentMinMax.x, obstaclePercentMinMax.y);

        mainCamera.transform.position = new Vector3((float)(_mapSize.x-1)/2,20,(float)(_mapSize.y-1)/2);
    }

    void MakeNewMap()
    {
        //Parfait Exist?
        bool isParfait = Random.Range(0,2) == 0 ? true : false;
        //Player Data
        PlayerProperty propertyA = new PlayerProperty(default,0,State.Idle,false,false);
        PlayerProperty propertyB = new PlayerProperty(default,0,State.Idle,false,false);
        
        
        
        //Map Data
        List<List<int>> datas = new List<List<int>>(_mapSize.x);
        List<List<int>> styles = new List<List<int>>();
        List<int> star_limit = new List<int>();

        List<Tuple<int, int>> exceptOutlineIndex = new List<Tuple<int, int>>();
        
        for (int i = 0; i < _mapSize.x; i++)
        {
            List<int> horizontalData = new List<int>();
            for (int j = 0; j < _mapSize.y; j++)
            {
                if (i == 0 || j == 0 || i == _mapSize.x - 1 || j == _mapSize.y - 1)
                    horizontalData.Add(BlockNumber.broken);
                else
                {
                    horizontalData.Add(BlockNumber.normal);
                    exceptOutlineIndex.Add(new Tuple<int, int>(i,j));
                }
                   
            }
            datas.Add(horizontalData);
        }

        int playerCount = 0;
        while (playerCount != 2)
        {
            var playerIndex = Random.Range(0, exceptOutlineIndex.Count);
            var randomIndexHeight = exceptOutlineIndex[playerIndex].Item1;
            var randomIndexWidth =  exceptOutlineIndex[playerIndex].Item2;


            if (datas[randomIndexHeight][randomIndexWidth] == BlockNumber.normal)
            {
                if(playerCount == 0)
                    propertyA.Position = new Vector3Int(randomIndexWidth, 1, randomIndexHeight);
                else
                    propertyB.Position = new Vector3Int(randomIndexWidth, 1, randomIndexHeight);
            }
            else
            {
                if(playerCount == 0)
                    propertyA.Position = new Vector3Int(randomIndexWidth, 2, randomIndexHeight);
                else
                    propertyB.Position = new Vector3Int(randomIndexWidth, 2, randomIndexHeight);
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
                
                datas[randomIndexHeight][randomIndexWidth] = parfaitBlockNumber;
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
            datas[randomIndexHeight][randomIndexWidth] = randomObstacleBlockNumber;
            
        }
        
        #endregion

        
        
       
        for (int h = 0; h < _mapSize.x; h++)
        {
            for (int w = 0; w < _mapSize.y; w++)
            {
                blockFactory.CreateBlock(datas[h][w],new List<int>(){0,0,0}, new Vector2(h, w));
            }
        }
        
        




        infiniteMap.Initialize(_mapSize,isParfait,propertyA.Position,propertyB.Position,datas,styles,star_limit);
    }

    bool ValidateMap() // BFS 경로탐색
    {
        
        return true;
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
            MakeMap();
        }
        
    }
    

}
