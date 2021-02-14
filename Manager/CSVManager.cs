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

    public Map mapPrefab;
    public Reward rewardPrefab;
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

        
    }
    void Start()
    {
        SetIsalndList();

        SetRewardList();

    }

    public void SetIsalndList()
    {
        stage = CSVReader.Read("stage");
        Debug.Log("stage count : " + stage);
        for (int i = 0; i < stage.Count; i++)
        {
            int title = int.Parse(stage[i]["title"].ToString());

            Map newMap = Instantiate(mapPrefab, default);
            newMap.map_title = stage[i]["title_text"].ToString();
            newMap.total_snow = int.Parse(stage[i]["snow_total"].ToString());
            int width = int.Parse(stage[i]["width"].ToString());
            int height = int.Parse(stage[i]["height"].ToString());
            int[,] datas = MapString(stage[i]["data"].ToString().Split(','), height, width);
            Debug.Log(stage[i]["posA"].ToString());
            Vector3 posA = GetVector3(stage[i]["posA"].ToString().Split('/'));
            Vector3 posB = GetVector3(stage[i]["posB"].ToString().Split('/'));
            bool isParfait = int.Parse(stage[i]["parfait"].ToString()) == 0 ? false : true;
            List<int> star_limit = GetList(stage[i]["star_limit"].ToString().Split('/'));

            newMap.Initialize(new Vector2(height, width), isParfait, posA, posB, datas, star_limit);
            newMap.transform.SetParent(transform);

            islands[title].maps.Add(newMap);

        }
    }

    public void SetRewardList()
    {
        rewards_csv = CSVReader.Read("reward");

        for (int i = 0; i < rewards.Count; i++)
        {
            int island_num = int.Parse(rewards_csv[i]["island"].ToString());
            int star_num = int.Parse(rewards_csv[i]["num"].ToString());

            string reward = rewards_csv[i]["reward"].ToString();
            int quantity = int.Parse(rewards_csv[i]["quantity"].ToString());

            Reward newReward = Instantiate(rewardPrefab, default);
            newReward.SetReward(island_num, star_num, reward, quantity);

            newReward.transform.SetParent(transform);
            rewards[island_num].rewards.Add(newReward);
        }
    }

    int[,] MapString(string[] data_string , int height , int width)
    {
        

        int[,] datas = new int[height,width];
        int count = 0;
        for(int i = 0 ; i < height ; i++)
        {
            for(int j = 0 ; j < width ; j++)
            {
                datas[i,j] = int.Parse(data_string[count]);
                count++;
            }
        }

        return datas;
    }

    Vector3 GetVector3(string[] data_string)
    {
        Debug.Log(data_string.Length);

        Vector3 pos = new Vector3();
        pos.x = int.Parse(data_string[0]);
        pos.y = int.Parse(data_string[1]);
        pos.z = int.Parse(data_string[2]);

        return pos;
    }

    List<int> GetList(string[] data_string)
    {
        List<int> list = new List<int>();
        for(int i = 0 ; i < data_string.Length ; i++)
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
        for(int i = 0; i < IslandData.island_last.Length ; i++)
        {
            int island_last = IslandData.island_last[i];
            int island_num;
            if(stage_num <= island_last)//이 섬에 해당하는 스테이지라면
            {
                if (i != 0)
                {
                    island_num = stage_num - IslandData.island_last[i - 1];
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
