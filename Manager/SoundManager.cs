using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
	public static SoundManager instance = null;

	public AudioClip popupSound;

	

	public AudioClip[] BGM;
	AudioSource audioSource;
	public AudioMixer bgmMixer;
	public AudioMixer sfxMixer;

	void Awake()
	{
		
		if(instance == null)
        {
            Debug.Log("Single instance is null");
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Single instance is not Single.. Destroy gameobject!");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);//Dont destroy this singleton gameobject :(
		
		audioSource = gameObject.GetComponent<AudioSource>();
		
		
	}

    private void Start()
    {
		float bgmValue = Mathf.Log10(PlayerPrefs.GetFloat("BGM", 1)) * 20;
		float sfxValue = Mathf.Log10(PlayerPrefs.GetFloat("SFX", 1)) * 20;

		bgmMixer.SetFloat("bgmValue", bgmValue);
		sfxMixer.SetFloat("sfxValue", sfxValue);
	}
    // Start is called before the first frame update


    public void ChangeBGM(int num)
	{

		audioSource.loop = true;
		audioSource.clip = BGM[num];
		audioSource.Play();
	}

    

	public void GameResultPopup()
	{
		audioSource.Stop();

		audioSource.loop = false;
		audioSource.clip = popupSound;

		audioSource.Play();
	}

	
}
