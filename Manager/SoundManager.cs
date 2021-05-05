using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance = null;

	

	

	public AudioClip[] BGM;
	public AudioSource audioSource;
	public AudioMixer bgmMixer;
	public AudioMixer sfxMixer;

	int scene_number;

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
		
		
		SceneManager.sceneLoaded += OnSceneLoaded;

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
		if(BGM[num] != BGM[scene_number])
        {
			StartCoroutine(FadeInVolume());
			audioSource.clip = BGM[num];
			audioSource.loop = true;
			audioSource.Play();
		}
		
	}

    

	public void Mute()
	{
		audioSource.mute = true;
	}


	IEnumerator FadeInVolume()
    {
		audioSource.mute = false;
		float t = 0;

		while(t <= 1)
        {
			t += Time.deltaTime * 0.3f;
			audioSource.volume = t;

			yield return null;
        }

		audioSource.volume = 1;
		yield break;
    }
	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		

		int scene_change = scene.buildIndex;

		if(scene_change == scene_number)
			StartCoroutine(FadeInVolume());

		ChangeBGM(scene_change);
		scene_number = scene_change;
	}
}
