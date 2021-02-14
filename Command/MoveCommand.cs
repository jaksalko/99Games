using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveCommand : ICommand
{
	Player player;
	int dir;
	Map map;
	Block[,] blocks;

	#region Change Object(Snow , cracker , parfait) List

	List<Vector2> erasedSnowList;
	int beforeParfaitOrder;

#endregion


#region Character Data   1. state(State)     2. onCloud(bool)      3. temp(int)     4. transform(Transform)

	Vector3 beforePositionA;
	Vector3 beforePositionB;

	Player.State beforeStateA;// idle , master , slave
	Player.State beforeStateB;

	bool beforeOnCloudA;
	bool beforeOnCloudB;

	int beforeTempA;
	int beforeTempB;

	#endregion

	KeyValuePair<Vector2, int>[] beforeParfaitPos;
	List<Vector2> beforeSnow; // 후 입력
	List<KeyValuePair<Vector2, Vector2>> beforeCracker; // 후 입력
													//남은 눈의 갯수는 복구된 체크 배열로 다시 계산가능
													//사라진 눈의 재생성도 마찬가지
													//파르페 얼음은 오더로 계산가능
													//크래커가 문제...

	public MoveCommand(Player p, Map m, int direction)
	{
		player = p;
		map = m;
		dir = direction;

		// playerNum = map.GetBlockData((int)player.transform.position.x, (int)player.transform.position.z);

		Player player1 = player;
		Player player2 = player.other;

		beforePositionA = player1.transform.position;
		beforePositionB = player2.transform.position;

		beforeStateA = player1.state;
		beforeStateB = player2.state;

		beforeTempA = player1.temp;
		beforeTempB = player2.temp;
		Debug.Log("before temp a is : " + beforeTempA);

		beforeOnCloudA = player1.onCloud;
		beforeOnCloudB = player2.onCloud;

		beforeParfaitOrder = GameController.ParfaitOrder;
		beforeParfaitPos = map.FindParfaitBlocks();

		beforeSnow = new List<Vector2>();

		beforeCracker = map.FindCrackerBlocks();
	}

	public void SetLaterData(List<Vector2> snowList, List<KeyValuePair<Vector2, int>> crackerList)  //call by player
	{
		beforeSnow.AddRange(snowList);
		//Map -> ErasedSnowList
	}
	public void Execute()
	{
		player.Move(map, dir);
	}

	public void Undo()
	{
		/*RETURN TO BEFORE STATE
         * GameController
         * Player(Both)
        */

		// throw new System.NotImplementedException();

		Player player1 = player;
		Player player2 = player.other;

		Debug.Log("player 1 current : " + player1.state);
		Debug.Log("player 1 before : " + beforeStateA);

		if (player1.state == beforeStateA)
		{
			if(beforeStateA == Player.State.Idle)
			{
				map.SetBlockData((int)player1.transform.position.x, (int)player1.transform.position.z, player1.temp);

				player1.transform.position = beforePositionA;
				if(beforePositionA.y > 0.5f)
				{
					map.SetBlockData((int)beforePositionA.x, (int)beforePositionA.z, BlockNumber.upperCharacter);
				}
				else
				{
					map.SetBlockData((int)beforePositionA.x, (int)beforePositionA.z, BlockNumber.character);
				}

				player1.temp = beforeTempA;
				// player1.state = beforeStateA;	// Idle
				player1.onCloud = beforeOnCloudA;
			}
			else if(beforeStateA == Player.State.Master)
			{
				map.SetBlockData((int)player1.transform.position.x, (int)player1.transform.position.z, player1.temp);

				player1.transform.position = beforePositionA;
				player2.transform.position = beforePositionB;
				map.SetBlockData((int)beforePositionA.x, (int)beforePositionA.z, BlockNumber.upperCharacter);

				player1.temp = beforeTempA;
				player2.temp = beforeTempB;
				// player1.state = beforeStateA;	// Master
				// player2.state = beforeStateB;	// Slave
				player1.onCloud = beforeOnCloudA;
				player2.onCloud = beforeOnCloudB;
			}
		}
		else
		{
			if(beforeStateA == Player.State.Idle)
			{
				Debug.Log("Idle -> Slave Undo");
				// Slave -> Idle (Can't be Master)
				// current) player1, player2 same position
				map.SetBlockData((int)player1.transform.position.x, (int)player1.transform.position.z, player1.temp);

				player1.transform.position = beforePositionA;
				if (beforePositionA.y > 0.5f)
				{
					map.SetBlockData((int)beforePositionA.x, (int)beforePositionA.z, BlockNumber.upperCharacter);
				}
				else
				{
					map.SetBlockData((int)beforePositionA.x, (int)beforePositionA.z, BlockNumber.character);
				}
				player2.transform.position = beforePositionB;
				if (beforePositionB.y > 0.5f)
				{
					map.SetBlockData((int)beforePositionB.x, (int)beforePositionB.z, BlockNumber.upperCharacter);
				}
				else
				{
					map.SetBlockData((int)beforePositionB.x, (int)beforePositionB.z, BlockNumber.character);
				}

				player1.temp = beforeTempA;
				player2.temp = beforeTempB;

				player1.state = beforeStateA;	// Slave -> Idle
				player2.state = beforeStateB;   // Master -> Idle
				player1.transform.SetParent(null);

				player1.onCloud = beforeOnCloudA;
				player2.onCloud = beforeOnCloudB;
			}
			else if(beforeStateA == Player.State.Master)
			{
				Debug.Log("Master -> Idle Undo");
				// Idle -> Master (Can't be Master -> Slave, Slave -> Master)
				map.SetBlockData((int)player1.transform.position.x, (int)player1.transform.position.z, player1.temp);
				map.SetBlockData((int)player2.transform.position.x, (int)player2.transform.position.z, player2.temp);

				player1.transform.position = beforePositionA;
				player2.transform.position = beforePositionB;
				// before) player1, player2 same position
				map.SetBlockData((int)beforePositionA.x, (int)beforePositionA.z, BlockNumber.upperCharacter);

				player1.temp = beforeTempA;
				player2.temp = beforeTempB;

				player1.state = beforeStateA;	// Idle -> Maste
				player2.state = beforeStateB;   // Idle -> Slave
				player2.transform.SetParent(player1.transform);

				player1.onCloud = beforeOnCloudA;
				player2.onCloud = beforeOnCloudB;
			}
			else if (beforeStateA == Player.State.Slave)
			{
				Debug.Log("Slave -> Idle Undo");
				// Idle -> Slave (Can't be Master -> Slave, Slave -> Master)
				map.SetBlockData((int)player1.transform.position.x, (int)player1.transform.position.z, player1.temp);
				map.SetBlockData((int)player2.transform.position.x, (int)player2.transform.position.z, player2.temp);

				player1.transform.position = beforePositionA;
				player2.transform.position = beforePositionB;
				// before) player1, player2 same position
				map.SetBlockData((int)beforePositionA.x, (int)beforePositionA.z, BlockNumber.upperCharacter);

				player1.temp = beforeTempA;
				player2.temp = beforeTempB;

				player1.state = beforeStateA;   // Idle -> Slave
				player2.state = beforeStateB;   // Idle -> Master
				player1.transform.SetParent(player2.transform);

				player1.onCloud = beforeOnCloudA;
				player2.onCloud = beforeOnCloudB;
			}
		}

		// parfait
		if (GameController.ParfaitOrder != beforeParfaitOrder)
		{
			int pre = beforeParfaitOrder;
			int now = GameController.ParfaitOrder;

			GameObject[] parfaits;
			parfaits = GameObject.FindGameObjectsWithTag("Parfait");

			Transform[] children = parfaits[pre].GetComponentsInChildren<Transform>(true);
			children[1].gameObject.SetActive(true);

			GameObject preObj = parfaits[pre].gameObject;
			preObj.GetComponent<ParfaitBlock>().Activate();

			GameObject nowObj = parfaits[now].gameObject;
			nowObj.GetComponent<ParfaitBlock>().Deactivate();

			GameController.ParfaitOrder = beforeParfaitOrder;
			KeyValuePair<Vector2, int> bp_pos = beforeParfaitPos[beforeParfaitOrder];

			map.SetBlockData((int)bp_pos.Key.y, (int)bp_pos.Key.x, bp_pos.Value);
		}

		// snow
		GameObject[] grounds;
		grounds = GameObject.FindGameObjectsWithTag("Ground");
		foreach (GameObject ground in grounds)
		{
			Vector2 groundpos = new Vector2(ground.transform.position.z, ground.transform.position.x);
			int blockType = map.GetBlockData((int)groundpos.y, (int)groundpos.x);
			if (blockType == BlockNumber.normal || blockType == BlockNumber.upperNormal)
			{
				int index = beforeSnow.IndexOf(groundpos);
				if (index > -1)
				{
					map.UpdateCheckArray((int)groundpos.y, (int)groundpos.x, false);
					GameController.instance.RemainCheck();

					Transform[] children = ground.transform.GetChild(0).GetComponentsInChildren<Transform>(true);
					foreach (Transform child in children)
					{
						child.gameObject.SetActive(true);
					}
				}
			}
		}

		// cracker
		List<KeyValuePair<Vector2, Vector2>> afterCracker = map.FindCrackerBlocks();

		var changeList = beforeCracker.Except(afterCracker);

		foreach (KeyValuePair<Vector2, Vector2> cracker in changeList)
		{
			Vector2 crackerpos = new Vector2(cracker.Key.y, cracker.Key.x);

			int blockType = map.GetBlockData((int)crackerpos.x, (int)crackerpos.y);

			if (blockType >= BlockNumber.cracker_0 && cracker.Value.y <= BlockNumber.broken)
			{
				map.SetBlockData((int)crackerpos.x, (int)crackerpos.y, (int)cracker.Value.y);
			}
			else if (blockType >= BlockNumber.upperBroken && cracker.Value.y <= BlockNumber.upperCracker_2)
			{
				map.SetBlockData((int)crackerpos.x, (int)crackerpos.y, (int)cracker.Value.y);
			}

			map.SetCrackerCount((int)crackerpos.x, (int)crackerpos.y, (int)cracker.Value.x);

		}

	}


}