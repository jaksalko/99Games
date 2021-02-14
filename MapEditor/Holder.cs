using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{

    public void ClearHolder()
    {
        int childCount = transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void GenerateChild()
    {

    }

    public void SetParent(GameObject obj)
    {
        obj.transform.position += transform.position;
        obj.transform.SetParent(transform);
    }
}
