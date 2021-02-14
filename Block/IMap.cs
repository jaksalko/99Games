using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IMap
{
    List<Tuple<Vector3,int>> GetDestination(Player player , Vector3 startPosition);
    void UpdateCheckArray(int width, int height, bool isCheck);
    int GetBlockData(int x, int z);
    void SetBlockData(int x, int z, int value);
    void SetBlocks(int x, int z, Block block);
}
