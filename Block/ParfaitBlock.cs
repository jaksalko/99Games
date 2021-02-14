using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParfaitBlock : Block
{
    public Animator iceBox;
    public ParticleSystem[] activeParticle;

    

    public enum State
    {
        inactive,
        active,
        clear
        
    }
    public int sequence;
    public State state;

	public AudioClip meltSound;
    public override void Init(int block_num)
    {
        base.Init(block_num);
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
    

    public void Activate()
    {
        Debug.Log("activate");
        state = State.active;
        iceBox.SetBool("melt", true);

		if (!GetComponent<AudioSource>().isPlaying)
		{
			GetComponent<AudioSource>().clip = meltSound;
			GetComponent<AudioSource>().Play();
		}

		for (int i = 0; i < activeParticle.Length; i++)
        {
            activeParticle[i].Play();
        }
        //iceBox.SetActive(false);
        //renderer.material.color = Color.white;// reveal real color


    }
    public void ActiveNextParfait()
    {
		Debug.Log("...active next parfait");
		state = State.clear;

        if(sequence < 3)
        {
            ParfaitBlock nextParfaitBlock = GameController.instance.mapLoader.parfaitBlock[sequence + 1];
            nextParfaitBlock.Activate();           
        }
		gameObject.transform.GetChild(0).gameObject.SetActive(false);
		// Destroy(gameObject);
	}

	public bool GetParfait(MapLoader map)
    {
        state = State.clear;

		if (sequence < 3)
        {
            map.parfaitBlock[sequence + 1].Activate();
			gameObject.transform.GetChild(0).gameObject.SetActive(false);
			// Destroy(gameObject);
            return false;//active next parfait
        }
        else
        {
			gameObject.transform.GetChild(0).gameObject.SetActive(false);
            // Destroy(this.gameObject);
            return true;//clear game
        }
            
       
        //Start Get Animation...
        

        
    }

	public void Deactivate()
	{
		Debug.Log(gameObject.name + "deactivate");

		iceBox.gameObject.SetActive(true);
		iceBox.SetBool("melt", false);

		state = State.inactive;

		for (int i = 0; i < activeParticle.Length; i++)
		{
			activeParticle[i].Stop();
		}
	}
}
