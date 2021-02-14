using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneUIScript : UIScript
{
	
    public GameObject EditorPlayPopup;
    public GameObject EditorSettingPopup;

	public GameObject[] islandList;
	public RectTransform backgroundImage;
	int maxLevel = IslandData.lastLevel;
	XMLManager xmlManager = XMLManager.ins;
	SoundManager soundManager = SoundManager.instance;
	public GameObject tutorialManager;

	public GameObject heartAnimation;
	public Image fader;
	public float fadeOutTime;

	public GameObject settingPopup;

	public Text boong_text;
	public Text heart_text;
	public Text heartTime_text;

	private void Start()
	{
		xmlManager = XMLManager.ins;
		soundManager = SoundManager.instance;
		soundManager.ChangeBGM(0);
        
		int highLevel = xmlManager.itemDB.user.current_stage;

		
			int island_num = Island_Name(highLevel);
			islandList[island_num].SetActive(true);
			backgroundImage.localPosition += Vector3.right*(-1080)*island_num;
		
		
			
		
		if(PlayerPrefs.GetInt("tutorial",0) == 0)//처음
		{
			tutorialManager.SetActive(true);
		}
		else if(PlayerPrefs.GetInt("tutorial",0) == 1)//튜토리얼 스테이지 끝
		{
			tutorialManager.SetActive(true);
		}
	}

    private void Update()
    {
		boong_text.text = xmlManager.itemDB.user.boong.ToString();
		heart_text.text = xmlManager.itemDB.user.heart + "/5";
		heartTime_text.text = IntToTimerString();
	}

	string IntToTimerString()
	{
		string time_string = "";
		int heart_time = xmlManager.itemDB.user.heart_time;
		int min = 0;
		int sec = 0;
		while (heart_time != 0)
		{
			if (heart_time >= 60)
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


	public void PressIslandBtn()
	{
		SceneManager.LoadScene("LevelScene");
	}

	public void PressPlayBtn()
	{
		
		GameManager.instance.nowLevel = GameManager.instance.userInfo.current_stage;
		StartCoroutine(GameStart());

		
	}

	public void PressEglooBtn()
	{
		SceneManager.LoadScene("MyInfoScene");
	}

	public void PressStoreBtn()
	{
		SceneManager.LoadScene("StoreScene");
	}

    public void EditorPlayBtn()
    {
		//추후에 난이도 설정
		//EditorPlayPopup.SetActive(true);
		StartCoroutine(GameManager.instance.LoadCustomMapList(success =>
		{
			if (success)
			{
				EditorPlayPopup.SetActive(true);
				
			}
			else
			{
				Debug.Log("cant load custom map list...");
			}
		}));

		
		//SceneManager.LoadScene("CustomMapPlayScene");
	}

    public void EditorBtn()
    {
        //맵크기설정
        EditorSettingPopup.SetActive(true);
        //SceneManager.LoadScene("MapEditor");
    }

	IEnumerator GameStart()
    {
		float t = 0;
		Color c = fader.color;

		heartAnimation.SetActive(true);
		fader.gameObject.SetActive(true);

		while(t <= 1)
        {
			t += Time.deltaTime / fadeOutTime;
			c.a = t;
			fader.color = c;

			yield return null;
        }

		Load_Island(GameManager.instance.nowLevel);
		yield break;
	}

	public void SettingButtonClicked()
    {
		settingPopup.SetActive(true);
    }
}
