using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    public UserInfo friendInfo;
    public UserFriend friend;
    protected JsonAdapter jsonAdapter;
    public Text nicknameText;
    public Text styleText;
    public Image profileImage;

    public ProfilePopup popup;

    public virtual void Initialize(UserInfo friendinfo , UserFriend friend_,ProfilePopup profilePopup)
    {
        friendInfo = friendinfo;
        friend = friend_;
        nicknameText.text = friendInfo.nickname;
        styleText.text = friendInfo.profile_style;
        profileImage.sprite = Resources.Load<Sprite>("Icon/Skin/"+friendInfo.profile_skin);

        jsonAdapter = JsonAdapter.instance;
        popup = profilePopup;
    }
}
