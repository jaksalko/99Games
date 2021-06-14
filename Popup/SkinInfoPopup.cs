using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinInfoPopup : MonoBehaviour
{
    public Text skinNameText;
    public Image skinImage;
    public Text skinInfoText;
    public Image skinRankImage;

    public void ActivatePopup(Skin skin)
    {
        skinNameText.text = skin.skinName;
        skinInfoText.text = skin.skinInfo;
        skinImage.sprite = Resources.Load<Sprite>("store/skin/" + skin.path);
        skinRankImage.sprite = Resources.Load<Sprite>("store/rank/" + skin.path[0]);
        gameObject.SetActive(true);
    }

    public void ExitButton()
    {
        gameObject.SetActive(false);
    }
}
