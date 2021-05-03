using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBlock : Block
{

	public GameObject snow;
    public Snow[] snows;
	
    public override void Init(int block_num, int style)
    {
        base.Init(block_num, style);
        object_styles[style].SetActive(true);
        SetSnow();
    }

    void SetSnow()//바닥에 깔리는 기본블럭은 눈이 설치되지 못하게 함. Visual
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

    public override void RevertBlock()
    {
        base.RevertBlock();
        for(int i = 0; i < snows.Length; i++)
        {
            snows[i].gameObject.SetActive(true);
        }
    }
}
