using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CloudBlock : Block
{
    public static event Action<GameObject> Exit;

    public int direction; // 0 상 1 우 2 하 3 좌
    Vector2 pos_;
    public override void Init(int block_num,int style)
    {
        base.Init(block_num,style);
        object_styles[style].SetActive(true);

        direction = (block_num % 10) - 1;//block num 11-14 21-24
        pos_ = new Vector2(transform.position.x, transform.position.z);
        transform.rotation = Quaternion.Euler(new Vector3(0, 90 * direction, 0));
    }

   
}
