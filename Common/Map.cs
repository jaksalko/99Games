using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




[Serializable]
public class Map : MonoBehaviour, IMap
{
    public string map_title;
    public int total_snow;

    int[,] step;
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

    
    private void Awake()
    {
        Debug.Log("Activate");
        step = new int[4, 2] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };

        blocks = new Block[mapsizeH, mapsizeW];//로더의 MakeGround에서 채워짐
        check = new bool[mapsizeH, mapsizeW];//마찬가지

        snowList = new List<Vector2>();
        stepped_blockList = new List<Block>();
        
    }

   

    public void Initialize(Vector2 size, bool isParfait, Vector3 posA, Vector3 posB, List<List<int>> datas, List<List<int>> styles, List<int> star_limit)
    {
        mapsizeH = (int)size.x;
        mapsizeW = (int)size.y;
        parfait = isParfait;
        startPositionA = posA;
        startPositionB = posB;


        this.star_limit = star_limit;

        this.datas = datas;
        this.styles = styles;
        if(map_title == null)
            map_title = PlayerPrefs.GetString("nickname","pingpengboong") + " " + DateTime.Now.ToString("yyyyMMddHHmmss");
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
                return BlockNumber.GetDownstairThroughBlock(getDirection, onCloud);
            case 2:
                return BlockNumber.GetUpstairThroughBlock(getDirection, onCloud);
            case 3:
                return BlockNumber.GetThirdFloorThroughBlock(getDirection, onCloud);
                

        }

        return new List<int>();
    }
    List<int> GetStopBlockList(int floor, int getDirection, bool onCloud)
    {
        switch (floor)
        {
            case 1:
                return BlockNumber.GetDownstairStopBlock(getDirection, onCloud);
            case 2:
                return BlockNumber.GetUpstairStopBlock(getDirection, onCloud);
            case 3:
                return BlockNumber.GetThirdFloorStopBlock(getDirection, onCloud);

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

    

    bool ChangeState(int next, int nextnext, Player player, ref Vector3 pos)
    {

        int floor = (int)pos.y;
        int direction = player.getDirection;
        int posX = (int)pos.x;
        int posZ = (int)pos.z;

        switch (floor)
        {
            case 1:
                if (next >= BlockNumber.parfaitA && next <= BlockNumber.parfaitD)
                {
                    //change block data parfait to normal
                    //blocks[posZ + step[direction, 0], posX + step[direction, 1]].data = BlockNumber.normal; //pos 위치가 아닌 한칸 이동한 위치
                    if ((next % 10 - 1) == GameController.ParfaitOrder)
                        GameController.ParfaitOrder++;


                    return true;
                }
                else if(next >= BlockNumber.cloudUp && next <= BlockNumber.cloudLeft)
                {
                    int cloudDirection = (next % 10) - 1;
                    Vector3 targetPosition = new Vector3(posX + step[direction, 1], pos.y, posZ + step[direction, 0]);

                    if (player.getDirection != cloudDirection)
                    {
                        player.getDirection = cloudDirection;
                        player.targetPositions.Add(new Tuple<Vector3, int>(targetPosition,cloudDirection));
                    }
                    
                    player.onCloud = true;

                    return true;
                }
                else if (next >= BlockNumber.slopeUp && next <= BlockNumber.slopeLeft)
                {
                    int nextFloor = floor + 1;
                    if (CheckNextBlock(GetThroughBlockList(nextFloor, player.getDirection, player.onCloud), nextnext) || CheckNextBlock(GetStopBlockList(nextFloor, player.getDirection, player.onCloud), nextnext))//다음은 지나갈 수 있는 블럭
                    {
                        Debug.Log("floor : 1");
                        //다음 블럭은 올라갈 수 있다
                        pos.y += 1;
                        return true;


                    }
                    else
                    {
                        Debug.Log("cant climb slope...");
                        //올라갈 수 없다 --> 슬로프를 올라가서는 안되므로 false 를 반환.
                        return false;
                    }
                    //player upstair --> true (floor = 1)
                    //if state==master --> other.floor = 2
                    //블럭 리스트 업데이트

                    //슬로프 앞에가 막혀있다면
                    //리턴 false
                    //upstair --> false (floor = 0)
                    //if state==master --> other.floor = 1
                    //다시 블럭리스트 업데이트 
                }
                else
                {
                    return true;
                }

            case 2:
                if (next >= BlockNumber.upperParfaitA && next <= BlockNumber.upperParfaitD)
                {
                    //blocks[posZ + step[direction, 0], posX + step[direction, 1]].data = BlockNumber.upperNormal;
                    if((next%10-1) == GameController.ParfaitOrder)
					    GameController.ParfaitOrder++;
                    Debug.Log("up");
                    return true;
                }
                else if (next >= BlockNumber.cloudUp && next <= BlockNumber.cloudLeft)
                {
                    pos.y -= 1;

                    int cloudDirection = (next % 10) - 1;
                    Vector3 targetPosition = new Vector3(posX + step[direction, 1], pos.y, posZ + step[direction, 0]);

                    if (player.getDirection != cloudDirection)
                    {
                        player.getDirection = cloudDirection;
                        player.targetPositions.Add(new Tuple<Vector3, int>(targetPosition, cloudDirection));
                    }
                    player.onCloud = true;
                    

                    return true;
                }
                else if (next >= BlockNumber.upperCloudUp && next <= BlockNumber.upperCloudLeft)
                {
                    int cloudDirection = (next % 10) - 1;
                    Vector3 targetPosition = new Vector3(posX + step[direction, 1], pos.y, posZ + step[direction, 0]);

                    if (player.getDirection != cloudDirection)
                    {
                        player.getDirection = cloudDirection;
                        player.targetPositions.Add(new Tuple<Vector3, int>(targetPosition, cloudDirection));
                    }

                    player.onCloud = true;

                    return true;
                }
                else if (next >= BlockNumber.slopeUp && next <= BlockNumber.slopeLeft)
                {
                    int nextFloor = floor - 1;
                    if (CheckNextBlock(GetThroughBlockList(nextFloor, player.getDirection, player.onCloud), nextnext) || CheckNextBlock(GetStopBlockList(nextFloor, player.getDirection, player.onCloud), nextnext))//다음은 지나갈 수 있는 블럭
                    {
                        //다음 블럭은 내려갈 수 있다
                        pos.y -= 1;
                        return true;

                    }
                    else
                    {
                        //내려갈 수 없다 --> 슬로프를 내력가서는 안되므로 false 를 반환.
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            case 3://2층에서는 through 로 들어올 수 없음.?
                if (next >= BlockNumber.cloudUp && next <= BlockNumber.cloudLeft)
                {
                    
                    player.onCloud = true;
                    pos.y -= 2;
                    int cloudDirection = (next % 10) - 1;
                    Vector3 targetPosition = new Vector3(posX + step[direction, 1], pos.y, posZ + step[direction, 0]);

                    if (player.getDirection != cloudDirection)
                    {
                        player.getDirection = cloudDirection;
                        player.targetPositions.Add(new Tuple<Vector3, int>(targetPosition, cloudDirection));
                    }
                    return true;
                }
                else if (next >= BlockNumber.upperCloudUp && next <= BlockNumber.upperCloudLeft)
                {
                   
                    player.onCloud = true;
                    pos.y -= 1;
                    int cloudDirection = (next % 10) - 1;
                    Vector3 targetPosition = new Vector3(posX + step[direction, 1], pos.y, posZ + step[direction, 0]);

                    if (player.getDirection != cloudDirection)
                    {
                        player.getDirection = cloudDirection;
                        player.targetPositions.Add(new Tuple<Vector3, int>(targetPosition, cloudDirection));
                    }
                    return true;
                }
                return false;

        }

        return false;//error
    }

    
    public void GetDestination(Player player, Vector3 pos)
    {
        

        Debug.Log("player name : " + player.name + " position : " + pos + " player dir : " + player.getDirection);
        int direction = player.getDirection;
       
        int floor = (int)pos.y;
        int posX = (int)pos.x;
        int posZ = (int)pos.z;

        if (pos == player.transform.position)//시작부분
        {
            Debug.Log("frist time call by getDestination");
            if (blocks[posZ,posX].type == Block.Type.Cracker)
            {
                Debug.Log("frist time call by getDestination : add cracker block");
                stepped_blockList.Add(blocks[posZ, posX]);
            }
        }



        int next = GetBlockData(x: posX + step[direction, 1], z: posZ+ step[direction, 0]);
        int nextnext = GetBlockData(x: posX + step[direction, 1] * 2, z: posZ + step[direction, 0] * 2);

        if(CheckNextBlock(GetThroughBlockList(floor, direction, player.onCloud), next) && ChangeState(next, nextnext, player , ref pos))//다음은 지나갈 수 있는 블럭
        {
            //지나갈 수 있는 블럭
            player.isLock = false;

            posX += step[direction, 1];
            posZ += step[direction, 0];
            
            UpdateCheckTrue(width: posX, height: posZ);

            pos = new Vector3(posX, pos.y, posZ);
            //if not endpoint recursive next point
            if (!isEndGame())
            {
                Player other = player.other;
                if(other.isLock)
                {
                    other.isLock = false;
                    //other.Move()
                    other.Move(this, other.temp % 10 - 1);
                }


                GetDestination(player, pos);
                return;

            }


        }
        else if (CheckNextBlock(GetStopBlockList(floor, direction, player.onCloud), next))//다음은 멈춰야하는 블럭
        {
            player.onCloud = false; // stop 이면 무조건 oncloud 에서 벗어남.
            player.isLock = false;

            posX += step[direction, 1];
            posZ += step[direction, 0];

            switch (floor)
            {
                case 1://솜사탕 위였으면 1단계 블럭 또는 열려있는 파르페  솜사탕 위가 아니면 솜사탕에서 멈춤 충돌 모션은
                    //actionnum = 3; //
                    
                    if (next >= BlockNumber.parfaitA && next <= BlockNumber.parfaitD)
                    {
                        player.actionnum = 3;//crash : 3
                        blocks[posZ, posX].data = BlockNumber.normal;
                        if ((next % 10 - 1) == GameController.ParfaitOrder)
                            GameController.ParfaitOrder++;
                    }
                    else
                    {
                        player.actionnum = 3;//crash : 3
                    }
                    break;

                case 2://drop 1-> 0 or ride character
                    if(next == BlockNumber.character)
                    {
                        //ride motion
                        player.actionnum = 2;//ride : 2

                        //player state          Idle --> Slave
                        //other player state    Idle --> Master
                    }
                    else if(next >= BlockNumber.normal && next <= BlockNumber.cracker_2)
                    {
                        pos.y -= 1;
                        player.actionnum = 5;//drop : 5
                    }
                    else if (next >= BlockNumber.parfaitA && next <= BlockNumber.parfaitD)
                    {
                        pos.y -= 1;
                        player.actionnum = 5;//drop : 5
                        blocks[posZ, posX].data = BlockNumber.normal;
                        if ((next % 10 - 1) == GameController.ParfaitOrder)
                            GameController.ParfaitOrder++;
                    }
                    else if (next >= BlockNumber.upperParfaitA && next <= BlockNumber.upperParfaitD)//onCloud(2층)에서 2층 파레페 먹고 멈
                    {
                        player.actionnum = 3;// crash : 3
                        blocks[posZ, posX].data = BlockNumber.upperNormal;
                        if ((next % 10 - 1) == GameController.ParfaitOrder)
                            GameController.ParfaitOrder++;
                    }
                    else
                    {
                        player.actionnum = 3;// crash : 3
                    }

                    break;

                case 3://drop 2-> 1 or 0
                    if (next >= BlockNumber.normal && next <= BlockNumber.cracker_2)
                    {
                        player.actionnum = 5;//drop : 5
                        pos.y -= 2;
                    }
                    else if(next >= BlockNumber.parfaitA && next <= BlockNumber.parfaitD)
                    {
                        player.actionnum = 5;//drop : 5
                        pos.y -= 2;
                        blocks[posZ, posX].data = BlockNumber.normal;
                        if ((next % 10 - 1) == GameController.ParfaitOrder)
                            GameController.ParfaitOrder++;
                    }
                    else if(next >= BlockNumber.upperNormal && next <= BlockNumber.upperCracker_2)
                    {
                        player.actionnum = 5;//drop : 5
                        pos.y -= 1;
                    }
                    else if (next >= BlockNumber.upperParfaitA && next <= BlockNumber.upperParfaitD)
                    {
                        player.actionnum = 5;//drop : 5
                        pos.y -= 1;
                        blocks[posZ, posX].data = BlockNumber.upperNormal;
                        if ((next % 10 - 1) == GameController.ParfaitOrder)
                            GameController.ParfaitOrder++;
                    }
                    else
                    {
                        player.actionnum = 3;//crash : 3
                    }
                    break;
            }//end switch


            
            
            pos = new Vector3(posX, pos.y, posZ);
            UpdateCheckTrue(width: posX, height: posZ);

            Player other = player.other;
            if (other.isLock)
            {
                other.isLock = false;
                //other.Move()
                other.Move(this, other.temp % 10 - 1);
            }




        }
        else//cant block
        {
            Debug.Log("cant block : " + pos + " BlockNumber : " + next);

            if((pos.y == 1 && next == BlockNumber.character) || (pos.y == 2 &&next == BlockNumber.upperCharacter))
            {
                player.actionnum = 4; // character끼리 충돌 : 4

                if (player.onCloud)
                    player.isLock = true;
            }
            else if(player.state == Player.State.Master && pos.y == 1 && next >= BlockNumber.upperNormal && next < BlockNumber.upperObstacle)
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
        player.targetPositions.Add(new Tuple<Vector3, int>(pos, player.getDirection));
        //temp
        player.temp = blocks[posZ, posX].data;
		Debug.Log("player temp is : " + player.temp);
		switch ((int)pos.y)
        {
            case 1:
                blocks[posZ, posX].data = BlockNumber.character;
                break;
            case 2:
                blocks[posZ, posX].data = BlockNumber.upperCharacter;
                break;
            case 3:
                blocks[posZ, posX].data = BlockNumber.upperCharacter;
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


