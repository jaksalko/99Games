using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Block : MonoBehaviour , IBlock
{
    public enum Type
    {
        Ground,
        SecondGround,
        Slope,
        Obstacle,
        Parfait,
        Cracked,
        broken,
        Cloud,
        Outline
    };

	public int Data { get; set; }
    public bool Snow { get; set; }
    public virtual void Init(int block_num)
    {
        Data = block_num;
        if((block_num == BlockNumber.normal && transform.position.y == -0.5f) || (block_num == BlockNumber.upperNormal && transform.position.y == 0.5f))
        {
            Snow = true;
        }
        else
        {
            Snow = false;
        }
    }

    public bool IsSnow()
    {
        if(Snow)
        {
            Snow = false;
            return true;
        }
        else
        {
            return false;
        }
        //throw new System.NotImplementedException();
    }
}
