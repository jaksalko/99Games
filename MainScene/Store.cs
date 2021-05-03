using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public Button[] tapButton; // skin block boong heart
    public GameObject[] storeView;


    private void Awake()
    {

    }

    public void TapButtonClicked(int num)
    {
        for(int i = 0; i < tapButton.Length; i++)
        {
            if (i == num)
            {
                tapButton[i].interactable = false;
                storeView[i].SetActive(true);
            }                
            else
            {
                tapButton[i].interactable = true;
                storeView[i].SetActive(false);
            }
                


        }
    }

    public void BuySkinForBoong(string name)
    {

    }

    public void BuySkinForPowder(string name)
    {

    }

    public void BuyBlockForPowder(string name)
    {

    }

    /// <summary>
    /// 확정아님
    /// </summary>
    public void BuyBoongForCash()
    {

    }

    public void BuyHeartForCash()
    {

    }

    public void BackToLobby()
    {
        gameObject.SetActive(false);
    }

}
