using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



[Serializable]
public class Map : MonoBehaviour
{
    public string map_title;
    public int total_snow;

    int[,] step = new int[4, 2] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
    public int mapsizeH;
    public int mapsizeW;
    public List<int> star_limit;

    public bool parfait = false;
    public Vector3 startPositionA;//    y축 -9 : 1 층 , -8 : 2층 
    public Vector3 startPositionB;



    public List<List<int>> datas;
    public List<List<int>> styles;
    public bool[,] check;

    Block[,] blocks;

    public List<Vector2> snowList;
    public List<Block> stepped_blockList;
    
    public void Initialize(Vector2Int size, bool isParfait, Vector3 posA, Vector3 posB, List<List<int>> datas, List<List<int>> styles, List<int> star_limit)
    {
        mapsizeH = size.x;
        mapsizeW = size.y;
        parfait = isParfait;
        startPositionA = posA;
        startPositionB = posB;


        this.star_limit = star_limit;

        this.datas = datas;
        this.styles = styles;
        if(map_title == null)
            map_title = PlayerPrefs.GetString("nickname","pingpengboong") + " " + DateTime.Now.ToString("yyyyMMddHHmmss");
        
        blocks = new Block[mapsizeH, mapsizeW];//로더의 MakeGround에서 채워짐
        check = new bool[mapsizeH, mapsizeW];//마찬가지

        snowList = new List<Vector2>();
        stepped_blockList = new List<Block>();
    }

    bool isEndGame()
    {
        for (int i = 0; i < mapsizeH; i++)
        {
            for (int j = 0; j < mapsizeW; j++)
            {
                if (!check[i, j])
                {
//                    Debug.Log("is false : " + i + "," + j);
                    return false;
                }
            }
        }
        Debug.Log("end game");
        return true;
    }

    List<int> GetThroughBlockList(int floor, int getDirection, bool onCloud)
    {
        switch (floor)
        {
            case 1:
                return BlockNumber.GetDownstairThroughBlock(GameController.ParfaitOrder,getDirection, onCloud);
            case 2:
                return BlockNumber.GetUpstairThroughBlock(GameController.ParfaitOrder,getDirection, onCloud);
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
                return BlockNumber.GetDownstairStopBlock(GameController.ParfaitOrder,onCloud);
            case 2:
                return BlockNumber.GetUpstairStopBlock(GameController.ParfaitOrder,onCloud);
            case 3:
                return BlockNumber.GetThirdFloorStopBlock(GameController.ParfaitOrder);

        }

        return new List<int>();
    }

