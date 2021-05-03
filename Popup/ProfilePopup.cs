using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ProfilePopup : MonoBehaviour
{

    UserInfo userInfo;
    AWSManager awsManager;

    [Header("MyInfo")]
    public Text styleText;                  //칭호
    public Text nicknameText;               //닉네임
    public Image profileImage;              //대표이미지
    public Text friendText;                 //친구현황
    

    [Header("Friend List")]
    public Button friendListButton;
    public GameObject frienListPanel;
    public Transform listContent;

    [Header("Friend Search")]
    public InputField friendSearchInputField;
    public GameObject friendSearchPanel;
    public Button friendSearchButton;
    public Transform searchContent;


    [Header("Friend Prefabs")]
    public FriendListItem friendListItem; // 2
    public FriendReceiveItem friendReceiveItem; //1
    public FriendSearchItem friendSearchItem; // 검색결과
    public FriendSendItem friendSendItem; //0


    [Header("Friend Categories")]
    public List<UserInfo> searchInfos;
    public List<UserInfo> friendSendInfo; //0
    public List<UserInfo> friendReceiveInfo; //1
    public List<UserInfo> friendListInfo; //2

    [Header("Heart Receive Popup")]
    public GameObject heartReceivePopup;
    public Transform heartReceiveContent;
    public HeartReceiveItem heartReceiveItem;
    public Text heartText;
    public int total_heart;

    List<UserFriend> userFriends;
    public void Refresh()
    {
        friendSendInfo.Clear();
        friendReceiveInfo.Clear();
        friendListInfo.Clear();



        awsManager = AWSManager.instance;
        userInfo = AWSManager.instance.userInfo;
        styleText.text = userInfo.profile_style;
        nicknameText.text = userInfo.nickname;
        profileImage.sprite = Resources.Load<Sprite>("Icon/Skin/" + userInfo.profile_skin);


        userFriends = awsManager.userFriend;
        searchInfos = awsManager.allUserInfo.ToList();


        for (int i = 0; i < userFriends.Count; i++)
        {
            Debug.Log(userFriends[i].nickname_friend);
            /*
            UserInfo friend = null;
            for (int j = 0; j < searchInfos.Count; j++)
            {
                
                if (searchInfos[j].nickname == userFriends[i].nickname_friend)
                {
                    friend = searchInfos[j];
                    Debug.Log(friend.nickname);
                    break;
                }
            }
            */
            UserInfo friend = searchInfos.Find((x) => x.nickname == userFriends[i].nickname_friend);
           
            if(userFriends[i].state == 0)
            {
                friendSendInfo.Add(friend);
                searchInfos.Remove(friend);
            }
            else if(userFriends[i].state == 1)
            {
                friendReceiveInfo.Add(friend);
                searchInfos.Remove(friend);
            }
            else if (userFriends[i].state == 2)
            {
                friendListInfo.Add(friend);
                searchInfos.Remove(friend);
            }
        }

        friendText.text = friendListInfo.Count + "[" + friendReceiveInfo.Count + "]/999"; 
        Search_Friend();//검색 이름에 있는 이름으로 친구 리스트를 생성
        RefreshListView();//내 친구목록 리스트를 생성함과 동시에 친구목록으로 이동
        GetHeartReceive();


    }
    void GetHeartReceive()
    {
        foreach (Transform child in heartReceiveContent)
        {
            GameObject.Destroy(child.gameObject);
        }

        total_heart = 0;

        List<Mailbox> mailboxes = AWSManager.instance.mailbox;
        foreach(Mailbox mail in mailboxes)
        {
            if(mail.item == "heart")
            {
                HeartReceiveItem item = Instantiate(heartReceiveItem);
                item.Initialize(mail,this);
                item.transform.SetParent(heartReceiveContent, false);
                total_heart++;
                //heartText.text = total_heart.ToString();
            }
        }
        heartText.text = total_heart.ToString();
    }

    public void Activate()//call by profile button clicked
    {
        JsonAdapter.instance.GetAllUserInfo(RefreshAllUserInfo);
                
    }
    public void RefreshAllUserInfo(bool success)
    {
        if(success)
        {
            if(JsonAdapter.instance.EndLoading())
            {
                JsonAdapter.instance.GetAllUserData(AWSManager.instance.userInfo.nickname, Refresh);
            }
        }
        else
        {

        }
    }
    public void Exit()
    {
        gameObject.SetActive(false);
        //초기화
    }
    public void ExitHeartPopup()
    {
        heartReceivePopup.SetActive(false);
    }

    public void RefreshListView()
    {
        foreach (Transform child in listContent)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < friendReceiveInfo.Count; i++)
        {
            UserFriend friend = userFriends.Find(x => x.nickname_friend == friendReceiveInfo[i].nickname);
            FriendReceiveItem listItem = Instantiate(friendReceiveItem);
            listItem.Initialize(friendReceiveInfo[i], friend, this);
            listItem.transform.SetParent(listContent, false);


        }

        for (int i = 0; i < friendListInfo.Count; i++)
        {
            UserFriend friend = userFriends.Find(x => x.nickname_friend == friendListInfo[i].nickname);
            FriendListItem listItem = Instantiate(friendListItem);
            listItem.Initialize(friendListInfo[i],friend, this);
            listItem.transform.SetParent(listContent, false);
            
            
        }

        
    }
    public void Active_FriendListView()
    {
        friendListButton.interactable = false;
        friendSearchButton.interactable = true;

        frienListPanel.SetActive(true);
        friendSearchPanel.SetActive(false);
    }
    public void Active_FriendSearchView()
    {
        friendSearchButton.interactable = false;
        friendListButton.interactable = true;

        frienListPanel.SetActive(false);
        friendSearchPanel.SetActive(true);
    }
    public void Search_Friend()
    {
        foreach (Transform child in searchContent)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (friendSearchInputField.text == "")
            return;

        for (int i = 0; i < friendReceiveInfo.Count; i++)
        {
            if(friendReceiveInfo[i].nickname.Contains(friendSearchInputField.text))
            {
                UserFriend friend = userFriends.Find(x => x.nickname_friend == friendReceiveInfo[i].nickname);
                FriendReceiveItem listItem = Instantiate(friendReceiveItem);
                listItem.Initialize(friendReceiveInfo[i], friend,this);
                listItem.transform.SetParent(searchContent, false);
            }
            


        }

        for (int i = 0; i < friendSendInfo.Count; i++)
        {
           
            if (friendSendInfo[i].nickname.Contains(friendSearchInputField.text))
            {

                UserFriend friend = userFriends.Find(x => x.nickname_friend == friendSendInfo[i].nickname);
                Debug.Log(friendSendInfo[i].nickname);
                FriendSendItem listItem = Instantiate(friendSendItem);
                listItem.Initialize(friendSendInfo[i], friend,this);
                listItem.transform.SetParent(searchContent, false);
            }
                


        }

        for (int i = 0; i < searchInfos.Count; i++)
        {
            if(searchInfos[i].nickname.Contains(friendSearchInputField.text))
            {
                UserFriend friend = userFriends.Find(x => x.nickname_friend == searchInfos[i].nickname);
                FriendSearchItem listItem = Instantiate(friendSearchItem);
                listItem.Initialize(searchInfos[i],friend, this);
                listItem.transform.SetParent(searchContent,false);
                
            }
        }


    }
    public void Active_HeartReceivePopup()
    {
        heartReceivePopup.SetActive(true);
    }

    void Refresh(bool success)
    {
        if (success)
        {
            if (JsonAdapter.instance.EndLoading())
            {
                Refresh();
                Active_FriendListView();
                gameObject.SetActive(true);
            }
        }
        else
        {

        }
    }



}
