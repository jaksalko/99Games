using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CSVManager : MonoBehaviour
{
    public static CSVManager instance = null;

    List<Dictionary<string, object>> stage;
    List<Dictionary<string, object>> rewards_csv;

    public List<Island> islands = new List<Island>();
    public List<Rewards> rewards = new List<Rewards>();

    public List<Skin> skins = new List<Skin>();
    

    public List<BlockPiece> blockPieces = new List<BlockPiece>();
    public BlockPiece blockPiecePrefab;

    public List<Style> styleList = new List<Style>();

    public Map mapPrefab;
    public Reward rewardPrefab;

    public IslandData islandData;
    // Start is called before the first frame update

    [Serializable]
    public struct Island
    {
        [SerializeField]
        public List<Map> maps;
    }

    [Serializable]
    public struct Rewards
    {
        [SerializeField]
        public List<Reward> rewards;
    }

    void Awake()
    {
        
        if (instance == null)
        {
            Debug.Log("Single instance is null");
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Single instance is not Single.. Destroy gameobject!");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);//Dont destroy this singleton gameobject :(

        GetStyleList();
        SetRewardList();
        SetIslandList();

        GetSkinItemData();
        GetBlockItemData();

    }
    void Start()
    {
        


    }
    public void GetStyleList()
    {
        List<Dictionary<string, object>> item = CSVReader.Read("style");

        for (int i = 0; i < item.Count; i++)
        {
            string txt = item[i]["style_text"].ToString();
            string info = item[i]["style_info"].ToString();
            string type = item[i]["type"].ToString();
            string condition = item[i]["condition"].ToString();
            int standard = int.Parse(item[i]["standard"].ToString());
            int reward_boong = int.Parse(item[i]["reward_boong"].ToString());
            int reward_heart = int.Parse(item[i]["reward_heart"].ToString());
            string reward_item = item[i]["reward_item"].ToString();

            Style style = new Style(txt, info, type, condition, standard, reward_boong, reward_heart, reward_item);
            styleList.Add(style);
            //StartCoroutine(style.CheckStyle());
        }



    }
    public void GetSkinItemData()
    {
        List<Dictionary<string, object>> item = CSVReader.Read("item");
        int skin_num = 0;
        for (int i = 0; i < item.Count; i++)
        {
            string type = item[i]["type"].ToString();
            
            Debug.Log(type);
            if (type == "skin")
            {
                string name_ = item[i]["name"].ToString();
                string info = item[i]["info"].ToString();
                int boong = int.Parse(item[i]["boong"].ToString());
                int skin_powder = int.Parse(item[i]["skin_powder"].ToString());
                string path = item[i]["path"].ToString();

                Skin skin = new Skin(name_, info, path, boong, skin_powder, 0,skin_num);
                skins.Add(skin);
                skin_num++;
                /*
                Debug.Log(name_ +" " + path);
                SkinItem skinItem = Instantiate(skinItemPrefab);
                MyIglooSkinItem myIglooSkinItem = Instantiate(myIglooSkinItemPrefab);
                myIglooSkinItem.InitializeItem(name_, info, path);
                myIglooSkinItems.Add(myIglooSkinItem);
                skinItem.Initialize(name_, info, boong, skin_powder, path);
                skinItems.Add(skinItem);

                myIglooSkinItem.transform.SetParent(transform, false);
                skinItem.transform.SetParent(transform, false);
                */
            }
        }
    }
    public void GetBlockItemData()
    {
        List<Dictionary<string, object>> item = CSVReader.Read("item");

        for (int i = 0; i < item.Count; i++)
        {
            string type = item[i]["type"].ToString();
            if (type == "blockPiece")
            {
                string name = item[i]["name"].ToString();
                string info = item[i]["info"].ToString();
                int block_powder = int.Parse(item[i]["block_powder"].ToString());
                
                string path = item[i]["path"].ToString();

                BlockPiece blockPiece = Instantiate(blockPiecePrefab);
                blockPiece.transform.SetParent(transform);
                blockPiece.Initialize(name, info, block_powder, path);
                blockPieces.Add(blockPiece);
            }
        }
    }

    public void SetIslandList()
    {
        
        stage = CSVReader.Read("stage_demo");
        Debug.Log("stage count : " + stage);
        for (int i = 0; i < stage.Count; i++)
        {
            int title = int.Parse(stage[i]["title"].ToString());

            Map newMap = Instantiate(mapPrefab, default);
            newMap.map_title = stage[i]["title_text"].ToString();
            newMap.total_snow = int.Parse(stage[i]["snow_total"].ToString());
            int width = int.Parse(stage[i]["width"].ToString());
            int height = int.Parse(stage[i]["height"].ToString());


            List<List<int>> datas = MapString(stage[i]["data"].ToString().Split(','), height, width);
            List<List<int>> styles = new List<List<int>>();


            for (int h = 0; h < height; h++)
            {

                for (int w = 0; w < width; w++)
                {
                    int blockNumber = datas[h][w];
                    styles.Add(GetStyleList(blockNumber, title));
                }
            }

 //           Debug.Log(stage[i]["posA"].ToString());
            Vector3 posA = GetVector3(stage[i]["posA"].ToString().Split('/'));
            Vector3 posB = GetVector3(stage[i]["posB"].ToString().Split('/'));
            bool isParfait = int.Parse(stage[i]["parfait"].ToString()) == 0 ? false : true;
            List<int> star_limit = GetList(stage[i]["star_limit"].ToString().Split('/'));

            newMap.Initialize(new Vector2Int(height, width), isParfait, posA, posB, datas, styles, star_limit);
            newMap.transform.SetParent(transform);

            islands[title].maps.Add(newMap);

            
        }

        islandData = new IslandData(islands[0].maps.Count-1,
            islands[1].maps.Count,
            islands[2].maps.Count,
            islands[3].maps.Count,
            islands[4].maps.Count);
    }

    List<int> GetStyleList(int blockNumber, int title)
    {
        List<int> style = new List<int>();


        style.Add(title);
        style.Add(title);
        style.Add(title);


        return style;
    }

    public void SetRewardList()
    {
        rewards_csv = CSVReader.Read("starReward");
        //Debug.Log(rewards_csv.Count);

        for (int i = 0; i < rewards_csv.Count; i++)
        {


            int island = int.Parse(rewards_csv[i]["island"].ToString());
            int level = int.Parse(rewards_csv[i]["level"].ToString());

            int boong = int.Parse(rewards_csv[i]["boong"].ToString());
            int heart = int.Parse(rewards_csv[i]["heart"].ToString());
            int block_powder = int.Parse(rewards_csv[i]["block_powder"].ToString());
            int skin_powder = int.Parse(rewards_csv[i]["skin_powder"].ToString());
            string item = rewards_csv[i]["item"].ToString();

            //Debug.Log(item);



            Reward newReward = Instantiate(rewardPrefab, default);
            newReward.SetReward(island, level, boong, heart, block_powder, skin_powder, item);

            newReward.transform.SetParent(transform);
            rewards[island].rewards.Add(newReward);
        }
    }

    List<List<int>> MapString(string[] data_string, int height, int width)
    {

        List<List<int>> map_datas = new List<List<int>>();


        int count = 0;

        for (int i = 0; i < height; i++)
        {
            List<int> line = new List<int>();
            for (int j = 0; j < width; j++)
            {
                int data = int.Parse(data_string[count]);
                line.Add(data);

                count++;
            }
            map_datas.Add(line);
        }

        return map_datas;
    }

    Vector3 GetVector3(string[] data_string)
    {
//        Debug.Log(data_string.Length);

        Vector3 pos = new Vector3();
        pos.x = int.Parse(data_string[0]);
        pos.y = int.Parse(data_string[1]);
        pos.z = int.Parse(data_string[2]);

        return pos;
    }

    List<int> GetList(string[] data_string)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < data_string.Length; i++)
        {
            list.Add(int.Parse(data_string[i]));
        }

        return list;
    }

    public List<Reward> GetRewardList(int island_num)
    {
        return rewards[island_num].rewards;
    }

    public Map GetMap(int stage_num)
    {
        for (int i = 0; i < islandData.island_last.Length; i++)
        {
            int island_last = islandData.island_last[i];
            int island_num;
            if (stage_num <= island_last)//이 섬에 해당하는 스테이지라면
            {
                if (i != 0)
                {
                    island_num = stage_num - islandData.island_last[i - 1];
                    return islands[i].maps[island_num - 1];
                }

                else
                {
                    return islands[i].maps[stage_num];
                }

            }
        }

        return null;
    }

}