    bool CheckNextBlock(List<int> checkList, int data)
    {
        foreach (var check in checkList)
        {
            if (check == data)
                return true;
        }
        
        return false;
    }

    

   

    
    public void GetDestination(List<Unit_Movement> movements,Player player, Vector3 from, Vector3 to)
    {

       
        
        int direction = player.getDirection;
       
        int floor = (int)from.y;
        int posX = (int)from.x;
        int posZ = (int)from.z;

        if (from == player.transform.position)//시작부분
        {
            
            if (blocks[posZ,posX].type == Block.Type.Cracker)
            {
               
                stepped_blockList.Add(blocks[posZ, posX]);
            }
        }



        int next = GetBlockData( posX + step[direction, 1],  posZ+ step[direction, 0]);
        int nextnext = GetBlockData( posX + step[direction, 1] * 2,  posZ + step[direction, 0] * 2);
        int nextBlockOrder = next % 10 - 1; // 파르페 순서 또는 솜사탕 방향

        if (CheckNextBlock(GetThroughBlockList(floor, direction, player.onCloud), next))//다음은 지나갈 수 있는 블럭
        {
            int nextFloor = (floor == 1) ? 2 : 1;
            if (next >= BlockNumber.slopeUp && next <= BlockNumber.slopeLeft
                && !CheckNextBlock(GetThroughBlockList(nextFloor, player.getDirection, player.onCloud), nextnext) && !CheckNextBlock(GetStopBlockList(nextFloor, player.getDirection, player.onCloud), nextnext))
            {
                Debug.Log("can't use slope");
            }
            else
            {
                to.x += step[direction, 1];
                to.z += step[direction, 0];

                if(BlockNumber.cloudBlocks.Contains(next))
                {
                    to.y += BlockNumber.GetFloorGap(floor, next);

                    if (player.getDirection != nextBlockOrder)
                    {
                        player.getDirection = nextBlockOrder;
                        movements.Add(new Unit_Movement(player, nextBlockOrder, from, to));
                        from = to;
                    }

                    player.onCloud = true;
                }
                else if(BlockNumber.parfaitBlocks.Contains(next))
                {
                    if (nextBlockOrder == GameController.ParfaitOrder)
                        GameController.ParfaitOrder++;
                }
                else if (BlockNumber.slopeBlocks.Contains(next))
                {
                    to.y = nextFloor;

                }

                player.isLock = false;
                UpdateCheckTrue(width: (int)to.x, height: (int)to.z);

                if (!isEndGame())
                {
                    Player other = player.other;
                    if (other.isLock)
                    {
                        other.isLock = false;
                        other.getDirection = other.temp % 10 - 1;
                        blocks[(int)other.transform.position.z, (int)other.transform.position.x].data = other.temp;
                        GetDestination(movements, other, other.transform.position, other.transform.position);
                        //other.Move(this, other.temp % 10 - 1);
                    }


                    GetDestination(movements, player, from,to);
                    return;

                }
            }
          
           
            


        }
        else if (CheckNextBlock(GetStopBlockList(floor, direction, player.onCloud), next))//다음은 멈춰야하는 블럭
        {
            player.onCloud = false; // stop 이면 무조건 oncloud 에서 벗어남.
            player.isLock = false;

            int floorGap = BlockNumber.GetFloorGap(floor, next);

            to.x += step[direction, 1];
            to.z += step[direction, 0];
            to.y += floorGap;

            if (floorGap < 0) player.actionnum = 5;
            else player.actionnum = 3;

            if (BlockNumber.parfaitBlocks.Contains(next))
            {

                

                if (nextBlockOrder == GameController.ParfaitOrder)
                    GameController.ParfaitOrder++;

                if (BlockNumber.second_floor.Contains(next))
                    blocks[(int)to.z, (int)to.x].data = BlockNumber.normal;
                else
                    blocks[(int)to.z, (int)to.x].data = BlockNumber.upperNormal;
            }
            else if(next == BlockNumber.character)
            {
                player.actionnum = 2;//ride : 2
            }

            UpdateCheckTrue(width: (int)to.x, height: (int)to.z);

            Player other = player.other;
            if (other.isLock)
            {
                other.isLock = false;
                other.getDirection = other.temp % 10 - 1;
                blocks[(int)other.transform.position.z, (int)other.transform.position.x].data = other.temp;
                GetDestination(movements, other, other.transform.position, other.transform.position);
                //other.Move(this, other.temp % 10 - 1);
            }




        }
        else//cant block
        {
            

            if((to.y == 1 && next == BlockNumber.character) || (to.y == 2 &&next == BlockNumber.upperCharacter))
            {
                player.actionnum = 4; // character끼리 충돌 : 4

                if (player.onCloud)
                    player.isLock = true;
            }
            else if(player.state == State.Master && to.y == 1 && next >= BlockNumber.upperNormal && next < BlockNumber.upperObstacle)
            {
                player.actionnum = 3;
                player.stateChange = true;
            }
            else
            {
                player.actionnum = 3;
            }
        }

        //pos = new Vector3(posX, pos.y, posZ);
        //player.targetPositions.Add(new Tuple<Vector3, int>(pos, player.getDirection));
        movements.Add(new Unit_Movement(player, player.getDirection,from, to));
        //temp
        player.temp = blocks[(int)to.z, (int)to.x].data;
		
		switch ((int)to.y)
        {
            case 1:
                blocks[(int)to.z, (int)to.x].data = BlockNumber.character;
                break;
            case 2:
                blocks[(int)to.z, (int)to.x].data = BlockNumber.upperCharacter;
                break;
            case 3:
                blocks[(int)to.z, (int)to.x].data = BlockNumber.upperCharacter;
                break;
        }
        //remaincheck는 도착한 후




        GameController.instance.moveCommand.SetLaterData(snowList, stepped_blockList);

        snowList.Clear();
        stepped_blockList.Clear();

        
        
    }
    public void UpdateCheckTrue(int width, int height)
    {
        stepped_blockList.Add(blocks[height, width]);

        if (!check[height, width])
        {
            snowList.Add(new Vector2(height, width));
            check[height, width] = true;
        }
    }

    public void UpdateCheckArray(int width, int height, bool isCheck)
    {
        //Debug.Log(height + "," + width + "  is checked " + isCheck);
        check[height, width] = isCheck;
    }
    
    public int GetBlockData(int x, int z)
    {
        if (x < mapsizeW && x >=0 && z < mapsizeH && z >= 0)
            return blocks[z, x].data;
        else
            return BlockNumber.obstacle;
    }
    
    public void SetBlockData(int x, int z , int value)
    {
        blocks[z, x].data = value;
    }

    public void SetBlocks(int x, int z , Block block)
    {
        blocks[z, x] = block;
    }

    public Block GetBlock(int width, int height)
    {
        return blocks[height, width];
    }

	

	
	
}


