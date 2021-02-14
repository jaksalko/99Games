using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager_Level : UIScript
{
	public GameObject islandList;
	public GameObject islandListContents;
	public List<string> isLandNameList;
	public GameObject[] levelList;
	public Sprite clearBtn;
	public Sprite nonclearBtn;
	public Sprite clearSelect;
	public Sprite nonclearSelect;
	private int highLevel;
	public GameObject tutorialManager;

	public RectTransform content; //2160~-2160
	public Transform[] stageScrollView;
	public LevelButton buttonPrefab;
	XMLManager xmlManager = XMLManager.ins;

	public StagePopup stagePopup;
	// Start is called before the first frame update
	void Start()
    {

		xmlManager = XMLManager.ins;
		highLevel = gameManager.userInfo.current_stage;

		content.localPosition = new Vector3(2160 - 1080*IslandData.Island_Num(highLevel),0,0);

		Debug.Log("high level : " + highLevel);

		for(int i = 0 ; i <= highLevel ; i++)
		{
			LevelButton newButton = Instantiate(buttonPrefab,default);

			newButton.SetStarImage(xmlManager.itemDB.user.star_list[i]);
			newButton.moveCount.text = "걸음 수 " + xmlManager.itemDB.user.move_list[i];
			newButton.stage = i;
			newButton.StageText();

			

			newButton.transform.SetParent(stageScrollView[newButton.island]);
		}

		for (int l = highLevel + 1 ; l <= IslandData.lastLevel ; l++)
        {
			LevelButton newButton = Instantiate(buttonPrefab,default);
			newButton.SetStarImage(0);
			newButton.moveCount.text = "걸음 수 99";
			newButton.stage = l;
			newButton.StageText();
			newButton.transform.SetParent(stageScrollView[newButton.island]);
			newButton.gameObject.GetComponent<Button>().interactable = false;
        }

		if(PlayerPrefs.GetInt("tutorial",0) == 1)
		{
			tutorialManager.SetActive(true);
		}


	}

	public void PressBackBtn()
	{
		SceneManager.LoadScene("MainScene");
	}

	


}
