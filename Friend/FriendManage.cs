using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FriendManage : UIScript
{
    public InputField friend_nickname;
    JsonAdapter jsonAdapter = new JsonAdapter();

	public RequestList requestItem;
	public FriendList friendItem;

	public Transform requestContent;
	public Transform friendContent;

	public ChatRoom chatroom;

	int friend_count = 0;
    private void Awake()
    {
		StartCoroutine(GetFriendList());
		StartCoroutine(GetFriendRequest());

		RequestList.Update += UpdateList;
    }

    public void SendButtonClicked()
    {
        //send request to friend;
        FriendRequest friendRequest = new FriendRequest(GameManager.instance.id, friend_nickname.text);
        var json = JsonUtility.ToJson(friendRequest);
		StartCoroutine(jsonAdapter.API_POST("friend/send", json , callback => { if (callback != null) UpdateList(); }));
    }
   
    void UpdateList()
    {
		StartCoroutine(GetFriendList());
		StartCoroutine(GetFriendRequest());
	}
    public IEnumerator GetFriendRequest()
    {

		while (true)
		{
			UnityWebRequest www = UnityWebRequest.Get(PrivateData.ec2+ "friend/request?id=" + GameManager.instance.id);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log(www.error);
			}
			else
			{
				// Show results as text
				Debug.Log(www.downloadHandler.text);



				string fixdata = JsonHelper.fixJson(www.downloadHandler.text);
				Debug.Log(fixdata);

				FriendRequest[] datas = JsonHelper.FromJson<FriendRequest>(fixdata);

				Debug.Log("request count : " + datas.Length);

				foreach (Transform child in requestContent)
				{
					Destroy(child.gameObject);
				}


				for (int i = 0; i < datas.Length; i++)
				{
					var item = Instantiate(requestItem, default, Quaternion.identity);
					item.sender_name.text = datas[i].id;
					item.transform.SetParent(requestContent);
				}



			}

			yield return new WaitForSeconds(3f);
		}

		

	}

	public IEnumerator GetFriendList()
	{
		

			UnityWebRequest www = UnityWebRequest.Get(PrivateData.ec2 + "friend/list?id=" + GameManager.instance.id);
		    yield return www.SendWebRequest();

		    if (www.isNetworkError || www.isHttpError)
		    {
			    Debug.Log(www.error);
		    }
		    else
		    {
			    // Show results as text
			    Debug.Log(www.downloadHandler.text);



			    string fixdata = JsonHelper.fixJson(www.downloadHandler.text);
			    Debug.Log(fixdata);

			    FriendRequest[] datas = JsonHelper.FromJson<FriendRequest>(fixdata);
			    Debug.Log("friend count : " + datas.Length);



			if (friend_count == datas.Length)
			{
				//do nothing
			}
            else
            {
				friend_count = datas.Length;
				foreach (Transform child in friendContent)
				{
					Destroy(child.gameObject);
				}

				for (int i = 0; i < datas.Length; i++)
				{
					var item = Instantiate(friendItem, default, Quaternion.identity);
					if (datas[i].id == GameManager.instance.id)
					{
						item.friend_name.text = datas[i].friend_id;
					}
					else
					{
						item.friend_name.text = datas[i].id;
					}

					item.transform.SetParent(friendContent);
				}
			}


		}


	}

    public void OpenChatRoom(FriendList list)
    {
        
		
		chatroom.gameObject.SetActive(true);
		chatroom.OpenChatRoom(GameManager.instance.id, list.friend_name.text.ToString());
	}
}
