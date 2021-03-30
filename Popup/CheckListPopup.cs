using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckListPopup : MonoBehaviour
{
    public Image checkList_character;
    public Image checkList_parfait;



    public void ExitButtonClicked()
    {
        gameObject.SetActive(false);
    }

    public void SetCheckList(int character , int parfait)
    {
        if (character == 2)
        {
            checkList_character.sprite = Resources.Load<Sprite>("Editor/Popup/CheckList_1_on");
        }
        else
        {
            checkList_character.sprite = Resources.Load<Sprite>("Editor/Popup/CheckList_1_off");
        }

        if(parfait == 0 || parfait == 4)
        {
            checkList_parfait.sprite = Resources.Load<Sprite>("Editor/Popup/CheckList_2_on");
        }
        else
        {
            checkList_parfait.sprite = Resources.Load<Sprite>("Editor/Popup/CheckList_2_off");
        }

    }
    
}
