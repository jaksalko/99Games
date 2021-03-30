using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class Parser
{
    public static int ASCII(char s)
    {
        return s - 'A';
    }

    public static char IntToASCII(int n)
    {
        return 'n';
    }

    public static List<int> StringToList(string s)
    {
        List<int> datas = new List<int>();
        datas = s.Split(',').Select(int.Parse).ToList();
        return datas;
    }
    public static string ListToString(List<int> datas)
    {
        List<int> t = new List<int>();
        string s = "";
        s = string.Join(",", datas);
        t = s.Split(',').Select(int.Parse).ToList();

        for(int i = 0; i < t.Count; i++)
        {
            Debug.Log(t[i]);
        }
        

        return s;
    }
}
