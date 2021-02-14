using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CloudBlock : Block
{
    public static event Action<GameObject> Exit;

    public int direction; // 0 상 1 우 2 하 3 좌
    Vector2 pos_;
    public override void Init(int block_num)
    {
        base.Init(block_num);
        direction = (block_num % 10) - 1;//block num 11-14 21-24
        pos_ = new Vector2(transform.position.x, transform.position.z);
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (other.gameObject.CompareTag("Player"))
        {
            Player onCloud_player = other.gameObject.GetComponent<Player>();
            Vector2 player_pos = new Vector2(onCloud_player.transform.position.x, onCloud_player.transform.position.z);
            float distance = Vector2.Distance(pos_, player_pos);
            if(direction != onCloud_player.direction && distance < 0.25f)
            {
                onCloud_player.direction = direction;
                onCloud_player.transform.position = new Vector3(pos_.x, onCloud_player.transform.position.y, pos_.y);
                onCloud_player.transform.rotation = Quaternion.Euler(new Vector3(0f, direction * 90, 0f));
                Debug.Log("move to : " + onCloud_player.transform.position);
                //Time.timeScale = 0;
            }
        }*/
    }

    /*private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Player onCloud_player = other.gameObject.GetComponent<Player>();
            if(!onCloud_player.onCloud && !onCloud_player.Moving() && onCloud_player.isActive)//call only one time
            {
                Debug.Log("ride cloud" + Data);
                onCloud_player.onCloud = true;
                //StartCoroutine(onCloud_player.MoveByCloud(data));
                onCloud_player.MoveByCloud(Data);
                
            }

            //if (onCloud_player.onCloud && !onCloud_player.isMoving && !onCloud_player.isActive)
            //{
            //    onCloud_player.MoveByCloud(data);
            //}



        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player onCloud_player = other.gameObject.GetComponent<Player>();
            Debug.Log("exit cloud");
            if(onCloud_player.isActive)
                onCloud_player.onCloud = false;

            
            Exit?.Invoke(onCloud_player.gameObject);
        }
    }*/
}
