using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevData : MonoBehaviour
{
    public Text version;
    public Text userID;

    public void SetVersion()
    {
        version.text = "version : " + Application.version;
    } 

    public void SetUserID(string nickname)
    {
        userID.text = "UID : " + nickname;
    }

    private void Start()
    {
        SetVersion();
    }
}
