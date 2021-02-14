using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using System;

public class Indexer : MonoBehaviour
{
    public int data;
    public MeshRenderer myRenderer;

    public Material default_material;
    public Material transparent_material;
    [SerializeField]int x; public int X { get { return x; } set { x = value; } }
    [SerializeField]int z; public int Z { get { return z; } set { z = value; } }
    [SerializeField]int floor; public int Floor { get { return floor; }
        set
        {
            floor = value;
            if(floor == 0)
                transform.localPosition = new Vector3(transform.position.x, floor, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x, floor, transform.position.z);
        }
    }
    public bool isFull;

    private void Awake()
    {
        this.ObserveEveryValueChanged(_ => floor).
            Subscribe(d => ChangeMaterial());
    }
    void ChangeMaterial()
    {
        if(floor == 0)
        {
            myRenderer.material = default_material;
        }
        else
        {
            myRenderer.material = transparent_material;
        }
    }
    public void SetXZ(int x, int z)//initialized data is "obstacle block"
    {
        data = BlockNumber.broken;
        floor = 0;
        isFull = false;
        this.x = x;
        this.z = z;
    }



}
