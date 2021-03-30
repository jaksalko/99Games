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
    public Material gray_material;
    public Material white_material;

    public float ray_time = 0;

    public List<Block> blocks;


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
    public void Initialize(int x, int z)//initialized data is "obstacle block"
    {
        data = BlockNumber.broken;
        floor = 0;
        isFull = false;
        this.x = x;//width
        this.z = z;//height

        default_material = (x + z) % 2 == 0 ? white_material : gray_material;
        myRenderer.material = default_material;

    }

    public void EraseBlock()
    {
        Block erasedBlock = blocks[blocks.Count - 1];
        blocks.RemoveAt(blocks.Count - 1);
        Destroy(erasedBlock.gameObject);

        if(blocks.Count == 0)
        {
            data = BlockNumber.broken;
        }
        else
        {
            data = blocks[blocks.Count - 1].data;
        }

        Floor = blocks.Count;
    }

    public void RotateBlock()
    {
        Block rotateBlock = blocks[blocks.Count - 1];

        if (rotateBlock.data >= BlockNumber.slopeUp && rotateBlock.data <= BlockNumber.slopeLeft)
        {
            if (rotateBlock.data != BlockNumber.slopeLeft)
                rotateBlock.data++;
            else
                rotateBlock.data = BlockNumber.slopeUp;
        }
        else if (rotateBlock.data >= BlockNumber.cloudUp && rotateBlock.data <= BlockNumber.cloudLeft)
        {
            if (rotateBlock.data != BlockNumber.cloudLeft)
                rotateBlock.data++;
            else
                rotateBlock.data = BlockNumber.cloudUp;
        }
        else if (rotateBlock.data >= BlockNumber.upperCloudUp && rotateBlock.data <= BlockNumber.upperCloudLeft)
        {
            if (rotateBlock.data != BlockNumber.upperCloudLeft)
                rotateBlock.data++;
            else
                rotateBlock.data = BlockNumber.upperCloudUp;
        }//25 ~ 28

        rotateBlock.transform.rotation = Quaternion.Euler(new Vector3(0, 90*(rotateBlock.data - BlockNumber.slopeUp), 0));
    }

    public void AddBlock(Block block)
    {
        blocks.Add(block);
        data = block.data;
        Floor = blocks.Count;
    }

    public (int,List<int>) GetLastBlockData()
    {
        List<int> style = new List<int>() { 0,0,0};
        

        if (blocks.Count != 0)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                Debug.Log("input style : " + blocks[i].style);
                style[i] = blocks[i].style;
            }
            
            return (blocks[blocks.Count - 1].data, style);
        }
            
        else
            return (BlockNumber.broken,style);
    }

}
