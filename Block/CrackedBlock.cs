using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackedBlock : Block
{   
	public int count;
	public int x;
    public int z;

    public Material transparentMaterial;
	public Material crackerMaterial;


	public MeshRenderer[] crackerRenderer;
    public MeshFilter[] crackerMesh;
	public Mesh cracker1;
    public Mesh cracker2;
    public Mesh cracker3;

    public ParticleSystem cracker_particle;

    public AudioClip[] crackerSound;
    public AudioSource audioSource;
    public Transform crackerDebris;

    public override void Init(int block_num,int style)
    {
        base.Init(block_num,style);
        object_styles[style].SetActive(true);

        x = (int)transform.position.x;
        z = (int)transform.position.z;
        count = 0;
		// count = Cracked;
        if(block_num < BlockNumber.upperNormal)
        {
            count = block_num - BlockNumber.cracker_0;
        }
        else
        {
            count = block_num - BlockNumber.upperCracker_0;
        }

        if (count == 1)//1
        {
            for (int i = 0; i < crackerMesh.Length; i++)
            {
                crackerMesh[i].mesh = cracker2;
            }
        }
        else if (count == 2)//2
        {

            crackerMesh[5].mesh = cracker3;
        }
        else if(count == 3)//broken
        {
           
            for (int i = 0; i < crackerRenderer.Length; i++)
            {
                crackerRenderer[i].material = transparentMaterial;
                
            }
        }
        

    }

    

    private void OnTriggerEnter(Collider other)
    {
        cracker_particle.Play();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Leg"))
            Debug.Log("stay");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Leg"))
        {
            audioSource.clip = crackerSound[count];
            audioSource.Play();
            count++;
            data++;
			// Cracked = count;
			Debug.Log("through the cracked block :" + count);
            if(count == 1)
            {
                
                for(int i = 0; i < crackerMesh.Length; i++)
                {
                    crackerMesh[i].mesh = cracker2;
                }
                
            }
            else if(count == 2)
            {
                
                crackerMesh[5].mesh = cracker3;
            }
            else if(count == 3)
            {
                
                for(int i = 0; i < crackerRenderer.Length; i++)
                {
                    crackerRenderer[i].material = transparentMaterial;
                }

                crackerDebris.gameObject.SetActive(true);
            }
            
        }
    }

    

	public void SetMaterial(int count)
	{
		for (int i = 0; i < crackerRenderer.Length; i++)
		{
			crackerRenderer[i].material = crackerMaterial;
		}

		if (count == 0)
		{
			for (int i = 0; i < crackerMesh.Length; i++)
			{

				crackerMesh[i].mesh = cracker1;
			}
		}
		else if (count == 1)
		{
			for (int i = 0; i < crackerMesh.Length; i++)
			{

				crackerMesh[i].mesh = cracker2;
			}
		}
		else if (count == 2)
		{
			crackerMesh[5].mesh = cracker3;
		}
		
	}

}
