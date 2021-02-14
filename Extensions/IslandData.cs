using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class IslandData
{
    public const int
        tutorial = 4,   // 0 - 4
        iceCream = 16,  // 5-6
        beach = 26,     // 7-8
        cracker = 37,   //9-11
        cottoncandy = 47;//37

    public const string
        stage1 = "Tutorial_Island",
        stage2 = "Icecream_Island",
        stage3 = "Beach_Island",
        stage4 = "Cracker_Island",
        stage5 = "Cottoncandy_Island";

    public const int lastLevel = 47;//13
    public static int[] island_last = { tutorial, iceCream, beach, cracker, cottoncandy };

    public static int Island_Num(int stage)
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
}
