using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance = null;

	public AudioClip popupSound;

	public Slider bgmSlider;
	public Slider sfxSlider;

	public AudioClip[] BGM;
	AudioSource audioSource;
	

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

	// Start is called before the first frame update
	void Start()
    {
		bgmSlider.value = PlayerPrefs.GetFloat("bgmVolumn", 1);
		sfxSlider.value = PlayerPrefs.GetFloat("sfxVolumn", 1);
	}

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

	public void BGMSliderOnValueChanged()
	{
		PlayerPrefs.SetFloat("bgmVolumn", bgmSlider.value);
		audioSource.volume = bgmSlider.value;
	}

	public void SFXSliderOnValueChanged()
	{
		PlayerPrefs.SetFloat("sfxVolumn", sfxSlider.value);
		//
	}
}
