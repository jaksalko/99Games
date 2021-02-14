using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePopup : MonoBehaviour
{
    public Dropdown styleDropdown;
    public Image skinImage;
    public Text nicknameText;
    public InputField introductionField;


    public void SelectMySkin(int skin_num)
    {

    }

    public void ExitProfilePopup()
    {
        gameObject.SetActive(false);
    }

    public void SaveInputTextField()
    {

    }
}
