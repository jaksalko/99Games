using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePopup : MonoBehaviour
{
    [Header("MyInfo")]
    public Text styleText;                  //칭호
    public Text nicknameText;               //닉네임
    public Image profileImage;              //대표이미지
    public Text friendText;                 //친구현황

    [Header("Friend List")]
    public Button friendListButton;
    public GameObject frienListPanel;

    [Header("Friend Search")]
    public GameObject friendSearchPanel;
    public Button friendSearchButton;

    UserInfo userInfo;


    public InputField friendSearchInputField;

    public void ActivatePopup()
    {
        userInfo = GameManager.instance.userInfo;
        nicknameText.text = userInfo.nickname;
        profileImage.sprite = Resources.Load<Sprite>("Icon/Skin/" + userInfo.profile_skin_num);
        Active_FriendListView();
        
    }

    public void Exit()
    {
        gameObject.SetActive(false);
        //초기화
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
        friendSearchInputField.text = "";
    }
    public void Active_HeartReceivePopup()
    {

    }
    
}
