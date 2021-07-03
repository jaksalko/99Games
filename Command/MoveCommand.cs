using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Unit_Movement
{
	public Player player;
	public int direction;
	public Vector3 from;
	public Vector3 to;

	public Unit_Movement(Player player_ , int direction_ , Vector3 from_ , Vector3 to_)
    {
		player = player_;
		direction = direction_;
		from = from_;
		to = to_;
    }

	
}

public class MoveCommand : ICommand
{
	List<Unit_Movement> movements;


	public struct PlayerState
    {
		public State state;
		public bool onCloud;
		public bool isLock;
		public int temp;

		public PlayerState(State state_ , bool cloud , bool isLock_ , int tmp)
        {
			state = state_;
			onCloud = cloud;
			isLock = isLock_;
			temp = tmp;
        }
    }

	//make move command
	Player player;//움직일 player
	int dir;//방향
	Map map;//플레이어가 움직일 맵

	//before data
	Vector3 beforePosition_move;
	Vector3 beforePosition_other;

	PlayerState player_move_state;
	PlayerState player_other_state;


	

	int beforeParfaitOrder;


	//after data
	List<Block> stepped_block;
	List<Vector2> beforeSnow; // height,width


	#region Change Object(Snow , cracker , parfait) List




	#endregion




	KeyValuePair<Vector2, int>[] beforeParfaitPos;
	
	List<KeyValuePair<Vector2, Vector2>> beforeCracker; // 후 입력
													//남은 눈의 갯수는 복구된 체크 배열로 다시 계산가능
													//사라진 눈의 재생성도 마찬가지
													//파르페 얼음은 오더로 계산가능
													//크래커가 문제...

	public MoveCommand(List<Unit_Movement> movements)
	{
		this.movements = movements;
		

		// playerNum = map.GetBlockData((int)player.transform.position.x, (int)player.transform.position.z);

		Player player_move = player;
		Player player_other = player.other;

		player_move_state = new PlayerState(player_move.state, player_move.onCloud, player_move.isLock, player_move.temp);
		player_other_state = new PlayerState(player_other.state, player_other.onCloud, player_other.isLock, player_other.temp);

		beforePosition_move = player_move.transform.position;
		beforePosition_other = player_other.transform.position;

		

		beforeParfaitOrder = GameController.ParfaitOrder;

		stepped_block = new List<Block>();
		beforeSnow = new List<Vector2>();
		
	}

	public void SetLaterData(List<Vector2> snowList, List<Block> step_blocks)  //call by player
	{
		for(int i = 0; i < snowList.Count; i++)
        {
			Debug.Log(snowList[i]);
        }
		beforeSnow.AddRange(snowList);
		stepped_block.AddRange(step_blocks);

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
		Player main_player = player;//먼저 움직인 캐릭터
		Player other_player = player.other;//그 다음에 움직인 캐릭터(안 움직였을 수도 있음)

		map.SetBlockData((int)other_player.transform.position.x, (int)other_player.transform.position.z, other_player.temp);
		map.SetBlockData((int)main_player.transform.position.x, (int)main_player.transform.position.z, main_player.temp);
		

		main_player.transform.position = beforePosition_move;
		other_player.transform.position = beforePosition_other;

		Debug.Log(player_move_state.state);

		main_player.temp = player_move_state.temp;
		main_player.onCloud = player_move_state.onCloud;
		main_player.isLock = player_move_state.isLock;
		main_player.state = player_move_state.state;

		other_player.temp = player_other_state.temp;
		other_player.onCloud = player_other_state.onCloud;
		other_player.isLock = player_other_state.isLock;
		other_player.state = player_other_state.state;

		if(main_player.state == State.Master)
        {
			other_player.transform.SetParent(main_player.transform);
        }
		else if(main_player.state == State.Slave)
        {
			main_player.transform.SetParent(other_player.transform);
		}
		else
        {
			other_player.transform.SetParent(null);
			main_player.transform.SetParent(null);
		}


		int main_player_data = (int)main_player.transform.position.y == 1 ? BlockNumber.character : BlockNumber.upperCharacter;
		int other_player_data = (int)other_player.transform.position.y == 1 ? BlockNumber.character : BlockNumber.upperCharacter;

		map.SetBlockData((int)main_player.transform.position.x, (int)main_player.transform.position.z, main_player_data);
		map.SetBlockData((int)other_player.transform.position.x, (int)other_player.transform.position.z, other_player_data);

		
		

		//reset check array
		for (int i = 0; i < beforeSnow.Count; i++)
        {
			int height = (int)beforeSnow[i].x;
			int width = (int)beforeSnow[i].y;
			map.UpdateCheckArray(width, height, false);

			Block block = map.GetBlock(width, height);
			if (block.type == Block.Type.Ground)
			{
				GroundBlock groundBlock = block.GetComponent<GroundBlock>();
				groundBlock.RevertBlock();
			}
		}

		GameController.instance.RemainCheck();
		GameController.instance.moveCount--;
		GameController.instance.UndoCommand();

		if (map.parfait)
        {
			

			foreach (ParfaitBlock parfaitBlock in GameController.instance.mapLoader.parfaitBlock)
			{
				if (beforeParfaitOrder > parfaitBlock.sequence)//DeActivate
				{
					Debug.Log("clear");
					parfaitBlock.ClearParfait();

				}
				else if (beforeParfaitOrder == parfaitBlock.sequence)//Activate
				{
					Debug.Log("active");
					parfaitBlock.Activate();
				}
				else
				{
					Debug.Log("deactive");
					parfaitBlock.DeActivate();

				}
			}

			GameController.ParfaitOrder = beforeParfaitOrder;
		}
		
		//블럭 리셋
		for(int i = 0; i < stepped_block.Count; i++)
        {
			if(stepped_block[i].type == Block.Type.Cracker)
            {
				CrackedBlock crackBlock = stepped_block[i].GetComponent<CrackedBlock>();

				//TriggerExit 코드를 방지하기 위함
				if(crackBlock.crack_ready)//아직 깨지지 않음 -> 캐릭터가 크래커 위에 있음
                {
					crackBlock.crack_ready = false; //움직여도 깨지지 않게 하기 위함
                }
				else//깨졌음 -> 캐릭터가 지나간 블럭
                {
					crackBlock.RevertBlock();
                }
				
            }
			

        }


		


	}


}