using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkin : MonoBehaviour
{
	public string skinName { get; set; }
	public Sprite skinImg { get; set; }
	public string skinInfo { get; set; }
	public bool isUsing { get; set; }
	public List<PlayerSkin> playerSkinList = new List<PlayerSkin>();

	public PlayerSkin()
	{
		skinName = "";
		skinImg = null;
		skinInfo = "";
		isUsing = false;
	}

	public List<PlayerSkin> PlayerSkinList()
	{
		playerSkinList.Add(new PlayerSkin());
		playerSkinList[0].skinName = "skin1";
		playerSkinList[0].skinImg = Resources.Load<Sprite>("Assets/Asset/Character_dark_ui.jpg");
		playerSkinList[0].skinInfo = "skin1 information";

		playerSkinList.Add(new PlayerSkin());
		playerSkinList[1].skinName = "skin2";
		playerSkinList[1].skinImg = null;
		playerSkinList[1].skinInfo = "skin2 information";

		playerSkinList.Add(new PlayerSkin());
		playerSkinList[2].skinName = "skin3";
		playerSkinList[2].skinImg = null;
		playerSkinList[2].skinInfo = "skin3 information";

		return playerSkinList;
	}
}
