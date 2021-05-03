using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParfaitBlock : Block
{
    public Animator iceBox;
    public ParticleSystem[] activeParticle;//parfait light

    

    public enum State
    {
        inactive,
        active,
        clear
        
    }
    public int sequence;
    public State state;
    
	public AudioClip meltSound;
    public override void Init(int block_num,int style)
    {
        base.Init(block_num,style);

        int parfait_num = block_num % 10 - 1;
        if(GameController.instance != null)
            GameController.instance.mapLoader.parfaitBlock[parfait_num] = this;
        object_styles[parfait_num].SetActive(true);

        state = State.inactive;
        switch(block_num)
        {
            case BlockNumber.parfaitA:
            case BlockNumber.upperParfaitA:
                sequence = 0;
                Activate();
                break;

            case BlockNumber.parfaitB:
            case BlockNumber.upperParfaitB:
                sequence = 1;
                break;
            case BlockNumber.parfaitC:
            case BlockNumber.upperParfaitC:
                sequence = 2;
                break;
            case BlockNumber.parfaitD:
            case BlockNumber.upperParfaitD:
                sequence = 3;
                break;



        }
    }
    

    
    public void ActiveNextParfait()
    {
		
        ClearParfait();

        if(sequence < 3)
        {
            ParfaitBlock nextParfaitBlock = GameController.instance.mapLoader.parfaitBlock[sequence + 1];
            nextParfaitBlock.Activate();           
        }

        
        // Destroy(gameObject);
    }

    

	public void ClearParfait()
    {
        state = State.clear;
        object_styles[sequence].SetActive(false);
        iceBox.gameObject.SetActive(false);
        for (int i = 0; i < activeParticle.Length; i++)
        {
            activeParticle[i].Stop();
        }

        GetComponent<BoxCollider>().enabled = false;
    }

    public void DeActivate()//얼려있는 상태로 돌아가기
    {

        state = State.inactive;
        object_styles[sequence].SetActive(true);//오브젝트가 보여야 하므로 true
        iceBox.gameObject.SetActive(true);
        iceBox.SetBool("melt", false);


        for (int i = 0; i < activeParticle.Length; i++)
        {
            activeParticle[i].Stop();
        }

        GetComponent<BoxCollider>().enabled = true;
    }

    public void Activate()//얼음이 녹은 상태로
    {
        state = State.active;
        object_styles[sequence].SetActive(true);//오브젝트가 보여야 하므로 true

        Debug.Log("activate");
        iceBox.gameObject.SetActive(true);
        iceBox.SetBool("melt", true);
        //GetComponent<BoxCollider>().enabled = true;
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().clip = meltSound;
            GetComponent<AudioSource>().Play();
        }

        for (int i = 0; i < activeParticle.Length; i++)
        {
            activeParticle[i].Play();
        }

        GetComponent<BoxCollider>().enabled = true;

    }
}
