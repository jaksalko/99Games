using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour
{
    public ParticleSystem particle;

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.CompareTag("Leg") || other.gameObject.CompareTag("Player"))
        {
            Debug.Log("clear");
            if(particle != null)
                particle.Play();
			gameObject.SetActive(false);
			// Destroy(gameObject);
        }
    }
	
}
