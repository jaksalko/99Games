using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;
using System;

public class Indexer : MonoBehaviour
{
    public int data;
    public bool placeable;
    public MeshRenderer myRenderer;
    public MeshRenderer placeableRenderer;

    public Material default_material;
    public Material transparent_material;
    public Material gray_material;
    public Material white_material;

    public Material placeable_material;
    public Material unplaceable_material;

    public float ray_time = 0;

    public List<Block> blocks;


    [SerializeField]int x; public int X { get { return x; } set { x = value; } }
    [SerializeField]int z; public int Z { get { return z; } set { z = value; } }
    [SerializeField]int floor;
    public int Floor {
        get
        {
            return floor;
        }
        set
        {
            Debug.Log("floor changed : " + floor);
            floor = value;
            
            transform.position = new Vector3(transform.position.x, floor, transform.position.z);

            if (data >= BlockNumber.character)
            {
                placeableRenderer.GetComponent<Transform>().position = transform.position - Vector3.up;
            }
            else
            {
                placeableRenderer.GetComponent<Transform>().position = transform.position - Vector3.up * 0.001f;
            }
        }
    }
    public bool isFull;

    private void Awake()
    {
        /*
        this.ObserveEveryValueChanged(_ => floor).
            Subscribe(d => ChangeMaterial());
            //Subscribe(d => CheckPlaceableIndex(new List<int>(){floor}));
        */

        this.ObserveEveryValueChanged(_ => placeable).
            Subscribe(p => ChangeMaterial());

        this.ObserveEveryValueChanged(_ => floor).
            Subscribe(p => ChangeMaterial());
    }
    void ChangeMaterial()
    {
        
        placeableRenderer.material = placeable ? placeable_material : unplaceable_material;
        myRenderer.material = floor == 0 ? default_material : transparent_material;
        
        
    }

    //call by (MapGenerator)select block, add block, erase block
    public void CheckPlaceableIndex(List<int> block_floor)
    {
        
        for (int i = 0; i < block_floor.Count; i++)
        {
            if(block_floor[i] == floor + 1 && !isFull)
            {
                placeableRenderer.material = placeable_material;
                placeable = true;

                return;
            }
        }

        placeableRenderer.material = unplaceable_material;
        placeable = false;

        
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

    public (int,int) MoveBlock()
    {
        Block erasedBlock = blocks[blocks.Count - 1];
        (int, int) blockData = (erasedBlock.data, erasedBlock.style);
        blocks.RemoveAt(blocks.Count - 1);
        Destroy(erasedBlock.gameObject);

        if (blocks.Count == 0)
        {
            data = BlockNumber.broken;
        }
        else
        {
            data = blocks[blocks.Count - 1].data;
        }

        Floor = blocks.Count;
        isFull = false; // 지웠으므로 항상 채울 수 있음.

        return blockData;
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

    public void EraseBlock()
    {
        Block erasedBlock = blocks[blocks.Count - 1];
        blocks.RemoveAt(blocks.Count - 1);
        Destroy(erasedBlock.gameObject);

        if (blocks.Count == 0)
        {
            data = BlockNumber.broken;
        }
        else
        {
            data = blocks[blocks.Count - 1].data;
        }

        Floor = blocks.Count;
        isFull = false; // 지웠으므로 항상 채울 수 있음.

    }

    public void AddBlock(Block block)
    {
        blocks.Add(block);
        data = block.data;
        Floor = blocks.Count;

        if (Floor == 1 && data != BlockNumber.normal)
            isFull = true;

        else if (Floor == 2 && data != BlockNumber.upperNormal)
            isFull = true;

        else if (Floor == 3)
            isFull = true;

        else
            isFull = false;

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
