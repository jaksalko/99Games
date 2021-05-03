using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlockStore : MonoBehaviour
{
    public Transform[] contents;
    CSVManager csvManager = CSVManager.instance;

    // Start is called before the first frame update
    void Start()
    {
        csvManager = CSVManager.instance;
        List<BlockPiece> blockPieces = csvManager.blockPieces;

        for(int i = 0; i < blockPieces.Count; i++)
        {
            if(blockPieces[i].blockName.Contains("아이스크림"))
            {
                blockPieces[i].transform.SetParent(contents[0]);
                
            }
            else if (blockPieces[i].blockName.Contains("파르페"))
            {
                blockPieces[i].transform.SetParent(contents[1]);
            }
            else if (blockPieces[i].blockName.Contains("크래커"))
            {
                blockPieces[i].transform.SetParent(contents[2]);
            }
            else if (blockPieces[i].blockName.Contains("솜사탕"))
            {
                blockPieces[i].transform.SetParent(contents[3]);
            }

            blockPieces[i].transform.localPosition = default;
            blockPieces[i].transform.localScale = new Vector3(1,1,1);
        }
    }

    void GetItemList()
    {

    }
   
}
