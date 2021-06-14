using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IslandData
{
    public int
        tutorial,   // 0 - 4
        icecream,  // 5-6
        beach,     // 7-8
        cracker,   //9-11
        cottoncandy;//37


    public int lastLevel;//13
    public int[] island_last;
    public int[] island_start;

    public int Island_Num(int stage)
    {
        for (int i = 0; i < island_last.Length; i++)
        {
            if (stage <= island_last[i])
            {
                Debug.Log("island num : " + island_last[i]);
                return i;
            }
        }

        return -1;

    }

    public IslandData(int tutorial_ , int icecream_ ,int beach_ , int cracker_ , int cottoncandy_)
    {
        tutorial = tutorial_;
        icecream = tutorial + icecream_;
        beach = icecream + beach_;
        cracker = beach + cracker_;
        cottoncandy = cracker + cottoncandy_;

        lastLevel = cottoncandy;

        island_last = new int[] { tutorial, icecream, beach, cracker, cottoncandy };
        island_start = new int[] { 0, tutorial + 1, icecream + 1, beach + 1, cracker + 1 };
    }
   
}
