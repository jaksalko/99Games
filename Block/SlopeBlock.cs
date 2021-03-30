using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeBlock : Block
{
    int direction;

    public override void Init(int block_num, int style)
    {
        base.Init(block_num, style);
        object_styles[style].SetActive(true);

        direction = block_num % 25;//block num 25-29
        transform.rotation = Quaternion.Euler(new Vector3(0, 90 * direction, 0));
    }
}