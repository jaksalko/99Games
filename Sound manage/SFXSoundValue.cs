using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXSoundValue : MonoBehaviour
{
	AudioSource audioSource;
	Slider sfXVolumnSlider;

    // Start is called before the first frame update
    void Start()
    {
		//Debug.Log(name);
		//audioSource = gameObject.GetComponent<AudioSource>();
		//sfXVolumnSlider = GameObject.FindGameObjectWithTag("BaseCanvas").transform.GetChild(1).transform.Find("SFX Slider").GetComponent<Slider>();
	}

    // Update is called once per frame
    void Update()
    {
		//audioSource.volume = sfXVolumnSlider.value;
	}
}
