using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomMapItem : MonoBehaviour
{
    

    public Text title;
    public Text maker;
    public Text moveCount;
    public Text likes;
    public Text play;
    public Image difficulty;
    public Image isClear;
    // Start is called before the first frame update

    public EditorMap itemdata;
    public Map map;
    public bool isPlayed;

    public Sprite minable;
    public Sprite unminable;
    public void Initialize(EditorMap item)
    {
        SetMining(false);
        name = item.title;
        itemdata = item;
        title.text = itemdata.title;
        maker.text = itemdata.maker;
        moveCount.text = itemdata.move.ToString();
        likes.text = itemdata.likes.ToString();
        play.text = itemdata.play_count.ToString();

        difficulty.sprite = Resources.Load<Sprite>("Icon/Level/" + itemdata.level);
        //isClear.sprite = Resources.Load<Sprite>();

        List<List<int>> datas = Parser.StringToListList(itemdata.datas, itemdata.height, itemdata.width);
        List<List<int>> styles = Parser.StringToListList(itemdata.styles, itemdata.height*itemdata.width, 3);
        List<int> star_limit = Parser.StringToList(itemdata.star_limit);

        Vector3 posA = default;
        Vector3 posB = default;

        bool isParfait = false;
        for(int i = 0; i < datas.Count; i++)
        {
            for(int j = 0; j < datas[i].Count; j++)
            {
                if(datas[i][j] == BlockNumber.character)
                {
                    if(posA == default)
                    {
                        posA = new Vector3(j, 1, i);
                    }
                    else
                    {
                        posB = new Vector3(j, 1, i);
                    }
                }
                else if(datas[i][j] == BlockNumber.upperCharacter)
                {
                    if (posA == default)
                    {
                        posA = new Vector3(j, 2, i);
                    }
                    else
                    {
                        posB = new Vector3(j, 2, i);
                    }
                }
                else if(datas[i][j] >= BlockNumber.parfaitA && datas[i][j] <= BlockNumber.parfaitD)
                {
                    isParfait = true;
                }
                else if(datas[i][j] >= BlockNumber.parfaitA && datas[i][j] <= BlockNumber.parfaitD)
                {
                    isParfait = true;
                }
            }
        }

        map.Initialize(new Vector2(itemdata.height, itemdata.width),isParfait, posA, posB, datas, styles, star_limit);
        map.map_title = itemdata.title;
        moveCount.text = itemdata.move.ToString();

        
    }

    public void SetMining(bool played)
    {
        isPlayed = played;
        if (isPlayed)
            isClear.sprite = unminable;
        else
            isClear.sprite = minable;
    }

    public void PlayButton()
    {
        AWSManager aws = AWSManager.instance;

        for (int i = 0; i < aws.editorMap.Count; i++)
        {
            if(aws.editorMap[i].itemdata.title == itemdata.title)
            {
                GameManager.instance.customMap = aws.editorMap[i].map;
                GameManager.instance.playCustomData = itemdata;
                
                GameManager.instance.retry = isPlayed;
                SceneManager.LoadScene("CustomMapPlayScene");//customMode Scene
                break;
            }
        }
        

        

        
    }
}
