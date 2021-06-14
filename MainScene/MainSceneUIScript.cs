using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneUIScript : UIScript
{
	class StartStage
    {
		public UserInfo userInfo;
		public UserHistory userHistory;

		public StartStage(UserInfo i , UserHistory h)
        {
			userInfo = i;
			userHistory = h;
        }
    }
	public GameObject EditorPlayPopup;

	public Button playButton;
	public Button profileButton;
	public Button editorLobbyButton;
	public GameObject[] lowerButtons;


	public RectTransform islandDesign;

	
	SoundManager soundManager = SoundManager.instance;
	public TutorialManager tutorialManager;

	public GameObject heartAnimation;
	public Image fader;
	public float fadeOutTime;

	public GameObject settingPopup;

	[Header("Island Text")]
	public Text boong_text;
	public Text heart_text;
	public Text heartTime_text;

	

	bool isAction = false;
	bool isEditor = false;
	bool editorMake = false;

	public ProfilePopup profilePopup;
	public Gatcha gatcha;
	public Store store;
	public MyIgloo myIgloo;

	public GameObject comingsoonPanel;
	public GameObject snowEffect;
	private void Start()
	{
		
		soundManager = SoundManager.instance;
		//soundManager.ChangeBGM(0);

		int highLevel = awsManager.userInfo.stage_current;


		int island_num = Island_Name(highLevel);
		//islandList[island_num].SetActive(true);
		islandDesign.localPosition += Vector3.right * (-1080) * island_num;




		if (PlayerPrefs.GetInt("tutorial", 0) == 0)//처음
		{
			tutorialManager.StartTutorial();
		}
		else if (PlayerPrefs.GetInt("tutorial", 0) == 1)//튜토리얼 스테이지 끝
		{
			tutorialManager.StartTutorial();
		}
	}

	private void Update()
	{
		boong_text.text = awsManager.userInfo.boong.ToString();
		if(awsManager.userInfo.heart > 5)
        {
			
			heart_text.text = "<color=#e34949>"+awsManager.userInfo.heart+ "</color>" + "/5";
		}
		else
        {
			heart_text.text = awsManager.userInfo.heart + "/5";
		}
		
		
		heartTime_text.text = IntToTimerString();

		if (awsManager.userInfo.heart <= 0)
			playButton.interactable = false;
		else
			playButton.interactable = true;
	}

	string IntToTimerString()
	{
		string time_string = "";
		int heart_time = awsManager.userInfo.heart_time;
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


	
	
	public void EditorPlayIslandButtonClicked(bool make)
    {
		editorMake = make;
		StartCoroutine(EditorModeChange());
	}
	
	public void EditorMakeIslandButtonClicked()
    {
		SceneManager.LoadScene("MapEditor");
		//StartCoroutine(EditorModeChange());
	}
	IEnumerator EditorModeChange()
    {
		
		Vector3 targetPosition;

		profileButton.gameObject.SetActive(!editorMake);
		editorLobbyButton.gameObject.SetActive(editorMake);

		if (editorMake)
        {
			foreach(GameObject button in lowerButtons)
            {
				button.SetActive(false);
            }
			targetPosition  = Vector3.right * (-1080) * 6;

			if(PlayerPrefs.GetInt("editorPlay", 0) == 0)
            {
				tutorialManager.StartTutorial();
            }
		}
		else
        {
			foreach (GameObject button in lowerButtons)
			{
				button.SetActive(true);
			}
			targetPosition = Vector3.right * (-1080) * 5;
		}

		while(islandDesign.localPosition != targetPosition)
        {

			islandDesign.localPosition = Vector3.Lerp(islandDesign.localPosition,targetPosition,Time.deltaTime * 10);

			if (Mathf.Abs(islandDesign.localPosition.x - targetPosition.x) <= 1)
				islandDesign.localPosition = targetPosition;
			yield return null;
		}
		Debug.Log("arrive");
		
		yield break;
	}
	public void ChangeGameContentButtonClicked()//Island <-> Editor 
	{
		if(!isAction)
        {
			isEditor = !isEditor;
			StartCoroutine(ChangeGameContentAction());
		}
			

	}
	IEnumerator ChangeGameContentAction()
    {
		isAction = true;

		playButton.gameObject.SetActive(!isEditor);//에디터 모드 일 때 안보임
		profileButton.gameObject.SetActive(true);//에디터 모드 , 일반 모드 항상 보임
		editorLobbyButton.gameObject.SetActive(false);//항상 안보임
		foreach (GameObject button in lowerButtons)
		{
			button.SetActive(true);
		}

		Vector3 targetPosition;
		

		if(isEditor)
		{
			targetPosition = Vector3.right * (-1080) * 5;
			snowEffect.SetActive(false);


			if (PlayerPrefs.GetInt("editorLobby", 0) == 0)
			{
				tutorialManager.StartTutorial();
			}

		}
		else
        {
			snowEffect.SetActive(true);
			editorMake = false;
			targetPosition = Vector3.right * (-1080) * Island_Name(awsManager.userInfo.stage_current);
		}

		islandDesign.localPosition = targetPosition;
		/*
		while (islandDesign.localPosition != targetPosition)
		{

			islandDesign.localPosition = Vector3.Lerp(islandDesign.localPosition, targetPosition, Time.deltaTime * 10);

			if (Mathf.Abs(islandDesign.localPosition.x - targetPosition.x) <= 1)
				islandDesign.localPosition = targetPosition;
			yield return null;
		}

		*/

		isAction = false;
		yield break;

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
	public void ProfileButtonClicked()
    {
		profilePopup.Activate();
    }
	public void MyEglooButtonClicked()
	{
		gatcha.gameObject.SetActive(false);
		store.gameObject.SetActive(false);
		myIgloo.gameObject.SetActive(true);
		//StartCoroutine(comingsoonPanelActivate());
	}

	public void StoreButtonClicked()
	{
		//StartCoroutine(comingsoonPanelActivate());
		
		foreach (GameObject button in lowerButtons)
		{
			button.SetActive(true);
		}
		myIgloo.gameObject.SetActive(false);
		gatcha.gameObject.SetActive(false);
		store.gameObject.SetActive(true);
		
	}

	public void GatchaButtonClicked()
    {
		StartCoroutine(comingsoonPanelActivate());
		/*
		foreach (GameObject button in lowerButtons)
		{
			button.SetActive(true);
		}

		store.gameObject.SetActive(false);
		gatcha.gameObject.SetActive(true);
		*/
	}
	IEnumerator comingsoonPanelActivate()
    {
		comingsoonPanel.SetActive(true);
		float t = 0;
		while(t < 2)
        {
			t += Time.deltaTime;
			yield return null;
        }

		comingsoonPanel.SetActive(false);

		yield break;
    }
	public void PressIslandBtn()
	{
		SceneManager.LoadScene("LevelScene");
	}

	public void PressPlayBtn()
	{
		UserInfo copyInfo = awsManager.userInfo.DeepCopy();
		UserHistory copyHistory = awsManager.userHistory.DeepCopy();
		StartStage startStage = new StartStage(copyInfo, copyHistory);
		copyInfo.heart--;
		copyHistory.heart_use++;
		jsonAdapter.UpdateData(startStage, "infoHistory", PlayButtonCallback);
		
	}

	void PlayButtonCallback(bool success)
	{
		if (success)
		{
			if (jsonAdapter.EndLoading())
			{
				jsonAdapter.GetAllUserData(awsManager.userInfo.nickname,StartGame);

				
			}
		}
		else
		{
			
		}
	}

	void StartGame(bool success)
    {
		if (success)
		{
			if (jsonAdapter.EndLoading())
			{
				GameManager.instance.nowLevel = awsManager.userInfo.stage_current;
				if (PlayerPrefs.GetInt("tutorial") == 0)
				{
					GameManager.instance.nowLevel = 0;

				}

				StartCoroutine(GameStart());
			}
		}
		else
		{
			
		}
	}
}
