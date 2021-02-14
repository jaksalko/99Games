using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ButtonManager_MyInfo : MonoBehaviour
{
	public GameObject skinInfo;
	public GameObject skinContents;
	public GameObject skinPreview;

	public GameObject skinBtnObj;

	private int selectSkinNum;
	private List<int> skinList = new List<int>();

	public void Start()
	{
		StartCoroutine(SkinList());
	}

	public void Update()
	{
		
	}

	public void PressBackBtn()
	{
		SceneManager.LoadScene("MainScene");
	}

	public void PressPlayer1Btn()
	{
		ResetContents(skinContents);
		GetContents("playerskin");
	}

	public void PressPlayer2Btn()
	{
		ResetContents(skinContents);
		GetContents("playerskin");
	}

	public void PressSkinBtn()
	{
		int index = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
		selectSkinNum = index;

		List<Dictionary<string, object>> playerskin = CSVReader.Read("playerskin");
		skinInfo.transform.GetChild(0).GetComponent<Text>().text = playerskin[index]["name"].ToString();
		skinInfo.transform.GetChild(1).GetComponent<Text>().text = playerskin[index]["information"].ToString();
		string location = playerskin[index]["location"].ToString();
		Debug.Log(location);
		Material skinmat = Resources.Load<Material>(location);
		skinPreview.GetComponent<SkinnedMeshRenderer>().material = skinmat;
	}

	public void PressApplyBtn()
	{

		/*
		List<Dictionary<string, object>> skin = CSVReader.Read("playerskin");

		for (var i = 0; i < skin.Count; i++)
		{
			Debug.Log("index " + (i).ToString() + " : " + skin[i]["id"] + " " + skin[i]["name"] + " " + skin[i]["grade"]);
		}
		*/
	}

	private void GetContents(string filename)
	{
		List<Dictionary<string, object>> skin = CSVReader.Read(filename);

		for (var i = 0; i < skin.Count; i++)
		{
			GameObject skinBtn = Instantiate(skinBtnObj, new Vector3(0, 0, 0), Quaternion.identity);
			skinBtn.transform.SetParent(skinContents.transform);
			skinBtn.transform.GetComponentInChildren<Text>().text = skin[i]["name"].ToString();

			int skinid = int.Parse(skin[i]["id"].ToString());
			if (skinList.Exists(x => x==skinid))
			{
				skinBtn.GetComponent<Button>().onClick.AddListener(() => PressSkinBtn());
			}
			else
			{
				skinBtn.GetComponent<Button>().interactable = false;
			}
		}
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
				if(results[i] != "null")
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
