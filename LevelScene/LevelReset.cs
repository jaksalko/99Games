using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PressBtn1()
	{
		PlayerPrefs.SetInt("level", 0);
		Debug.Log("----------- tutorial island -----------");
	}

	public void PressBtn2()
	{
		PlayerPrefs.SetInt("level", 5);
		Debug.Log("----------- icecream island -----------");
	}
}
