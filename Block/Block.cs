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
        Outline,
        Character
    };

    public int style; // 0 : 튜토리얼 1 : 아이스크림 2 : 파르페 3 : 크래커 4 : 솜사탕 5 : 에디터
    public GameObject[] object_styles;

    public int data { get; set; }
    

    public virtual void Init(int block_num , int style)
    {
        data = block_num;
        this.style = style;
    }

    
}
