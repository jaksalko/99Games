using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public static BlockFactory instance;

    public GroundBlock groundBlockPrefab;
    public GroundBlock secondGroundBlockPrefab;
    public ObstacleBlock obstacleBlockPrefab;
    public SlopeBlock slopeBlockPrefab;
    public ParfaitBlock parfaitBlockPrefab;
    public CloudBlock cloudBlockPrefab;
    public CrackedBlock crackedBlockPrefab;

    public Player[] playerPrefabs;

    int player_count = 0;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();


        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);//Dont destroy this singleton gameobject :(

    }

    

    public Block EditorCreateBlock(int blockNumber, int style, Vector2 position)
    {
        Block newBlock;
        //Debug.Log(blockNumber);
        if (BlockNumber.normal == blockNumber)
        {

            newBlock = Instantiate(groundBlockPrefab, new Vector3(position.x, 0, position.y), groundBlockPrefab.transform.rotation);
            newBlock.name = (int)position.x + "," + (int)position.y + " : " + blockNumber;
        }
        else if (BlockNumber.upperNormal == blockNumber)
        {
            
            newBlock = Instantiate(secondGroundBlockPrefab, new Vector3(position.x, 1, position.y), groundBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.obstacle == blockNumber)
        {
           
            newBlock = Instantiate(obstacleBlockPrefab, new Vector3(position.x, 1, position.y), obstacleBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.upperObstacle == blockNumber)
        {
           
            newBlock = Instantiate(obstacleBlockPrefab, new Vector3(position.x, 2, position.y), obstacleBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.parfaitA <= blockNumber && BlockNumber.parfaitD >= blockNumber)
        {
           
            newBlock = Instantiate(parfaitBlockPrefab, new Vector3(position.x, 1, position.y), parfaitBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.upperParfaitA <= blockNumber && BlockNumber.upperParfaitD >= blockNumber)
        {
            newBlock = Instantiate(parfaitBlockPrefab, new Vector3(position.x, 2, position.y), parfaitBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.cloudUp <= blockNumber && BlockNumber.cloudLeft >= blockNumber)
        {
            newBlock = Instantiate(cloudBlockPrefab, new Vector3(position.x, 0, position.y), cloudBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.upperCloudUp <= blockNumber && BlockNumber.upperCloudLeft >= blockNumber)
        {
           
            newBlock = Instantiate(cloudBlockPrefab, new Vector3(position.x, 1, position.y), cloudBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.cracker_0 <= blockNumber && BlockNumber.broken >= blockNumber)
        {
            newBlock = Instantiate(crackedBlockPrefab, new Vector3(position.x, 0, position.y), crackedBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.upperCracker_0 <= blockNumber && BlockNumber.upperCracker_2 >= blockNumber)
        {
           
            newBlock = Instantiate(crackedBlockPrefab, new Vector3(position.x, 1, position.y), crackedBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.slopeUp <= blockNumber && BlockNumber.slopeLeft >= blockNumber)
        {
            
            newBlock = Instantiate(slopeBlockPrefab, new Vector3(position.x, 1, position.y), slopeBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.character == blockNumber)
        {
            
           


            newBlock = Instantiate(playerPrefabs[style], new Vector3(position.x, 1, position.y), playerPrefabs[style].transform.rotation);
        }
        else if (BlockNumber.upperCharacter == blockNumber)
        {
           

            newBlock = Instantiate(playerPrefabs[style], new Vector3(position.x, 2, position.y), playerPrefabs[style].transform.rotation);
        }
        else
        {
            newBlock = null;
        }


        newBlock.Init(blockNumber, style);

        return newBlock;
    }

    public Block CreateBlock(int blockNumber, List<int> styles, Vector2 position)
    {
        

        Block newBlock;
        int height = 0;
        //Debug.Log(blockNumber);

        

        
        if(blockNumber >= BlockNumber.upperCharacter)//3층하단 블럭
        {
            height = 2;
            Block underBlock = Instantiate(groundBlockPrefab, new Vector3(position.x, 0, position.y), groundBlockPrefab.transform.rotation);
            underBlock.Init(blockNumber, styles[0]);
            underBlock = Instantiate(secondGroundBlockPrefab, new Vector3(position.x, 1, position.y), groundBlockPrefab.transform.rotation);
            underBlock.Init(blockNumber, styles[1]);
        }
        else if(blockNumber >= BlockNumber.upperNormal)//2층 하단 블럭
        {
            height = 1;
            Block underBlock = Instantiate(groundBlockPrefab, new Vector3(position.x, 0, position.y), groundBlockPrefab.transform.rotation);
            underBlock.Init(blockNumber, styles[0]);
        }


        if(BlockNumber.normal == blockNumber)
        {
            
            newBlock = Instantiate(groundBlockPrefab, new Vector3(position.x ,0 , position.y), groundBlockPrefab.transform.rotation);
            newBlock.name = (int)position.x + "," + (int)position.y + " : " + blockNumber;
        }
        else if(BlockNumber.upperNormal == blockNumber)
        {
            
            newBlock = Instantiate(secondGroundBlockPrefab, new Vector3(position.x, 1, position.y), groundBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.obstacle == blockNumber)
        {
            
            newBlock = Instantiate(obstacleBlockPrefab, new Vector3(position.x, 1, position.y), obstacleBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.upperObstacle == blockNumber)
        {
           
            newBlock = Instantiate(obstacleBlockPrefab, new Vector3(position.x, 2, position.y), obstacleBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.parfaitA <= blockNumber && BlockNumber.parfaitD >= blockNumber)
        {
            
            newBlock = Instantiate(parfaitBlockPrefab, new Vector3(position.x, 1, position.y), parfaitBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.upperParfaitA <= blockNumber && BlockNumber.upperParfaitD >= blockNumber)
        {
            
            newBlock = Instantiate(parfaitBlockPrefab, new Vector3(position.x, 2, position.y), parfaitBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.cloudUp <= blockNumber && BlockNumber.cloudLeft >= blockNumber)
        {
            newBlock = Instantiate(cloudBlockPrefab, new Vector3(position.x, 0, position.y), cloudBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.upperCloudUp <= blockNumber && BlockNumber.upperCloudLeft >= blockNumber)
        {
            
            newBlock = Instantiate(cloudBlockPrefab, new Vector3(position.x, 1, position.y), cloudBlockPrefab.transform.rotation);
        }
        else if(BlockNumber.cracker_0 <= blockNumber && BlockNumber.broken >= blockNumber)
        {
            newBlock = Instantiate(crackedBlockPrefab, new Vector3(position.x, 0, position.y), crackedBlockPrefab.transform.rotation);
        }
        else if (BlockNumber.upperCracker_0 <= blockNumber && BlockNumber.upperCracker_2 >= blockNumber)
        {
            
            newBlock = Instantiate(crackedBlockPrefab, new Vector3(position.x, 1, position.y), crackedBlockPrefab.transform.rotation);
        }
        else if(BlockNumber.slopeUp <= blockNumber && BlockNumber.slopeLeft >= blockNumber)
        {
            
            newBlock = Instantiate(slopeBlockPrefab, new Vector3(position.x, 1, position.y), slopeBlockPrefab.transform.rotation);
        }
        else if(BlockNumber.character == blockNumber)
        {
            int player_skin;
            if(player_count == 0)
            {
                player_count = 1;
                player_skin = AWSManager.instance.userInfo.skin_a;
            }
            else
            {
                player_count = 0;
                player_skin = AWSManager.instance.userInfo.skin_b;
            }
            newBlock = Instantiate(playerPrefabs[player_skin], new Vector3(position.x, 1, position.y), Quaternion.identity);
        }
        else if(BlockNumber.upperCharacter == blockNumber)
        {
            int player_skin;
            if (player_count == 0)
            {
                player_count = 1;
                player_skin = AWSManager.instance.userInfo.skin_a;
            }
            else
            {
                player_count = 0;
                player_skin = AWSManager.instance.userInfo.skin_b;
            }
            newBlock = Instantiate(playerPrefabs[player_skin], new Vector3(position.x, 2, position.y), Quaternion.identity);
        }
        else
        {
            newBlock = null;
        }

        

        Debug.Log(styles[styles.Count - 1]);
        newBlock.Init(blockNumber, styles[height]);

        return newBlock;
    }
        
}
