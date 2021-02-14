using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class ChatRoom : UIScript
{

	string id_1;
	string id_2;

	JsonAdapter jsonAdapter = new JsonAdapter();
	public InputField txt;

	public ChatList mychat;
	public ChatList friendchat;

	public Transform content;

	float waittime = 1f;
	int chat_count = 0;
    public void OpenChatRoom(string id_1 , string id_2)
    {
		this.id_1 = id_1;
		this.id_2 = id_2;
		StartCoroutine(ChatList(id_1, id_2));
    }

	public IEnumerator ChatList(string id_1 ,string id_2)
	{

		while (true)
		{



			UnityWebRequest www = UnityWebRequest.Get(PrivateData.ec2+"chat/get?id_1=" + id_1 + "&id_2=" + id_2);
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

				Chat[] datas = JsonHelper.FromJson<Chat>(fixdata);
				Debug.Log("chat count : " + datas.Length);

                if(chat_count == datas.Length)
                {
					//do nothing
                }
                else
                {
					chat_count = datas.Length;
					foreach (Transform child in content)
					{
						Destroy(child.gameObject);
					}

					for (int i = 0; i < datas.Length; i++)
					{
						Debug.Log(datas[i].text);

						if (datas[i].id_1 == id_1)//mychat
						{
							var my = Instantiate(mychat, default, Quaternion.identity);
							my.txt.text = datas[i].text;
							my.transform.SetParent(content);
						}
						else
						{
							var friend = Instantiate(friendchat, default, Quaternion.identity);
							friend.txt.text = datas[i].text;
							friend.transform.SetParent(content);
						}
					}


				}



			}

			yield return new WaitForSeconds(waittime);
		}

	}

    public void SendButton()
    {
		StartCoroutine(Send_and_Update());
	}

    IEnumerator Send_and_Update()
    {
		Chat chat = new Chat(id_1, id_2, txt.text.ToString());
		var json = JsonUtility.ToJson(chat);
		
		yield return StartCoroutine(jsonAdapter.API_POST("chat/post", json , callback => { }));
		txt.text = "";
        
        //yield return StartCoroutine(ChatList(id_1, id_2));
		yield break;
	}
}
