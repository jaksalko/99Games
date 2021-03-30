using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBlock : Block
{

	public GameObject snow;

	
    public override void Init(int block_num, int style)
    {
        base.Init(block_num, style);
        object_styles[style].SetActive(true);
        SetSnow();
    }

    void SetSnow()
    {
        if(BlockNumber.normal == data && transform.position.y == 0)
        {
            snow.SetActive(true);
        }
        else if(BlockNumber.upperNormal == data && transform.position.y == 1)
        {
            snow.SetActive(true);
        }
        else
        {
            snow.SetActive(false);
        }

    }

}
