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

    public static string ArrayToString(int[,] datas)
    {
        List<int> t = new List<int>();
        for(int i = 0; i < datas.GetLength(0); i++)
        {
            for(int j = 0; j < datas.GetLength(1); j++)
            {
                t.Add(datas[i, j]);
            }
        }

        return ListToString(t);
    }

    public static string ListListToString(List<List<int>> datas)
    {
        List<int> t = new List<int>();
        for(int i = 0; i < datas.Count; i++)
        {
            t.AddRange(datas[i]);
            
        }

        return ListToString(t);
    }

    public static List<List<int>> StringToListList(string s, int first , int second)//first : 1차원 길이 second : 2차원 길이
    {
        List<List<int>> results = new List<List<int>>();

        List<int> datas = new List<int>();
        datas = s.Split(',').Select(int.Parse).ToList();

        for(int i = 0; i < first ; i++)
        {

            results.Add(datas.GetRange(i * second , second).ToList());
            
        }

        return results;

    }

}
