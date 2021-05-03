using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EditorBlock : MonoBehaviour
{
    public int blockNumber;
    public int styleNumber;

    public void SetEditorBlock(int block , int style)
    {
        blockNumber = block;
        styleNumber = style;
    }

    public List<int> placeableFloor()
    {
        List<int> placeableList = new List<int>();

        if(BlockNumber.first_floor.Contains(blockNumber))
        {
            placeableList.Add(1);

            if (blockNumber != BlockNumber.normal)
                placeableList.Add(2);
        }
        else if(BlockNumber.second_floor.Contains(blockNumber))
        {
            placeableList.Add(2);

            if(blockNumber == BlockNumber.character || blockNumber == BlockNumber.obstacle || (blockNumber >= BlockNumber.parfaitA && blockNumber <= BlockNumber.parfaitD) )
            {
                placeableList.Add(3);
            }
        }
        else if(BlockNumber.third_floor.Contains(blockNumber))
        {
            placeableList.Add(3);
        }

        foreach(int i in placeableList)
        {
            Debug.Log(i);
        }
        return placeableList;
    }
   
}
