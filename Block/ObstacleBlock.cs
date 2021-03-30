using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBlock : Block
{
   
    public override void Init(int block_num, int style)
    {
       
        base.Init(block_num, style);
        object_styles[style].SetActive(true);

    }
}
