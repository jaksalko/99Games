using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ButtonManager_Store : MonoBehaviour
{
	public GameObject skinInfo;
	public GameObject skinContents;
	public GameObject skinPreview;

	public GameObject skinBtnObj;

	public string skintype;
	private int selectSkinType;
	private int selectSkinNum;
	private List<int> skinList = new List<int>();

	public Dropdown _dropdown;

	// Use this for initialization
	public void Start()
	{
		StartCoroutine(SkinList());

		GetContents("playerskin", "id");
		skintype = "playerskin";
		selectSkinType = 1;

		_dropdown.onValueChanged.AddListener(delegate
		{
			DropdownValueChangedHandler(_dropdown);
		});
	}

	public void Update()
	{
		
	}

	public void PressBackBtn()
	{
		SceneManager.LoadScene("MainScene");
	}

	public void PressPlayerSkinBtn()
	{
		ResetContents(skinContents);
		GetContents("playerskin", "id");
		skintype = "playerskin";
		selectSkinType = 1;
	}

	public void PressBlockSkinBtn()
	{
		ResetContents(skinContents);
		GetContents("blockskin", "id");
		skintype = "blockskin";
		selectSkinType = 3;
	}

	public void PressSkinBtn()
	{
		string skinname = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
		int index = 0;

		switch(selectSkinType)
		{
			case 1:
			case 2:
				List<Dictionary<string, object>> playerskin = CSVReader.Read("playerskin");

				for(int i = 0; i < playerskin.Count; i++)
				{
					if (skinname == playerskin[i]["name"].ToString())
					{
						index = i;
						break;
					}
				}

				skinInfo.transform.GetChild(0).GetComponent<Text>().text = playerskin[index]["name"].ToString();

				int skinid = int.Parse(playerskin[index]["id"].ToString());
				if (skinList.Exists(x => x == skinid))
				{
					skinInfo.transform.GetChild(1).GetComponent<Text>().text = "이미 보유하고있는 스킨입니다.";
					skinInfo.transform.GetChild(2).GetComponent<Text>().text = "구매불가";
				}
				else
				{
					skinInfo.transform.GetChild(1).GetComponent<Text>().text = playerskin[index]["information"].ToString();
					skinInfo.transform.GetChild(2).GetComponent<Text>().text = playerskin[index]["cost"].ToString();
				}

				string location = playerskin[index]["location"].ToString();
				Material skinmat = Resources.Load<Material>(location);
				skinPreview.GetComponent<SkinnedMeshRenderer>().material = skinmat;
				break;
			case 3:
				List<Dictionary<string, object>> blockskin = CSVReader.Read("blockskin");
				skinInfo.transform.GetChild(0).GetComponent<Text>().text = blockskin[index]["name"].ToString();
				skinInfo.transform.GetChild(1).GetComponent<Text>().text = blockskin[index]["information"].ToString();
				skinInfo.transform.GetChild(2).GetComponent<Text>().text = blockskin[index]["cost"].ToString();
				break;
			default:
				Debug.Log("Non-set Skin Type Error");
				break;
		}
	}

	public void PressApplyBtn()
	{
		switch (selectSkinType)
		{
			case 1:

				break;
			case 2:

				break;
			case 3:

				break;
			default:
				Debug.Log("Non-set Skin Type Error");
				break;
		}

		/*
		List<Dictionary<string, object>> skin = CSVReader.Read("playerskin");

		for (var i = 0; i < skin.Count; i++)
		{
			Debug.Log("index " + (i).ToString() + " : " + skin[i]["id"] + " " + skin[i]["name"] + " " + skin[i]["grade"]);
		}
		*/
	}

	private void DropdownValueChangedHandler(Dropdown target)
	{

		//Debug.Log("DropdownValueChangedHandler");
		//Debug.Log("target : " + target.value);

		ResetContents(skinContents);
		
		if (target.value == 0)
		{
			GetContents(skintype, "id");
		}
		else if(target.value == 1)
		{
			GetContents(skintype, "grade");
		}
		else if(target.value == 2)
		{
			GetContents(skintype, "name");
		}

	}

	public void SetDropdownIndex(int index)
	{
		_dropdown.value = index;
	}

	private void GetContents(string filename, string sortStyle)
	{
		List<Dictionary<string, object>> skin = CSVReader.Read(filename);

		switch(sortStyle)
		{
			case "name":
				skin.Sort((x, y) => ((string)x["name"]).CompareTo((string)y["name"]));
				break;
			case "grade":
				skin.Sort((x, y) => ((string)x["grade"]).CompareTo((string)y["grade"]));
				break;
			case "id":
				skin.Sort((x, y) => ((int)x["id"]).CompareTo((int)y["id"]));
				break;
			default:
				skin.Sort((x, y) => ((int)x["id"]).CompareTo((int)y["id"]));
				Debug.Log("not define sort style (sorted by id)");
				break;
		}
		
		for (var i = 0; i < skin.Count; i++)
		{
			GameObject skinBtn = Instantiate(skinBtnObj, new Vector3(0, 0, 0), Quaternion.identity);
			skinBtn.transform.SetParent(skinContents.transform);
			skinBtn.transform.GetComponentInChildren<Text>().text = skin[i]["name"].ToString();

			skinBtn.GetComponent<Button>().onClick.AddListener(() => PressSkinBtn());
		}

		skinInfo.transform.GetChild(0).GetComponent<Text>().text = "";
		skinInfo.transform.GetChild(1).GetComponent<Text>().text = "";
		skinInfo.transform.GetChild(2).GetComponent<Text>().text = "";
	}

	private void ResetContents(GameObject obj)
	{
		Transform[] childList = obj.GetComponentsInChildren<Transform>(true);
		if (childList != null)
		{
			for (int i = 1; i < childList.Length; i++)
			{
				if (childList[i] != transform)
					Destroy(childList[i].gameObject);
			}
		}

		skinInfo.transform.GetChild(0).GetComponent<Text>().text = "";
		skinInfo.transform.GetChild(1).GetComponent<Text>().text = "";
		skinInfo.transform.GetChild(2).GetComponent<Text>().text = "";
	}

	public IEnumerator SkinList()
	{
		// UnityWebRequest www = UnityWebRequest.Get("http://ec2-15-164-219-253.ap-northeast-2.compute.amazonaws.com:3000/igloo/playerskin?userid=" + GameManager.instance.id);
		UnityWebRequest www = UnityWebRequest.Get("http://ec2-15-164-219-253.ap-northeast-2.compute.amazonaws.com:3000/igloo/playerskin?userid=test1");
		yield return www.SendWebRequest();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);
		}
		else
		{
			string result = www.downloadHandler.text;

			// Debug.Log(result);

			string pattern = @"([\W]+(skinid)+_\d\W:)|(}])";
			Regex regex = new Regex(pattern);
			result = regex.Replace(result, "%");
			// Debug.Log(result);

			string[] results;

			results = result.Split('%');
			for (int i = 1; i < results.Length - 1; i++)
			{
				// Debug.Log(results[i]);
				if (results[i] != "null")
				{
					int skin = int.Parse(results[i]);
					skinList.Add(skin);
					// Debug.Log(skin);
				}
			}
		}
		yield break;
	}
}
