using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneUIScript : UIScript
{

	public GameObject EditorPlayPopup;


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

	public RectTransform editorDesign;
	public RectTransform islandDesign;

	bool isAction = false;
	bool isEditor = false;
	bool editorMake = false;

	public ProfilePopup profilePopup;


	private void Start()
	{
		xmlManager = XMLManager.ins;
		soundManager = SoundManager.instance;
		soundManager.ChangeBGM(0);

		int highLevel = xmlManager.itemDB.user.current_stage;


		int island_num = Island_Name(highLevel);
		islandList[island_num].SetActive(true);
		backgroundImage.localPosition += Vector3.right * (-1080) * island_num;




		if (PlayerPrefs.GetInt("tutorial", 0) == 0)//처음
		{
			tutorialManager.SetActive(true);
		}
		else if (PlayerPrefs.GetInt("tutorial", 0) == 1)//튜토리얼 스테이지 끝
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

	public void EditorPlayIslandButtonClicked()
    {
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

		if(editorMake)
        {
			targetPosition = editorDesign.localPosition + (Vector3.right * 1080);
		}
		else
        {
			targetPosition = editorDesign.localPosition - (Vector3.right * 1080);
		}

		while(editorDesign.localPosition != targetPosition)
        {

			editorDesign.localPosition = Vector3.Lerp(editorDesign.localPosition,targetPosition,Time.deltaTime * 10);

			if (Mathf.Abs(editorDesign.localPosition.x - targetPosition.x) <= 1)
				editorDesign.localPosition = targetPosition;
			yield return null;
		}
		Debug.Log("arrive");
		editorMake = editorMake ? false : true;
		yield break;
	}
	public void ChangeGameContentButtonClicked()//Island <-> Editor 
	{
		if(!isAction)
			StartCoroutine(ChangeGameContentAction());

	}
	IEnumerator ChangeGameContentAction()
    {
		isAction = true;

		float time = 0;
		Vector3 editor_targetPosition;
		Vector3 island_targetPosition;

		if(isEditor)
		{
			editor_targetPosition = new Vector3(0, 1920, 0);
			island_targetPosition = new Vector3(0, 0, 0);
		}
		else
        {
			editor_targetPosition = new Vector3(0, 0, 0);
			island_targetPosition = new Vector3(0, -1920, 0);
		}
		
		while(editor_targetPosition != editorDesign.localPosition)
        {
			Debug.Log(editor_targetPosition);
			time += Time.deltaTime;
			editorDesign.localPosition = Vector3.Lerp(editorDesign.localPosition , editor_targetPosition, Time.deltaTime * 10);
			islandDesign.localPosition = Vector3.Lerp(islandDesign.localPosition, island_targetPosition, Time.deltaTime * 10);

			if (Mathf.Abs(editorDesign.localPosition.y - editor_targetPosition.y) <= 1)
				editorDesign.localPosition = editor_targetPosition;
			yield return null;
        }

		if (isEditor)
		{
			isEditor = false;
		}
		else
		{
			isEditor = true;

		}

		isAction = false;
		yield break;

    }


	public void PressEglooBtn()
	{
		SceneManager.LoadScene("MyInfoScene");
	}

	public void PressStoreBtn()
	{
		SceneManager.LoadScene("StoreScene");
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
		profilePopup.gameObject.SetActive(true);
    }
}
