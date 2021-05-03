using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinStore : MonoBehaviour
{
    public Dropdown sortDropDown;
    public Transform content;
    CSVManager csvManager = CSVManager.instance;

    // Start is called before the first frame update
    void Start()
    {
        csvManager = CSVManager.instance;
    }

    void GetItemList()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
}