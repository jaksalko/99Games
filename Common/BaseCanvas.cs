using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class BaseCanvas : MonoBehaviour
{
	public Slider bgmSlider;
	public Slider sfxSlider;
	public GameObject userState;
	public static BaseCanvas Instance;
	public float btnlocX;
	public float btnlocY;

	[SerializeField]
	private GameObject changePlayerBtn;

	[Header("INFO")]
	public Text boong_text;
	public Text heart_text;
	public Text heartTime_text;

	public GameObject friendManage;

	AWSManager awsManager = AWSManager.instance;
    GameManager gameManager = GameManager.instance;
	XMLManager xmlManager = XMLManager.ins;
	void Awake()
    {
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		
		Instance = this;
		DontDestroyOnLoad(gameObject);


		//StartCoroutine(UpdateInfo());

	}

	private void Start()
	{
		bgmSlider.value = PlayerPrefs.GetFloat("bgmVolumn", 1);
		sfxSlider.value = PlayerPrefs.GetFloat("sfxVolumn", 1);


        awsManager = AWSManager.instance;
        gameManager = GameManager.instance;
		xmlManager = XMLManager.ins;
			
		
		//Debug.Log(awsManager.user.nickname + " :  " + awsManager.user.boong +","+ awsManager.user.heart);
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex >= 4)
		{
			changePlayerBtn = GameObject.FindGameObjectWithTag("ChangePlayer");
			userState.SetActive(false);
		}
		else
		{
			userState.SetActive(true);
		}

		boong_text.text = awsManager.userInfo.boong.ToString();
		heart_text.text = awsManager.userInfo.heart + "/5";
		heartTime_text.text = IntToTimerString();
	}

	string IntToTimerString()
	{
		string time_string = "";
		int heart_time = awsManager.userInfo.heart_time;
		int min = 0;
		int sec = 0;
		while(heart_time != 0)
		{
			if(heart_time >= 60)
			{
				heart_time -= 60;
				min++;
			}
			else
			{
				sec = heart_time;
				heart_time = 0;
			}
		}
		time_string = min + ":" + sec;
		return time_string;
	}

	public void InitLocBtnClick()
	{
		PlayerPrefs.SetFloat("ChangePlayerBtnX", btnlocX);
		PlayerPrefs.SetFloat("ChangePlayerBtnY", btnlocY);

		if(changePlayerBtn != null)
		{
			RectTransform rect = changePlayerBtn.GetComponent<RectTransform>();
			rect.position = new Vector2(btnlocX, btnlocY);
		}
	}

	public void SaveBGMVolumn()
	{
		PlayerPrefs.SetFloat("bgmVolumn", bgmSlider.value);
	}

	public void SaveSFXVolumn()
	{
		PlayerPrefs.SetFloat("sfxVolumn", sfxSlider.value);
	}

    public void FriendManageOpen()
    {
		friendManage.SetActive(true);
    }
}
